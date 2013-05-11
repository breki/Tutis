using Brejc.OsmLibrary;

namespace SpatialitePlaying.CustomPbf
{
    public interface IOsmObjectDiscovery
    {
        void Begin();
        void End();

        void ProcessBoundingBox(OsmBoundingBox osmBoundingBox);
        void ProcessNode(OsmNode node);
        void ProcessWay(OsmWay way);
        void ProcessRelation(OsmRelation relation);
    }
}