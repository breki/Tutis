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

            Console.WriteLine("Reading OSM data...");

            InMemoryOsmDatabase osmDb = new InMemoryOsmDatabase ();
            using (OsmPbfReader osmReader = new OsmPbfReader())
            {
                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, osmDb);
                osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, osmDb);
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

                //using (IDbCommand command = dbConnection.CreateCommand ())
                //{
                //    command.CommandText = @"SELECT AddGeometryColumn('areas','geom',-1,'POLYGON', 2)";
                //    long value = (long)command.ExecuteScalar ();
                //    Assert.AreEqual (1, value);
                //}

                stopwatch.Start();

                //using (IDbCommand command = dbConnection.CreateCommand ())
                //{
                //    command.CommandText = @"INSERT INTO areas (id, geom) VALUES (@p1, PolygonFromText(@p2, 4326))";
                //    command.AddParameter("@p1", 1);
                //    command.AddParameter ("@p2", "POLYGON ((30 10, 10 20, 20 40, 40 40, 30 10))");
                //    command.ExecuteNonQuery ();
                //}


                Console.WriteLine ("Started filling data...");

                DBBulkProcessor.BulkInsert (
                    SqliteHelper.SqliteProviderFactory,
                    dbConnection,
                    "SELECT id, geom FROM areas",
                    "INSERT INTO areas (id, geom) VALUES (@param1, @param2)",
                    dt =>
                    {
                        foreach (OsmWay way in osmDb.Ways)
                        {
                            if (!way.IsClosed)
                                continue;

                            DataRow row = dt.NewRow ();
                            int i = 0;
                            row[i++] = 1;
                            row[i++] = WkbPolygonFromNodes (osmDb, way);
                            dt.Rows.Add (row);

                            addedRowsCount++;

                            if (addedRowsCount % 1000 == 0)
                            {
                                elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000;
                                Console.Out.WriteLine (
                                    "Added {0} rows in {1} s ({2} rows/s)",
                                    addedRowsCount,
                                    elapsedSeconds,
                                    ((double)addedRowsCount) / elapsedSeconds);
                            }

                            //if (addedRowsCount >= 10000)
                            //    break;
                        }
                    });


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

        // http://www.gaia-gis.it/gaia-sins/BLOB-Geometry.html
        private static byte[] WkbPolygonFromNodes(IInMemoryOsmDataSource osmDb, IOsmNodesList nodes)
        {
            using (MemoryStream stream = new MemoryStream ())
            using (BinaryWriter writer = new BinaryWriter (stream))
            {
                writer.Write((byte)0);
                writer.Write((byte)1); // little-endian
                writer.Write(4326); // SRID

                List<OsmNode> nodesList = new List<OsmNode>();
                Bounds2 mbr = new Bounds2();

                foreach (long nodeId in nodes.Nodes)
                {
                    OsmNode node = osmDb.GetNode(nodeId);
                    nodesList.Add(node);
                    mbr.ExtendToCover(node.X, node.Y);
                }

                writer.Write(mbr.MinX);
                writer.Write(mbr.MinY);
                writer.Write(mbr.MaxX);
                writer.Write(mbr.MaxY);
                writer.Write ((byte)0x7C);

                writer.Write(3);
                writer.Write(1); // number of rings

                writer.Write(nodesList.Count);
                foreach (OsmNode node in nodesList)
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
