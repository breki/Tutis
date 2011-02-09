using System;
using System.Collections.Generic;
using System.IO;
using Brejc.GpsLibrary.Gpx;
using MbUnit.Framework;
using PBuff.Sample3;
using ProtoBuf;

namespace PBuff.Tests
{
    public class Sample3Tests
    {
        [Test]
        public void Sample3()
        {
            GpxFile gpxFile = GpxFile.Load(@"..\..\..\GPS\2010.06.17.gpx");

            GpsTrack track = new GpsTrack();
            track.X = new List<double>();
            track.Y = new List<double>();
            track.Elevation = new List<float>();
            track.Time = new List<ulong>();

            foreach (ITrackSegment segment in gpxFile.Tracks[0].Segments)
            {
                foreach (ITrackPoint trackPoint in segment.Points)
                {
                    track.X.Add(trackPoint.X);
                    track.Y.Add(trackPoint.Y);
                    track.Elevation.Add((float)
                        (trackPoint.Elevation.HasValue ? trackPoint.Elevation.Value : GpsTrack.MagicElevation));
                    track.Time.Add(trackPoint.Time.HasValue ? (ulong)trackPoint.Time.Value.Ticks : 0);
                }
            }

            long len;
            using (Stream outputStream = File.Open("Sample3.dat", FileMode.Create))
            {
                Serializer.Serialize(outputStream, track);
                outputStream.Flush();
                len = outputStream.Position;
            }

            string protoDef = Serializer.GetProto<GpsTrack>();
            File.WriteAllText("Sample3.proto", protoDef);

            Assert.AreEqual(1868, len);

            using (Stream inputStream = File.Open("Sample3.dat", FileMode.Open))
            {
                GpsTrack track2 = Serializer.Deserialize<GpsTrack>(inputStream);
                Assert.AreEqual(15.604103, track2.X[10]);
                Assert.AreEqual(45.896726, track2.Y[10]);
                Assert.AreEqual(128.47f, track2.Elevation[10]);
                Assert.AreEqual(new DateTime(2010, 06, 17, 8, 32, 44).Ticks, (long)track2.Time[10]);
                
                Assert.AreEqual(170.77f, track2.MaxElevation);

                Assert.AreElementsEqual(track.X, track2.X);
                Assert.AreElementsEqual(track.Y, track2.Y);
                Assert.AreElementsEqual(track.Elevation, track2.Elevation);
                Assert.AreElementsEqual(track.Time, track2.Time);
            }
        }
    }
}