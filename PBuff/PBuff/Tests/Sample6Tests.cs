using System;
using System.Collections.Generic;
using System.IO;
using Brejc.GpsLibrary.Gpx;
using MbUnit.Framework;
using PBuff.Sample6;
using ProtoBuf;

namespace PBuff.Tests
{
    public class Sample6Tests
    {
        [Test]
        public void Sample6()
        {
            GpxFile gpxFile = GpxFile.Load(@"..\..\..\GPS\2010.06.17.gpx");

            GpsTrack track = new GpsTrack();
            track.X = new List<long>();
            track.Y = new List<long>();
            track.Elevation = new List<int>();
            track.Time = new List<long>();

            long x = 0;
            long y = 0;
            int elevation = 0;
            long time = 0;

            foreach (ITrackSegment segment in gpxFile.Tracks[0].Segments)
            {
                foreach (ITrackPoint trackPoint in segment.Points)
                {
                    long cx = ((long)(Math.Round(trackPoint.X / GpsTrack.HGranularity)));
                    track.X.Add(cx - x);
                    x = cx;

                    long cy = ((long)(Math.Round(trackPoint.Y / GpsTrack.HGranularity)));
                    track.Y.Add(cy - y);
                    y = cy;

                    float elevationInMeters = (float)(trackPoint.Elevation.HasValue
                                                          ? trackPoint.Elevation.Value
                                                          : GpsTrack.MagicElevation);

                    int cEle = (int)Math.Round(elevationInMeters / GpsTrack.VGranularity);
                    track.Elevation.Add(cEle - elevation);
                    elevation = cEle;

                    long cTime = trackPoint.Time.HasValue ? trackPoint.Time.Value.Ticks / TimeSpan.TicksPerMillisecond : 0;
                    track.Time.Add(cTime - time);
                    time = cTime;
                }
            }

            long len;
            using (Stream outputStream = File.Open("Sample6.dat", FileMode.Create))
            {
                Serializer.Serialize(outputStream, track);
                outputStream.Flush();
                len = outputStream.Position;
            }

            string protoDef = Serializer.GetProto<GpsTrack>();
            File.WriteAllText("Sample6.proto", protoDef);

            Assert.AreEqual(419, len);

            using (Stream inputStream = File.Open("Sample6.dat", FileMode.Open))
            {
                GpsTrack track2 = Serializer.Deserialize<GpsTrack>(inputStream);

                Assert.AreApproximatelyEqual(15.604103, track2.GetPointX(10), 0.001);
                Assert.AreApproximatelyEqual(45.896726, track2.GetPointY(10), 0.001);
                Assert.AreApproximatelyEqual(128.47f, track2.GetPointElevation(10).Value, 0.5);
                Assert.LessThanOrEqualTo(new DateTime(2010, 06, 17, 8, 32, 44) - track2.GetPointTime(10).Value, new TimeSpan(0, 0, 1));

                Assert.AreEqual(170.8f, track2.MaxElevation);
                
                Assert.AreElementsEqual(track.X, track2.X);
                Assert.AreElementsEqual(track.Y, track2.Y);
                Assert.AreElementsEqual(track.Elevation, track2.Elevation);
                Assert.AreElementsEqual(track.Time, track2.Time);
            }
        }
    }
}