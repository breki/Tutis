using System;
using System.Collections.Generic;
using ProtoBuf;

namespace PBuff.Sample6
{
    [ProtoContract]
    public class GpsTrack
    {
        public const float MagicElevation = -5000;
        public const double HGranularity = 0.000001;
        public const float VGranularity = 0.1f;

        [ProtoMember(1, Name = "x", Options = MemberSerializationOptions.Packed, DataFormat = DataFormat.ZigZag)]
        public IList<long> X { get; set; }

        [ProtoMember(2, Name = "y", Options = MemberSerializationOptions.Packed, DataFormat = DataFormat.ZigZag)]
        public IList<long> Y { get; set; }

        [ProtoMember(3, Name = "elevation", Options = MemberSerializationOptions.Packed, DataFormat = DataFormat.ZigZag)]
        public IList<int> Elevation { get; set; }

        [ProtoMember(4, Name = "time", Options = MemberSerializationOptions.Packed)]
        public IList<long> Time { get; set; }

        public double GetPointX(int point)
        {
            long x = 0;
            for (int i = 0; i < point; i++)
                x += X[i];

            return x * HGranularity;
        }

        public double GetPointY(int point)
        {
            long y = 0;
            for (int i = 0; i < point; i++)
                y += Y[i];

            return y * HGranularity;
        }

        public double? GetPointElevation(int point)
        {
            int ele = 0;
            for (int i = 0; i < point; i++)
                ele += Elevation[i];

            float elevation = ele * VGranularity;
            if (elevation == MagicElevation)
                return null;

            return elevation;
        }

        public DateTime? GetPointTime(int point)
        {
            long milliseconds = 0;
            for (int i = 0; i < point; i++)
                milliseconds += Time[i];

            return new DateTime(milliseconds * TimeSpan.TicksPerMillisecond);
        }

        public float? MaxElevation 
        { 
            get 
            {
                int? max = null;
                int? elevation = null;
                foreach (int dElevation in Elevation)
                {
                    if (max == null)
                    {
                        max = dElevation;
                        elevation = dElevation;
                    }
                    else
                    {
                        elevation += dElevation;
                        max = Math.Max(max.Value, elevation.Value);
                    }
                }

                if (max.HasValue)
                    return max.Value * VGranularity;

                return null;
            }
        }
    }
}