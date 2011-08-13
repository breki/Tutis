using System;
using System.Collections.Generic;
using ProtoBuf;

namespace PBuff.Sample2
{
    [ProtoContract]
    public class GpsTrack
    {
        [ProtoMember(1, Name = "points")]
        public IList<GpsPoint> Points { get; set; }

        public float? MaxElevation 
        { 
            get 
            { 
                float? max = null;
                foreach (GpsPoint gpsPoint in Points)
                {
                    if (max == null)
                        max = gpsPoint.Elevation;
                    else if (gpsPoint.Elevation.HasValue)
                        max = Math.Max(max.Value, gpsPoint.Elevation.Value);
                }

                return max;
            }
        }
    }
}