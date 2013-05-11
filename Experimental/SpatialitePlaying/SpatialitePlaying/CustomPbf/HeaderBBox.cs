using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class HeaderBBox
    {
        [ProtoMember (1, Name = "left", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Left { get; set; }
        [ProtoMember (2, Name = "right", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Right { get; set; }
        [ProtoMember (3, Name = "top", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Top { get; set; }
        [ProtoMember (4, Name = "bottom", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Bottom { get; set; }
    }
}