using System;
using ProtoBuf;

namespace PBuff.Sample1
{
    [ProtoContract]
    public class GpsPoint
    {
        [ProtoMember(1, Name = "x", IsRequired = true)]
        public double X { get; set; }

        [ProtoMember(2, Name = "y", IsRequired = true)]
        public double Y { get; set; }

        [ProtoMember(3, Name = "elevation", IsRequired = false)]
        public float? Elevation { get; set; }

        [ProtoMember(4, Name = "time", IsRequired = false)]
        public DateTime? Time { get; set; }

        public override bool Equals(object obj)
        {
            GpsPoint other = obj as GpsPoint;
            if (other == null)
                return false;

            return X == other.X && Y == other.Y && Equals(Time, other.Time) && Equals(Elevation, other.Elevation);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}