using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class DenseNodes
    {
        [ProtoMember (1, Name = "id", IsRequired = true, DataFormat = DataFormat.ZigZag, Options = MemberSerializationOptions.Packed)]
        public IList<long> Id
        {
            get { return id; }
            set { id = value; }
        }

        [ProtoMember (5, Name = "denseinfo", IsRequired = false)]
        public DenseInfo DenseInfo { get; set; }

        [ProtoMember (8, Name = "lat", IsRequired = true, DataFormat = DataFormat.ZigZag, Options = MemberSerializationOptions.Packed)]
        public IList<long> Lat
        {
            get { return lat; }
            set { lat = value; }
        }

        [ProtoMember (9, Name = "lon", IsRequired = true, DataFormat = DataFormat.ZigZag, Options = MemberSerializationOptions.Packed)]
        public IList<long> Lon
        {
            get { return lon; }
            set { lon = value; }
        }

        [ProtoMember(10, Name = "keys_vals", Options = MemberSerializationOptions.Packed)]
        public IList<int> KeysVals
        {
            get { return keysVals; }
            set { keysVals = value; }
        }

        private IList<long> id = new List<long>();
        private IList<long> lat = new List<long>();
        private IList<long> lon = new List<long>();
        private IList<int> keysVals = new List<int> ();
    }
}