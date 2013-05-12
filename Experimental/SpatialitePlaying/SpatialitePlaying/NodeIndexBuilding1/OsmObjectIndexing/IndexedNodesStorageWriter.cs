using Brejc.Common.FileSystem;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class IndexedNodesStorageWriter : IndexedOsmObjectStorageWriterBase, IIndexedNodesStorageWriter
    {
        public IndexedNodesStorageWriter(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public void StoreNode(long nodeId, double x, double y)
        {
            FlushCurrentBlockIfFull (nodeId);

            Writer.Write (nodeId);
            Writer.Write (x);
            Writer.Write (y);

            IncrementObjectsInBlockCount();
        }

        protected override string ObjectTypeName
        {
            get { return "nodes"; }
        }
    }
}