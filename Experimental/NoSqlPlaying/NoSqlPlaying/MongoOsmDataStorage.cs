using System;
using Brejc.OsmLibrary;
using MongoDB.Driver;

namespace NoSqlPlaying
{
    public class MongoOsmDataStorage : IOsmDataStorage
    {
        public MongoOsmDataStorage(MongoDatabase osmMongoDb)
        {
            this.osmMongoDb = osmMongoDb;
        }

        public IOsmDataBulkInsertSession StartBulkInsertSession()
        {
            return new MongoOsmDataBulkInsertSession(osmMongoDb);
        }

        private readonly MongoDatabase osmMongoDb;
    }
}