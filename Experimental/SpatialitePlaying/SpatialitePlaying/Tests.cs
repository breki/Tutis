using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.OsmLibrary;
using Brejc.OsmLibrary.Pbf;
using Brejc.Rdbms;
using NUnit.Framework;

namespace SpatialitePlaying
{
    class Tests
    {
        [Test]
        public void AddOsmAreas ()
        {
            WindowsFileSystem fileSystem = new WindowsFileSystem ();
            fileSystem.DeleteFile ("test.sqlite", false);
            string connString = SqliteHelper.ConstructConnectionString ("test.sqlite", false);

            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();
            int addedRowsCount = 0;
            long elapsedSeconds;

            Console.WriteLine("Reading OSM data... (phase 1)");

            TestOsmDataStorage osmDb2 = new TestOsmDataStorage (fileSystem);

            using (OsmPbfReader osmReader = new OsmPbfReader())
            {
                //osmReader.Settings.SkipNodes = true;
                //osmReader.Settings.SkipRelations = true;
                osmReader.Settings.IgnoreCreatedByTags = true;
                osmReader.Settings.LoadExtendedData = false;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, osmDb);
                osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, osmDb2);
                //osmReader.Read (@"D:\brisi\austria-latest.osm.pbf", fileSystem, osmDb2);
            }

            Console.WriteLine ("Reading OSM data... (phase 2)");

