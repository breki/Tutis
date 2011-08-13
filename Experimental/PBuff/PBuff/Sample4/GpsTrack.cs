using System;
using System.Collections.Generic;
using ProtoBuf;

namespace PBuff.Sample4
{
    [ProtoContract]
    public class GpsTrack
    {
        public const float MagicElevation = -5000;
        public const double HGranularity = 0.000001;
        public const float VGranularity = 0.1f;

        [ProtoMember(1, Name = "x", Options = MemberSerializationOptions.Packed)]
        public IList<long> X { get; set; }

        [ProtoMember(2, Name = "y", Options = MemberSerializationOptions.Packed)]
        public IList<long> Y { get; set; }

        [ProtoMember(3, Name = "elevation", Options = MemberSerializationOptions.Packed)]
        public IList<int> Elevation { get; set; }

        [ProtoMember(4, Name = "time", Options = MemberSerializationOptions.Packed)]
        public IList<ulong> Time { get; set; }

        public float? MaxElevation 
        { 
            get 
            { 
                int? max = null;
                foreach (int elevation in Elevation)
                {
                    if (max == null)
                        max = elevation;
                    else 
                        max = Math.Max(max.Value, elevation);
                }

                if (max.HasValue)
                    return max.Value * VGranularity;

                return null;
            }
        }
    }
}