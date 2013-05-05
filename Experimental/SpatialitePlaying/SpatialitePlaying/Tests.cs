using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Brejc.Common.FileSystem;
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

            DbProviderFactory dbProviderFactory = SqliteHelper.SqliteProviderFactory;
            using (IDbConnection dbConnection = dbProviderFactory.CreateConnection ())
            {
                dbConnection.ConnectionString = connString;
                dbConnection.Open ();

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"SELECT load_extension('libspatialite-4.dll');";
                    command.ExecuteNonQuery ();
                }

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"CREATE TABLE areas (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT)";
                    command.ExecuteNonQuery ();
                }
            }

            using (IDbConnection dbConnection = dbProviderFactory.CreateConnection ())
            {
                dbConnection.ConnectionString = connString;
                dbConnection.Open ();

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"SELECT load_extension('libspatialite-4.dll');";
                    command.ExecuteNonQuery ();
                }

                dbConnection.ExecuteCommand ("SELECT InitSpatialMetadata();");
                //dbConnection.enable_load_extension (False);

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"SELECT AddGeometryColumn('areas','geom',-1,'POLYGON', 2)";
                    object value = command.ExecuteScalar ();
                }
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int addedRowsCount = 0;
            long elapsedSeconds;

            using (IDbConnection dbConnection = dbProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connString;
                dbConnection.Open();

                using (IDbCommand command = dbConnection.CreateCommand())
                {
                    command.CommandText = @"SELECT load_extension('libspatialite-4.dll');";
                    command.ExecuteNonQuery();
                }

                using (OsmPbfReader osmReader = new OsmPbfReader())
                {
                    InMemoryOsmDatabase osmDb = new InMemoryOsmDatabase();
                    osmReader.Read(@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, osmDb);

                    DBBulkProcessor.BulkInsert(
                        SqliteHelper.SqliteProviderFactory,
                        dbConnection,
                        "SELECT geom FROM areas",
                        "INSERT INTO areas (geom) VALUES (@p1)",
                        dt =>
                            {
                                foreach (OsmWay way in osmDb.Ways)
                                {
                                    if (!way.IsClosed)
                                        continue;

                                    StringBuilder geomBuilder = new StringBuilder();
                                    geomBuilder.Append("POLYGON((");

                                    string comma = null;
                                    foreach (long nodeId in way.Nodes)
                                    {
                                        OsmNode node = osmDb.GetNode(nodeId);
                                        geomBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1} {2}", comma, node.X, node.Y);
                                        comma = ",";
                                    }

                                    geomBuilder.Append("))");

                                    DataRow row = dt.NewRow();
                                    int i = 0;
                                    row[i++] = geomBuilder.ToString();
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

                                    if (addedRowsCount >= 10000)
                                        break;
                                }
                            });

                }
            }

            elapsedSeconds = stopwatch.ElapsedMilliseconds/1000;
            Console.Out.WriteLine("----------------------");
            Console.Out.WriteLine(
                "Added {0} rows in {1} s ({2} rows/s)", 
                addedRowsCount, 
                elapsedSeconds, 
                ((double)addedRowsCount) / elapsedSeconds);
        }
    }
}
