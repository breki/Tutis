using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class PrimitiveGroup
    {
        [ProtoMember (1, Name = "nodes")]
        public IList<PbfNode> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        [ProtoMember (2, Name = "dense", IsRequired = false)]
        public DenseNodes DenseNodes { get; set; }

        [ProtoMember (3, Name = "ways")]
        public IList<PbfWay> Ways
        {
            get { return ways; }
            set { ways = value; }
        }

        [ProtoMember (4, Name = "relations")]
        public IList<PbfRelation> Relation
        {
            get { return relation; }
            set { relation = value; }
        }

        [ProtoMember (5, Name = "changesets")]
        public IList<ChangeSet> ChangeSets
        {
            get { return changeSets; }
            set { changeSets = value; }
        }

        private IList<PbfNode> nodes = new List<PbfNode>();
        private IList<PbfWay> ways = new List<PbfWay> ();
        private IList<PbfRelation> relation = new List<PbfRelation> ();
        private IList<ChangeSet> changeSets = new List<ChangeSet> ();
    }
}