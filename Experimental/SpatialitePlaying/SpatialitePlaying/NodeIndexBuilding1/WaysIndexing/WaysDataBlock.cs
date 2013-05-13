using System;

namespace SpatialitePlaying.NodeIndexBuilding1.WaysIndexing
{
    public class WaysDataBlock
    {
        public WaysDataBlock(int waysCount)
        {
            wayData = new WayData[waysCount];
        }

        public WayData GetWayData(long id, ref int? startingIndex)
        {
            for (int i = startingIndex ?? 0; i < wayData.Length; i++)
            {
                WayData way = wayData[i];
                long indexedWaysId = way.WayId;
                if (indexedWaysId == id)
                {
                    startingIndex = i + 1;
                    return way;
                }

                if (indexedWaysId > id)
                    throw new InvalidOperationException ("The node does not belong to this block");
            }

            throw new InvalidOperationException ("The node was not found");
        }

        public void SetWayData(int i, WayData data)
        {
            wayData[i] = data;
        }

        private WayData[] wayData;
    }
}