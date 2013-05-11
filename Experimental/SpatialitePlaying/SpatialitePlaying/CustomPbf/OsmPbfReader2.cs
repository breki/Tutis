using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Brejc.Common.FileSystem;
using Brejc.Common.Tasks;
using Brejc.Geometry;
using Brejc.OsmLibrary;
using Brejc.OsmLibrary.Pbf;
using SpatialitePlaying.CustomPbf;
using Ionic.Zlib;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    public class OsmPbfReader2 : IOsmReader
    {
        public bool AllowCancellation
        {
            get { return allowCancellation; }
            set { allowCancellation = value; }
        }

        public OsmReaderSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public ITaskExecutionContext TaskExecutionContext
        {
            get { return taskExecutionContext; }
            set { taskExecutionContext = value; }
        }

        public void Read (Stream stream, IOsmDataStorage osmStorage)
        {
            nodesCounter = 0;
            relationsCounter = 0;
            waysCounter = 0;
            tagsFactory.Clear ();

            using (BinaryReader binaryReader = new BinaryReader (stream))
            using (bulkInsertSession = osmStorage.StartBulkInsertSession (threadSafe: true))
            {
                ReadBlocks (binaryReader);
            }

            osmStorage.EndBulkInsertSession ();

            if (taskExecutionContext != null && !taskExecutionContext.ShouldAbort)
                taskExecutionContext.Write (
                    "Read {0} nodes, {1} ways and {2} relations in total",
                    nodesCounter,
                    waysCounter,
                    relationsCounter);
        }

        public void Read (string osmFileName, IFileSystem fileSystem, IOsmDataStorage osmStorage)
        {
            using (Stream stream = fileSystem.OpenFileToRead (osmFileName))
            {
                Read (stream, osmStorage);
                return;
            }
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources            
                }

                disposed = true;
            }
        }

        private void ReadBlocks (BinaryReader binaryReader)
        {
            while (binaryReader.PeekChar () != -1)
            {
                int blockHeaderLength = ReadInt32BigEndian (binaryReader.BaseStream);

                if (blockHeaderLength > FasterOsmPbfReader.MaxBlockHeaderSize)
                    throw InvalidFile ("PBF block header exceeds the maximum size.");

                BlockHeader header;
                using (Stream dataStream = ReadBlockAsStream (binaryReader, blockHeaderLength))
                {
                    header = Serializer.Deserialize<BlockHeader> (dataStream);
                }

                Blob blob;
                using (Stream dataStream = ReadBlockAsStream (binaryReader, header.DataSize))
                {
                    blob = Serializer.Deserialize<Blob> (dataStream);
                }

                if (blob.RawSize.HasValue && blob.RawSize >= FasterOsmPbfReader.MaxBlockUncompressedSize)
                    throw InvalidFile ("PBF blob exceeds the maximum size.");

                Stream blobStream;
                if (blob.Raw != null)
                    blobStream = new MemoryStream (blob.Raw);
                else
                    blobStream = new ZlibStream (new MemoryStream (blob.ZLibData), CompressionMode.Decompress);

                using (blobStream)
                {
                    if (header.Type == "OSMHeader")
                    {
                        //string proto = Serializer.GetProto<OsmHeader>();
                        //log.Debug(proto);
                        OsmHeader osmHeader = Serializer.Deserialize<OsmHeader> (blobStream);
                        if (osmHeader.BBox != null)
                        {
                            Bounds2 bounds = new Bounds2 (
                                osmHeader.BBox.Left * FasterOsmPbfReader.Nanodegree,
                                osmHeader.BBox.Bottom * FasterOsmPbfReader.Nanodegree,
                                osmHeader.BBox.Right * FasterOsmPbfReader.Nanodegree,
                                osmHeader.BBox.Top * FasterOsmPbfReader.Nanodegree);
                            bounds.Normalize ();
                            OsmBoundingBox osmBoundingBox = new OsmBoundingBox (bounds, osmHeader.Source);
                            bulkInsertSession.AddBoundingBox (osmBoundingBox);
                        }
                        //log.DebugFormat ("OSMHeader");
                    }
                    else if (header.Type == "OSMData")
                    {
                        //string proto = Serializer.GetProto<PrimitiveBlock>();
                        //log.Debug(proto);
                        PrimitiveBlock primitiveBlock = Serializer.Deserialize<PrimitiveBlock> (blobStream);
                        //log.DebugFormat(
                        //    "PrimitiveBlock ({0} groups, {1} strings)", 
                        //    primitiveBlock.PrimitiveGroups.Count,
                        //    primitiveBlock.StringTable.S.Count);

                        foreach (PrimitiveGroup primitiveGroup in primitiveBlock.PrimitiveGroups)
                        {
                            ProcessPrimitiveGroup (primitiveBlock, primitiveGroup);
                            //log.DebugFormat(
                            //    "PrimitiveGroup ({0} nodes, {1} ways, {2} relations)",
                            //    primitiveGroup.Nodes.Count,
                            //    primitiveGroup.Ways.Count,
                            //    primitiveGroup.Relation.Count);
                        }
                    }
                }
            }
        }

        private void ProcessPrimitiveGroup (PrimitiveBlock primitiveBlock, PrimitiveGroup primitiveGroup)
        {
            ProcessNodes (primitiveGroup, primitiveBlock);
            ProcessDenseNodes (primitiveGroup, primitiveBlock);
            ProcessWays (primitiveGroup, primitiveBlock);
            ProcessRelations (primitiveGroup, primitiveBlock);
        }

        private void ProcessNodes (PrimitiveGroup primitiveGroup, PrimitiveBlock primitiveBlock)
        {
            if (!settings.SkipNodes)
            {
                foreach (PbfNode pbfNode in primitiveGroup.Nodes)
                {
                    double y = FasterOsmPbfReader.Nanodegree * (primitiveBlock.LatOffset + (primitiveBlock.Granularity * pbfNode.Lat));
                    double x = FasterOsmPbfReader.Nanodegree * (primitiveBlock.LonOffset + (primitiveBlock.Granularity * pbfNode.Lon));
                    OsmNode node = new OsmNode (pbfNode.Id, x, y);

                    if (pbfNode.Keys != null)
                    {
                        List<Tag> tags = new List<Tag> ();
                        for (int i = 0; i < pbfNode.Keys.Count; i++)
                        {
                            string key = primitiveBlock.StringTable[pbfNode.Keys[i]];
                            string value = primitiveBlock.StringTable[pbfNode.Vals[i]];

                            if (false == settings.IgnoreCreatedByTags
                                || 0 != string.Compare (key, "created_by", StringComparison.Ordinal))
                                tags.Add (tagsFactory.GetTag (key, value));
                        }

                        if (tags.Count > 0)
                            node.SetTags (tags);
                    }

                    if (settings.LoadExtendedData)
                        FillExtendedData (node, pbfNode.Info, primitiveBlock);
                    bulkInsertSession.AddNode (node);
                }

                nodesCounter += primitiveGroup.Nodes.Count;
            }
        }

        private void ProcessDenseNodes (PrimitiveGroup primitiveGroup, PrimitiveBlock primitiveBlock)
        {
            if (primitiveGroup.DenseNodes == null)
                return;

            if (!settings.SkipNodes)
            {
                long id = 0;
                long lon = 0;
                long lat = 0;
                long changeset = 0;
                long timestamp = 0;
                int userId = 0;
                int userSid = 0;

                DenseInfo denseInfo = primitiveGroup.DenseNodes.DenseInfo;
                bool hasExtendedData = denseInfo != null;

                IList<int> keysVals = primitiveGroup.DenseNodes.KeysVals;
                int totalKeysValsCount = keysVals.Count;
                bool hasKeysVals = keysVals != null && totalKeysValsCount > 0;

                List<Tag> collectedTags = new List<Tag> ();

                int keysValsIndex = 0;
                for (int i = 0; i < primitiveGroup.DenseNodes.Id.Count; i++)
                {
                    long deltaId = primitiveGroup.DenseNodes.Id[i];
                    long deltaLon = primitiveGroup.DenseNodes.Lon[i];
                    long deltaLat = primitiveGroup.DenseNodes.Lat[i];
                    id += deltaId;
                    lon += deltaLon;
                    lat += deltaLat;

                    double y = FasterOsmPbfReader.Nanodegree * (primitiveBlock.LatOffset + (primitiveBlock.Granularity * lat));
                    double x = FasterOsmPbfReader.Nanodegree * (primitiveBlock.LonOffset + (primitiveBlock.Granularity * lon));

                    OsmNode node = new OsmNode (id, x, y);

                    if (hasKeysVals)
                    {
                        while (keysValsIndex < totalKeysValsCount)
                        {
                            int keyId = keysVals[keysValsIndex];

                            // did we reach the delimiter for this node?
                            if (keyId > 0)
                            {
                                int valId = keysVals[keysValsIndex + 1];
                                string key = primitiveBlock.StringTable[keyId];
                                string value = primitiveBlock.StringTable[valId];

                                if (false == settings.IgnoreCreatedByTags
                                    || 0 != string.Compare (key, "created_by", StringComparison.Ordinal))
                                    collectedTags.Add (tagsFactory.GetTag (key, value));

                                keysValsIndex += 2;
                            }
                            else
                            {
                                keysValsIndex++;
                                break;
                            }
                        }

                        if (collectedTags.Count > 0)
                        {
                            node.SetTags (collectedTags);
                            collectedTags.Clear ();
                        }
                    }

                    if (hasExtendedData && settings.LoadExtendedData)
                    {
                        node.ExtendedData = new OsmObjectExtendedData ();

                        changeset += denseInfo.ChangeSet[i];
                        node.ExtendedData.ChangesetId = (int)changeset;

                        timestamp += denseInfo.Timestamp[i];
                        node.ExtendedData.Timestamp = UnixEpoch.Add (
                            new TimeSpan (timestamp * primitiveBlock.DateGranularity * TimeSpan.TicksPerMillisecond));

                        userId += denseInfo.UserId[i];
                        node.ExtendedData.UserId = userId;

                        userSid += denseInfo.UserSid[i];
                        node.ExtendedData.User = primitiveBlock.StringTable[userSid];
                        if (node.ExtendedData.User.Length == 0)
                            node.ExtendedData.User = null;

                        node.ExtendedData.Version = denseInfo.Version[i];
                    }

                    bulkInsertSession.AddNode (node);
                }
            }

            nodesCounter += primitiveGroup.DenseNodes.Id.Count;
        }

        private void ProcessWays (PrimitiveGroup primitiveGroup, PrimitiveBlock primitiveBlock)
        {
            if (!settings.SkipWays)
            {
                for (int wayIndex = 0; wayIndex < primitiveGroup.Ways.Count; wayIndex++)
                {
                    PbfWay pbfWay = primitiveGroup.Ways[wayIndex];
                    OsmWay way = new OsmWay (pbfWay.Id);
                    if (settings.LoadExtendedData)
                        FillExtendedData (way, pbfWay.Info, primitiveBlock);

                    if (pbfWay.Keys != null)
                    {
                        List<Tag> tags = new List<Tag> ();
                        for (int i = 0; i < pbfWay.Keys.Count; i++)
                        {
                            string key = primitiveBlock.StringTable[pbfWay.Keys[i]];
                            string value = primitiveBlock.StringTable[pbfWay.Vals[i]];

                            if (false == settings.IgnoreCreatedByTags
                                || 0 != string.Compare (key, "created_by", StringComparison.Ordinal))
                                tags.Add (tagsFactory.GetTag (key, value));
                        }

                        if (tags.Count > 0)
                            way.SetTags (tags);
                    }

                    long nodeId = 0;
                    if (pbfWay.Refs != null)
                    {
                        long[] nodesIds = new long[pbfWay.Refs.Count];

                        for (int i = 0; i < pbfWay.Refs.Count; i++)
                        {
                            long deltaNodeId = pbfWay.Refs[i];
                            nodeId += deltaNodeId;
                            nodesIds[i] = nodeId;
                        }

                        way.SetNodes (nodesIds);
                    }

                    bulkInsertSession.AddWay (way);
                }
            }

            waysCounter += primitiveGroup.Ways.Count;
        }

        [SuppressMessage ("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        private void ProcessRelations (PrimitiveGroup primitiveGroup, PrimitiveBlock primitiveBlock)
        {
            if (!settings.SkipRelations)
            {
                foreach (PbfRelation pbfRelation in primitiveGroup.Relation)
                {
                    long id = pbfRelation.Id;
                    OsmRelation relation = new OsmRelation (id);

                    if (pbfRelation.Keys != null)
                    {
                        List<Tag> tags = new List<Tag> ();
                        for (int i = 0; i < pbfRelation.Keys.Count; i++)
                        {
                            string key = primitiveBlock.StringTable[pbfRelation.Keys[i]];
                            string value = primitiveBlock.StringTable[pbfRelation.Vals[i]];

                            if (false == settings.IgnoreCreatedByTags
                                || 0 != string.Compare (key, "created_by", StringComparison.Ordinal))
                                tags.Add (tagsFactory.GetTag (key, value));
                        }

                        if (tags.Count > 0)
                            relation.SetTags (tags);
                    }

                    if (settings.LoadExtendedData)
                        FillExtendedData (relation, pbfRelation.Info, primitiveBlock);

                    long referenceId = 0;
                    for (int i = 0; i < pbfRelation.RolesSid.Count; i++)
                    {
                        string role = primitiveBlock.StringTable[pbfRelation.RolesSid[i]];
                        PbfRelationMemberType pbfReferenceType = pbfRelation.Types[i];
                        OsmReferenceType referenceType;

                        switch (pbfReferenceType)
                        {
                            case PbfRelationMemberType.Node:
                                referenceType = OsmReferenceType.Node;
                                break;
                            case PbfRelationMemberType.Way:
                                referenceType = OsmReferenceType.Way;
                                break;
                            case PbfRelationMemberType.Relation:
                                referenceType = OsmReferenceType.Relation;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException ();
                        }

                        referenceId += pbfRelation.MemIds[i];

                        relation.AddMember (referenceType, referenceId, role);
                    }

                    bulkInsertSession.AddRelation (relation);
                }
            }

            relationsCounter += primitiveGroup.Relation.Count;
        }

        private static void FillExtendedData (OsmObjectBase osmObject, Info info, PrimitiveBlock primitiveBlock)
        {
            if (info != null)
            {
                osmObject.ExtendedData = new OsmObjectExtendedData ();
                if (info.ChangeSet.HasValue)
                    osmObject.ExtendedData.ChangesetId = (int)info.ChangeSet.Value;

                if (info.Timestamp.HasValue)
                    osmObject.ExtendedData.Timestamp = UnixEpoch.Add (
                        new TimeSpan (info.Timestamp.Value * primitiveBlock.DateGranularity * TimeSpan.TicksPerMillisecond));

                if (info.UserId.HasValue)
                    osmObject.ExtendedData.UserId = info.UserId.Value;

                if (info.UserSid.HasValue)
                    osmObject.ExtendedData.User = primitiveBlock.StringTable[info.UserSid.Value];

                if (info.Version.HasValue)
                    osmObject.ExtendedData.Version = info.Version.Value;
            }
        }

        private static readonly DateTime UnixEpoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static Stream ReadBlockAsStream (BinaryReader binaryReader, int length)
        {
            byte[] data = binaryReader.ReadBytes (length);
            return new MemoryStream (data);
        }

        private static int ReadInt32BigEndian (Stream stream)
        {
            short value1 = ReadInt16BigEndian (stream);
            short value2 = ReadInt16BigEndian (stream);

            return value1 << 16 | ((ushort)value2);
        }

        private static short ReadInt16BigEndian (Stream stream)
        {
            byte value1 = (byte)stream.ReadByte ();
            byte value2 = (byte)stream.ReadByte ();

            return (short)(value1 << 8 | value2);
        }

        [SuppressMessage ("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static InvalidOsmFileException InvalidFile (string messageFormat, params object[] args)
        {
            string message = string.Format (
                CultureInfo.InvariantCulture,
                messageFormat,
                args);
            throw new InvalidOsmFileException (message);
        }

        private bool allowCancellation;
        private IOsmDataBulkInsertSession bulkInsertSession;
        private OsmReaderSettings settings = new OsmReaderSettings ();
        private ITaskExecutionContext taskExecutionContext;
        private int nodesCounter;
        private int waysCounter;
        private int relationsCounter;
        private ITagFactory tagsFactory = new TagFactory ();
        //private static readonly ILog log = LogManager.GetLogger(typeof(OsmPbfReader));
        private bool disposed;
    }
}