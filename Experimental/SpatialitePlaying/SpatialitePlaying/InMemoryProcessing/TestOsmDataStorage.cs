using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.OsmLibrary;
using Brejc.Rdbms;

namespace SpatialitePlaying.InMemoryProcessing
{
    public class TestOsmDataStorage : IOsmDataStorage
    {
        public TestOsmDataStorage(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            DbProviderFactory dbProviderFactory = SqliteHelper.SqliteProviderFactory;
            dbConnection = dbProviderFactory.CreateConnection();

            fileSystem.DeleteFile ("osm-temp-import.sqlite", false);
            string connString = SqliteHelper.ConstructConnectionString ("osm-temp-import.sqlite", false);
            dbConnection.ConnectionString = connString;
            dbConnection.Open();

            dbConnection.ExecuteCommand ("PRAGMA journal_mode=OFF");
            dbConnection.ExecuteCommand ("PRAGMA count_changes=OFF");
            dbConnection.ExecuteCommand ("PRAGMA cache_size=4000");
            dbConnection.ExecuteCommand ("PRAGMA temp_store=MEMORY");

            using (IDbCommand command = dbConnection.CreateCommand ())
            {
                command.CommandText = @"CREATE TABLE nodes (node_id INTEGER PRIMARY KEY, x REAL NULL, y REAL NULL)";
                command.ExecuteNonQuery ();
            }
        }

        public string AttributionText { get; set; }

        public IDbConnection DbConnection
        {
            get { return dbConnection; }
        }

        public ReadingPhase ReadingPhase
        {
            get { return readingPhase; }
            set { readingPhase = value; }
        }

        public IOsmDataBulkInsertSession StartBulkInsertSession(bool threadSafe)
        {
            return new TestOsmDataBulkInsertSession(this, readingPhase);
        }

        public void EndBulkInsertSession()
        {
            readingPhase = readingPhase + 1;
        }

        public List<OsmWay> UsedWays
        {
            get { return usedWays; }
        }

        public void AddNodeData (OsmNode node)
        {
            nodesData[node.ObjectId] = new PointD2(node.X, node.Y);
        }

        public IPointD2 GetNodeData(long nodeId)
        {
            PointD2 value;
            if (!nodesData.TryGetValue(nodeId, out value))
                return null;

            return value;
        }

        private readonly IFileSystem fileSystem;
        private ReadingPhase readingPhase = ReadingPhase.ReadWays;
        private Dictionary<long, PointD2> nodesData = new Dictionary<long, PointD2> ();
        private List<OsmWay> usedWays = new List<OsmWay> ();
        private IDbConnection dbConnection;
    }
}