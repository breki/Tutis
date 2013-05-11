using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.OsmLibrary;
using Brejc.Rdbms;
using NUnit.Framework;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class OsmFileProcessor : IOsmDataStorage, IOsmDataBulkInsertSession
    {
        public OsmFileProcessor(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public string AttributionText { get; set; }

        public IOsmDataBulkInsertSession StartBulkInsertSession (bool threadSafe)
        {
            Console.WriteLine ("Started reading OSM data...");

            nodesStorage = new NodesStorage("nodes.dat", fileSystem);
            nodesStorage.InitializeForWriting();

            PrepareSqliteDb();

            return this;
        }

        public void EndBulkInsertSession ()
        {
            Console.WriteLine ("Recovering geometries...");

            using (IDbCommand command = dbConnection.CreateCommand ())
            {
                command.CommandText = @"SELECT RecoverGeometryColumn('areas','geom',4326,'POLYGON', 2)";
                long value = (long)command.ExecuteScalar ();
                Assert.AreEqual (1, value, "RecoverGeometryColumn failed");
            }

            Console.WriteLine ("Creating spatial index...");

            using (IDbCommand command = dbConnection.CreateCommand ())
            {
                command.CommandText = @"SELECT CreateSpatialIndex('areas','geom')";
                long value = (long)command.ExecuteScalar ();
                Assert.AreEqual (1, value, "CreateSpatialIndex failed");
            }
        }

        public void Dispose ()
        {
        }

        public void AddNode (OsmNode node)
        {
            nodesStorage.WriteNode(node.ObjectId, node.X, node.Y);
            nodesCount++;
            if (nodesCount % 200000 == 0)
                Console.WriteLine("Added {0} nodes", nodesCount);
        }

        public void AddWay (OsmWay way)
        {
            if (!initializedForReading)
            {
                nodesStorage.CloseForWriting();
                nodesStorage.InitializeForReading();
                initializedForReading = true;
                Console.WriteLine("Started reading ways...");
                stopwatch.Start();
            }

            if (way.IsClosed)
                return;

            cachedWays.Add(way);

            if (cachedWays.Count > 10000)
            {
                SortedSet<long> neededNodes = new SortedSet<long>();
                foreach (OsmWay cachedWay in cachedWays)
                {
                    foreach (long nodeId in cachedWay.Nodes)
                    {
                        if (nodeId < 0)
                            Debugger.Break();

                        neededNodes.Add(nodeId);
                    }
                }

                IDictionary<long, NodeData> nodesDict = nodesStorage.FetchNodes(neededNodes);

                addedRowsCount += cachedWays.Count;
                //DBBulkProcessor.BulkInsert (
                //    SqliteHelper.SqliteProviderFactory,
                //    dbConnection,
                //    "SELECT id, geom FROM areas",
                //    "INSERT INTO areas (id, geom) VALUES (@param1, @param2)",
                //    dt =>
                //    {
                //        foreach (OsmWay cachedWay in cachedWays)
                //        {
                //            DataRow row = dt.NewRow ();
                //            row[0] = 1;
                //            row[1] = WkbPolygonFromNodes (cachedWay, nodesDict);
                //            dt.Rows.Add (row);

                //            addedRowsCount++;
                //        }
                //    });

                cachedWays.Clear();

                long elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000;
                Console.Out.WriteLine (
                    "Added {0} rows in {1} s ({2} rows/s)",
                    addedRowsCount,
                    elapsedSeconds,
                    ((double)addedRowsCount) / elapsedSeconds);
            }
        }

        public void AddRelation (OsmRelation relation)
        {
        }

        public void AddBoundingBox (OsmBoundingBox box)
        {
        }

        public void AddNote (string noteType, string note)
        {
        }

        private void PrepareSqliteDb()
        {
            string sqliteFileName = "test.sqlite";

            fileSystem.DeleteFile(sqliteFileName, false);

            string connString = SqliteHelper.ConstructConnectionString(sqliteFileName, false);

            DbProviderFactory dbProviderFactory = SqliteHelper.SqliteProviderFactory;
            dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = connString;
            dbConnection.Open();

            Console.WriteLine("Loading spatialite...");

            dbConnection.ExecuteCommand("PRAGMA journal_mode=OFF");
            dbConnection.ExecuteCommand("PRAGMA count_changes=OFF");
            dbConnection.ExecuteCommand("PRAGMA cache_size=4000");

            using (IDbCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = @"SELECT load_extension('libspatialite-4.dll');";
                command.ExecuteNonQuery();
            }

            Console.WriteLine ("Initializing spatial metadata...");

            using (IDbCommand command = dbConnection.CreateCommand ())
            {
                command.CommandText = @"SELECT InitSpatialMetadata();";
                long value = (long)command.ExecuteScalar ();
                Assert.AreEqual (1, value);
            }

            Console.WriteLine ("Creating table...");

            using (IDbCommand command = dbConnection.CreateCommand ())
            {
                command.CommandText = @"CREATE TABLE areas (id INTEGER, geom BLOB NOT NULL)";
                command.ExecuteNonQuery ();
            }
        }

        private static byte[] WkbPolygonFromNodes (IOsmNodesList nodes, IDictionary<long, NodeData> nodesDict)
        {
            using (MemoryStream stream = new MemoryStream ())
            using (BinaryWriter writer = new BinaryWriter (stream))
            {
                writer.Write ((byte)0);
                writer.Write ((byte)1); // little-endian
                writer.Write (4326); // SRID

                List<IPointD2> nodesList = new List<IPointD2> ();
                Bounds2 mbr = new Bounds2 ();

                foreach (long nodeId in nodes.Nodes)
                {
                    NodeData nodeData = nodesDict[nodeId];

                    if (nodeData != null)
                    {
                        nodesList.Add (new PointD2(nodeData.X, nodeData.Y));
                        mbr.ExtendToCover (nodeData.X, nodeData.Y);
                    }
                }

                writer.Write (mbr.MinX);
                writer.Write (mbr.MinY);
                writer.Write (mbr.MaxX);
                writer.Write (mbr.MaxY);
                writer.Write ((byte)0x7C);

                writer.Write (3);
                writer.Write (1); // number of rings

                writer.Write (nodesList.Count);
                foreach (PointD2 node in nodesList)
                {
                    writer.Write (node.X);
                    writer.Write (node.Y);
                }

                writer.Write ((byte)0xFE);

                writer.Flush ();
                return stream.ToArray ();
            }
        }

        private readonly IFileSystem fileSystem;
        private List<OsmWay> cachedWays = new List<OsmWay>();
        private int nodesCount;
        private INodesStorage nodesStorage;
        private bool initializedForReading;
        private static IDbConnection dbConnection;
        private int addedRowsCount;
        private Stopwatch stopwatch = new Stopwatch();
    }
}