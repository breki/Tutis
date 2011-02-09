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

            long len;
            using (Stream outputStream = File.Open("Sample1.dat", FileMode.Create))
            {
                Serializer.Serialize(outputStream, track);
                outputStream.Flush();
                len = outputStream.Position;
            }

            string protoDef = Serializer.GetProto<GpsTrack>();
            File.WriteAllText("Sample1.proto", protoDef);

            Assert.AreEqual(2240, len);

            using (Stream inputStream = File.Open("Sample1.dat", FileMode.Open))
            {
                GpsTrack track2 = Serializer.Deserialize<GpsTrack>(inputStream);
                Assert.AreEqual(170.77f, track2.MaxElevation);

                Assert.AreElementsEqual(track.Points, track2.Points);
            }
        }
    }
}