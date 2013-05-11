using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class BlockHeader
    {
        [ProtoMember(1, Name="type", IsRequired = true)]
        public string Type { get; set; }

        [ProtoMember (2, Name="indexdata", IsRequired = false)]
        public byte[] IndexData { get; set; }

        [ProtoMember (3, Name = "datasize", IsRequired = true)]
        public int DataSize { get; set; }
    }
}