﻿using System;
using Brejc.OsmLibrary;

namespace SpatialitePlaying
{
    public class OsmFileAnalyzer : IOsmDataStorage, IOsmDataBulkInsertSession
    {
        public string AttributionText { get; set; }

        public long NodesCount
        {
            get { return nodesCount; }
        }

        public long LastNodeId
        {
            get { return lastNodeId; }
        }

        public bool AreNodeIdsMonotone
        {
            get { return areNodeIdsMonotone; }
        }

        public long WaysCount
        {
            get { return waysCount; }
        }

        public long LastWayId
        {
            get { return lastWayId; }
        }

        public bool AreWaysIdsMonotone
        {
            get { return areWaysIdsMonotone; }
        }

        public long RelationsCount
        {
            get { return relationsCount; }
        }

        public long LastRelationId
        {
            get { return lastRelationId; }
        }

        public bool AreRelationIdsMonotone
        {
            get { return areRelationIdsMonotone; }
        }

        public IOsmDataBulkInsertSession StartBulkInsertSession(bool threadSafe)
        {
            return this;
        }

        public void EndBulkInsertSession()
        {
        }

        public void Dispose()
        {
        }

        public void AddNode(OsmNode node)
        {
            nodesCount++;
            if (areNodeIdsMonotone)
                areNodeIdsMonotone = node.ObjectId > lastNodeId;
            lastNodeId = node.ObjectId;
        }

        public void AddWay(OsmWay way)
        {
            waysCount++;
            if (areWaysIdsMonotone)
                areWaysIdsMonotone = way.ObjectId > lastWayId;
            lastWayId = way.ObjectId;
        }

        public void AddRelation(OsmRelation relation)
        {
            relationsCount++;
            if (areRelationIdsMonotone)
                areRelationIdsMonotone = relation.ObjectId > lastRelationId;
            lastRelationId = relation.ObjectId;
        }

        public void AddBoundingBox(OsmBoundingBox box)
        {
        }

        public void AddNote(string noteType, string note)
        {
        }

        private long nodesCount;
        private long lastNodeId = -1;
        private bool areNodeIdsMonotone = true;
        private long waysCount;
        private long lastWayId = -1;
        private bool areWaysIdsMonotone = true;
        private long relationsCount;
        private long lastRelationId = -1;
        private bool areRelationIdsMonotone = true;
    }
}