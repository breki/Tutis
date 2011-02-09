using System;
using System.Collections.Generic;
using ProtoBuf;

namespace PBuff.Sample3
{
    [ProtoContract]
    public class GpsTrack
    {
        public const float MagicElevation = -5000;

        [ProtoMember(1, Name = "x", Options = MemberSerializationOptions.Packed)]
        public IList<double> X { get; set; }

        [ProtoMember(2, Name = "y", Options = MemberSerializationOptions.Packed)]
        public IList<double> Y { get; set; }

        [ProtoMember(3, Name = "elevation", Options = MemberSerializationOptions.Packed)]
        public IList<float> Elevation { get; set; }

        [ProtoMember(4, Name = "time", Options = MemberSerializationOptions.Packed)]
        public IList<ulong> Time { get; set; }

        public float? MaxElevation 
        { 
            get 
            { 
                float? max = null;
                foreach (float elevation in Elevation)
                {
                    if (max == null)
                        max = elevation;
                    else 
                        max = Math.Max(max.Value, elevation);
                }

                return max;
            }
        }
    }
}