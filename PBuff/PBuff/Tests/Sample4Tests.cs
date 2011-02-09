using System;
using System.Collections.Generic;
using System.IO;
using Brejc.GpsLibrary.Gpx;
using MbUnit.Framework;
using PBuff.Sample4;
using ProtoBuf;

namespace PBuff.Tests
{
    public class Sample4Tests
    {
        [Test]
        public void Sample4()
        {
            GpxFile gpxFile = GpxFile.Load(@"..\..\..\GPS\2010.06.17.gpx");

            GpsTrack track = new GpsTrack();
            track.X = new List<long>();
            track.Y = new List<long>();
            track.Elevation = new List<int>();
            track.Time = new List<ulong>();

            foreach (ITrackSegment segment in gpxFile.Tracks[0].Segments)
            {
                foreach (ITrackPoint trackPoint in segment.Points)
                {
                    track.X.Add(((long) (Math.Round(trackPoint.X/GpsTrack.Nanodegree*GpsTrack.HGranularity))));
                    track.Y.Add(((long) (Math.Round(trackPoint.Y/GpsTrack.Nanodegree*GpsTrack.HGranularity))));

                    float elevation = (float) (trackPoint.Elevation.HasValue
                                                   ? trackPoint.Elevation.Value
                                                   : GpsTrack.MagicElevation);

                    track.Elevation.Add((int)Math.Round(elevation / GpsTrack.VGranularity));
                    ulong time = trackPoint.Time.HasValue ? (ulong) trackPoint.Time.Value.Ticks / TimeSpan.TicksPerMillisecond : 0;
                    track.Time.Add(time);
                }
            }

            long len;
            using (Stream outputStream = File.Open("Sample4.dat", FileMode.Create))
            {
                Serializer.Serialize(outputStream, track);
                outputStream.Flush();
                len = outputStream.Position;
            }

            string protoDef = Serializer.GetProto<GpsTrack>();
            File.WriteAllText("Sample4.proto", protoDef);

            Assert.AreEqual(1420, len);

            using (Stream inputStream = File.Open("Sample4.dat", FileMode.Open))
            {
                GpsTrack track2 = Serializer.Deserialize<GpsTrack>(inputStream);
                Assert.AreEqual(170.8f, track2.MaxElevation);
                Assert.AreElementsEqual(track.X, track2.X);
                Assert.AreElementsEqual(track.Y, track2.Y);
                Assert.AreElementsEqual(track.Elevation, track2.Elevation);
                Assert.AreElementsEqual(track.Time, track2.Time);
            }
        }
    }
}