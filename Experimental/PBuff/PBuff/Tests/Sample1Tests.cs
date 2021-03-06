using System;
using System.Collections.Generic;
using System.IO;
using Brejc.GpsLibrary.Gpx;
using MbUnit.Framework;
using PBuff.Sample1;
using ProtoBuf;

namespace PBuff.Tests
{
    public class Sample1Tests
    {
        [Test]
        public void Sample1()
        {
            GpxFile gpxFile = GpxFile.Load(@"..\..\..\GPS\2010.06.17.gpx");

            GpsTrack track = new GpsTrack();
            track.Points = new List<GpsPoint>();

            foreach (ITrackSegment segment in gpxFile.Tracks[0].Segments)
            {
                foreach (ITrackPoint trackPoint in segment.Points)
                {
                    GpsPoint point = new GpsPoint();
                    point.X = trackPoint.X;
                    point.Y = trackPoint.Y;
                    if (trackPoint.Elevation.HasValue)
                        point.Elevation = (float)trackPoint.Elevation.Value;
                    if (trackPoint.Time.HasValue)
                        point.Time = trackPoint.Time.Value;
                    track.Points.Add(point);
                }
            }

            Assert.AreEqual(15.604103, track.Points[10].X);
            Assert.AreEqual(45.896726, track.Points[10].Y);
            Assert.AreEqual(128.47f, track.Points[10].Elevation);
            Assert.AreEqual(new DateTime(2010, 06, 17, 8, 32, 44), track.Points[10].Time);

            long len;
            using (Stream outputStream = File.Open("Sample1.dat", FileMode.Create))
            {
                Serializer.Serialize(outputStream, track);
                outputStream.Flush();
                len = outputStream.Position;
            }

            string protoDef = Serializer.GetProto<GpsTrack>();
            File.WriteAllText("Sample1.proto", protoDef);

            Assert.AreEqual(2239, len);

            using (Stream inputStream = File.Open("Sample1.dat", FileMode.Open))
            {
                GpsTrack track2 = Serializer.Deserialize<GpsTrack>(inputStream);
                Assert.AreEqual(15.604103, track2.Points[10].X);
                Assert.AreEqual(45.896726, track2.Points[10].Y);
                Assert.AreEqual(128.47f, track2.Points[10].Elevation);
                Assert.AreEqual(new DateTime(2010, 06, 17, 8, 32, 44), track2.Points[10].Time);

                Assert.AreEqual(170.77f, track2.MaxElevation);

                Assert.AreElementsEqual(track.Points, track2.Points);
            }
        }
    }
}