            using (OsmPbfReader osmReader = new OsmPbfReader())
            {
                osmReader.Settings.SkipNodes = false;
                osmReader.Settings.SkipWays = true;
                osmReader.Settings.SkipRelations = true;
                osmReader.Settings.IgnoreCreatedByTags = true;
                osmReader.Settings.LoadExtendedData = false;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, osmDb);
                osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, osmDb2);
            }

            DbProviderFactory dbProviderFactory = SqliteHelper.SqliteProviderFactory;
            using (IDbConnection dbConnection = dbProviderFactory.CreateConnection ())
            {
                dbConnection.ConnectionString = connString;
                dbConnection.Open ();

                Console.WriteLine ("Loading spatialite...");

                dbConnection.ExecuteCommand("PRAGMA journal_mode=OFF");
                dbConnection.ExecuteCommand("PRAGMA count_changes=OFF");
                dbConnection.ExecuteCommand("PRAGMA cache_size=4000");

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"SELECT load_extension('libspatialite-4.dll');";
                    command.ExecuteNonQuery ();
                }

                Console.WriteLine ("Initializing spatial metadata...");

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"SELECT InitSpatialMetadata();";
                    long value = (long)command.ExecuteScalar ();
                    Assert.AreEqual(1, value);
                }

                Console.WriteLine ("Creating table...");

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"CREATE TABLE areas (id INTEGER, geom BLOB NOT NULL)";
                    command.ExecuteNonQuery ();
                }

                stopwatch.Start();

                Console.WriteLine ("Started filling data...");

                int waysCount = 0;
                while (waysCount < osmDb2.UsedWays.Count)
                {
                    DBBulkProcessor.BulkInsert(
                        SqliteHelper.SqliteProviderFactory,
                        dbConnection,
                        "SELECT id, geom FROM areas",
                        "INSERT INTO areas (id, geom) VALUES (@param1, @param2)",
                        dt =>
                            {
                                for (int i = 0; i + waysCount < osmDb2.UsedWays.Count; waysCount++, i++)
                                {
                                    OsmWay way = osmDb2.UsedWays[waysCount];
                                    if (!way.IsClosed)
                                        continue;

                                    DataRow row = dt.NewRow();
                                    row[0] = 1;
                                    row[1] = WkbPolygonFromNodes(osmDb2, way);
                                    dt.Rows.Add(row);

                                    addedRowsCount++;

                                    if (addedRowsCount%1000 == 0)
                                    {
                                        elapsedSeconds = stopwatch.ElapsedMilliseconds/1000;
                                        Console.Out.WriteLine(
                                            "Added {0} rows in {1} s ({2} rows/s)",
                                            addedRowsCount,
                                            elapsedSeconds,
                                            ((double)addedRowsCount)/elapsedSeconds);
                                    }

                                    if (i >= 10000)
                                        break;

                                    //if (addedRowsCount >= 10000)
                                    //    break;
                                }
                            });
                }

                elapsedSeconds = stopwatch.ElapsedMilliseconds/1000;
                Console.Out.WriteLine("----------------------");
                Console.Out.WriteLine(
                    "Added {0} rows in {1} s ({2} rows/s)", 
                    addedRowsCount, 
                    elapsedSeconds, 
                    ((double)addedRowsCount) / elapsedSeconds);

                Console.WriteLine ("Recovering geometries...");

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"SELECT RecoverGeometryColumn('areas','geom',4326,'POLYGON', 2)";
                    long value = (long)command.ExecuteScalar ();
                    Assert.AreEqual (1, value, "RecoverGeometryColumn failed");
                }

                Console.WriteLine ("Creating spatial index...");

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"SELECT CreateSpatialIndex('areas','geom')";
                    long value = (long)command.ExecuteScalar ();
                    Assert.AreEqual (1, value, "CreateSpatialIndex failed");
                }
            }
        }

        [Test]
        public void AnalyzeOsmFile()
        {
            OsmFileAnalyzer analyzer = new OsmFileAnalyzer();

            WindowsFileSystem fileSystem = new WindowsFileSystem ();
            using (OsmPbfReader osmReader = new OsmPbfReader ())
            {
                osmReader.Settings.IgnoreCreatedByTags = true;
                osmReader.Settings.LoadExtendedData = false;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, osmDb);
                //osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, analyzer);
                osmReader.Read (@"D:\brisi\austria-latest.osm.pbf", fileSystem, analyzer);
            }
        }

        [Test]
        public void StoreNodesInBinaryFile()
        {
            OsmNodesBinaryRecorder recorder = new OsmNodesBinaryRecorder(new WindowsFileSystem());

            WindowsFileSystem fileSystem = new WindowsFileSystem ();
            using (OsmPbfReader osmReader = new OsmPbfReader ())
            {
                osmReader.Settings.SkipRelations = true;
                osmReader.Settings.SkipWays = true;
                osmReader.Settings.IgnoreCreatedByTags = true;
                osmReader.Settings.LoadExtendedData = false;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, osmDb);
                osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, recorder);
                //osmReader.Read (@"D:\brisi\austria-latest.osm.pbf", fileSystem, recorder);
            }
        }

        // http://www.gaia-gis.it/gaia-sins/BLOB-Geometry.html
        private static byte[] WkbPolygonFromNodes(TestOsmDataStorage osmDb, IOsmNodesList nodes)
        {
            using (MemoryStream stream = new MemoryStream ())
            using (BinaryWriter writer = new BinaryWriter (stream))
            {
                writer.Write((byte)0);
                writer.Write((byte)1); // little-endian
                writer.Write(4326); // SRID

                List<IPointD2> nodesList = new List<IPointD2> ();
                Bounds2 mbr = new Bounds2();

                foreach (long nodeId in nodes.Nodes)
                {
                    IPointD2 nodeData = osmDb.GetNodeData(nodeId);

                    if (nodeData != null)
                    {
                        nodesList.Add(nodeData);
                        mbr.ExtendToCover(nodeData.X, nodeData.Y);
                    }
                }

                writer.Write(mbr.MinX);
                writer.Write(mbr.MinY);
                writer.Write(mbr.MaxX);
                writer.Write(mbr.MaxY);
                writer.Write ((byte)0x7C);

                writer.Write(3);
                writer.Write(1); // number of rings

                writer.Write(nodesList.Count);
                foreach (PointD2 node in nodesList)
                {
                    writer.Write(node.X);
                    writer.Write(node.Y);
                }

                writer.Write ((byte)0xFE);

                writer.Flush ();
                return stream.ToArray ();
            }
        }
    }
}
