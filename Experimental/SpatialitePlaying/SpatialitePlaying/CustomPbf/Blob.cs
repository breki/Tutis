using System;
using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class Blob
    {
        [ProtoIgnore]
        public BlockHeader Header
        {
            get; set;
        }

        [ProtoMember (1, Name = "raw", IsRequired = false)]
        public byte[] Raw { get; set; }

        [ProtoMember (2, Name = "raw_size", IsRequired = false)]
        public int? RawSize { get; set; }

        [ProtoMember (3, Name = "zlib_data", IsRequired = false)]
        public byte[] ZLibData { get; set; }
    }
}