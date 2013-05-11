using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    public enum PbfRelationMemberType
    {
        Node = 0,
        Way = 1,
        Relation = 2
    }

    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class PbfRelation
    {
        [ProtoMember(1, Name = "id", IsRequired = true)]
        public long Id { get; set; }

        [ProtoMember(2, Name = "keys", Options = MemberSerializationOptions.Packed)]
        public IList<uint> Keys { get; set; }

        [ProtoMember(3, Name = "vals", Options = MemberSerializationOptions.Packed)]
        public IList<uint> Vals { get; set; }

        [ProtoMember(4, Name = "info", IsRequired = false)]
        public Info Info { get; set; }

        [ProtoMember(8, Name = "roles_sid", Options = MemberSerializationOptions.Packed)]
        public IList<int> RolesSid
        {
            get { return rolesSid; }
            set { rolesSid = value; }
        }

        [ProtoMember (9, Name = "memids", Options = MemberSerializationOptions.Packed, DataFormat = DataFormat.ZigZag)]
        public IList<long> MemIds
        {
            get { return memIds; }
            set { memIds = value; }
        }

        [ProtoMember (10, Name = "memids", Options = MemberSerializationOptions.Packed)]
        public IList<PbfRelationMemberType> Types
        {
            get { return types; }
            set { types = value; }
        }

        private IList<int> rolesSid = new List<int> ();
        private IList<long> memIds = new List<long> ();
        private IList<PbfRelationMemberType> types = new List<PbfRelationMemberType> ();
    }
}