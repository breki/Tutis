using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class PbfNode
    {
        [ProtoMember(1, Name = "id", IsRequired = true)]
        public long Id { get; set; }

        [ProtoMember(2, Name = "keys", Options = MemberSerializationOptions.Packed)]
        public IList<uint> Keys { get; set; }

        [ProtoMember(3, Name = "vals", Options = MemberSerializationOptions.Packed)]
        public IList<uint> Vals { get; set; }

        [ProtoMember(4, Name = "info", IsRequired = false)]
        public Info Info { get; set; }

        [ProtoMember (8, Name = "lat", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Lat { get; set; }

        [ProtoMember (9, Name = "lon", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Lon { get; set; }
    }
}