using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using Brejc.Common.FileSystem;
using Brejc.OsmLibrary;
using Brejc.OsmLibrary.Pbf;
using Brejc.Rdbms;

namespace SpatialitePlaying
{
    class Program
    {
        static void Main (string[] args)
        {
            WindowsFileSystem fileSystem = new WindowsFileSystem();
            fileSystem.DeleteFile("test.sqlite", false);
            string connString = SqliteHelper.ConstructConnectionString("test.sqlite", false);

            DbProviderFactory dbProviderFactory = SqliteHelper.SqliteProviderFactory;
            using (IDbConnection dbConnection = dbProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connString;
                dbConnection.Open();

                using (IDbCommand command = dbConnection.CreateCommand())
                {
                    command.CommandText = @"SELECT load_extension('libspatialite-4.dll');";
                    command.ExecuteNonQuery();
                }

                using (IDbCommand command = dbConnection.CreateCommand())
                {
                    command.CommandText = @"CREATE TABLE areas (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT)";
                    command.ExecuteNonQuery();
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

                dbConnection.ExecuteCommand("SELECT InitSpatialMetadata();");
                //dbConnection.enable_load_extension (False);

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"SELECT AddGeometryColumn('areas','geom',4326, 'POLYGON', 2)";
                    object value = command.ExecuteScalar();
                }
            }

            using (IDbConnection dbConnection = dbProviderFactory.CreateConnection())
            {
                dbConnection.ConnectionString = connString;
                dbConnection.Open ();

                using (IDbCommand command = dbConnection.CreateCommand ())
                {
                    command.CommandText = @"SELECT load_extension('libspatialite-4.dll');";
                    command.ExecuteNonQuery ();
                }

                using (OsmPbfReader osmReader = new OsmPbfReader ())
                {
                    InMemoryOsmDatabase osmDb = new InMemoryOsmDatabase ();
                    osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, osmDb);

                    foreach (OsmWay way in osmDb.Ways)
                    {
                        if (!way.IsClosed)
                            continue;

                        using (IDbCommand command = dbConnection.CreateCommand ())
                        {
                            StringBuilder geomBuilder = new StringBuilder();
                            geomBuilder.Append ("POLYGON((");

                            string comma = null;
                            foreach (long nodeId in way.Nodes)
                            {
                                OsmNode node = osmDb.GetNode(nodeId);
                                geomBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1} {2}", comma, node.X, node.Y);
                                comma = ",";
                            }

                            geomBuilder.Append("))");

                            command.CommandText = @"INSERT INTO areas (geom) VALUES (GeomFromText(@p1,4326))";
                            command.AddParameter("@p1", geomBuilder.ToString());
                            command.ExecuteNonQuery ();
                        }
                    }
                }
            }
        }
    }
}
