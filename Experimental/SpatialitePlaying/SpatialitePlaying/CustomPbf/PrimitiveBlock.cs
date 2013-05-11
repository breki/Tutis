using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class PrimitiveBlock
    {
        [ProtoMember (1, Name = "stringtable", IsRequired = true)]
        public StringTable StringTable { get; set; }

        [ProtoMember (2, Name = "primitivegroup")]
        public IList<PrimitiveGroup> PrimitiveGroups
        {
            get { return primitiveGroups; }
            set { primitiveGroups = value; }
        }

        [ProtoMember (16, Name = "granularity", IsRequired = false)]
        public int Granularity
        {
            get { return granularity; }
            set { granularity = value; }
        }

        [ProtoMember (19, Name = "lat_offset", IsRequired = false)]
        public long LatOffset { get; set; }

        [ProtoMember (20, Name = "lon_offset", IsRequired = false)]
        public long LonOffset { get; set; }

        [ProtoMember (18, Name = "date_granularity", IsRequired = false)]
        public int DateGranularity
        {
            get { return dateGranularity; }
            set { dateGranularity = value; }
        }

        private IList<PrimitiveGroup> primitiveGroups = new List<PrimitiveGroup> ();
        private int granularity = 100;
        private int dateGranularity = 1000;
    }
}