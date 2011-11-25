using System;
using System.Collections;
using System.Reflection;
using Brejc.Common.FileSystem;
using Brejc.OsmLibrary;
using log4net;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace NoSqlPlaying
{
    public class MongoDBTests
    {
        [Test]
        public void FillNodes()
        {
            MongoDatabase osmMongoDb = ConnectToMongo();

            InMemoryOsmDatabase osmDb = new InMemoryOsmDatabase();
            OsmXmlReader reader = new OsmXmlReader();
            reader.Read(
                @"D:\brisi\nordrhein-westfalen.osm.bz2",
                //@"..\..\..\..\samples\OSM\MariborPohorje.osm.bz2",
                new WindowsFileSystem(),
                osmDb);

            osmMongoDb.DropCollection("nodes");
            MongoCollection<Node> nodes = osmMongoDb.GetCollection<Node>("nodes");
            nodes.EnsureIndex(IndexKeys.GeoSpatial("Loc"));

            foreach (OsmNode osmNode in osmDb.Nodes)
            {
                Node node = new Node();
                node.OsmId = osmNode.ObjectId;
                node.Loc = new Point();
                node.Loc.X = osmNode.X;
                node.Loc.Y = osmNode.Y;

                if (osmNode.Tags != null)
                {
                    node.Tags = new Hashtable();
                    foreach (Tag tag in osmNode.Tags)
                        node.Tags.Add(tag.Key, tag.Value);
                }

                nodes.Insert(node);
            }
        }

        [Test]
        public void ReadNodes()
        {
            MongoDatabase osmMongoDb = ConnectToMongo();
            MongoCollection<Node> nodes = osmMongoDb.GetCollection<Node>("nodes");

            int counter = 0;
            foreach (Node node in nodes.FindAll())
                counter++;
                //log.DebugFormat("{0}, {1}", node.X, node.Y);

            Assert.AreEqual(144493, counter);

            IMongoQuery query = Query.WithinRectangle("Loc", 15.6, 46.4, 15.7, 46.7);

            counter = 0;
            foreach (Node node in nodes.Find(query))
                counter++;
            Assert.AreEqual(27727, counter);

            //query = Query.WithinRectangle("Loc", 15.6, 46.4, 15.7, 46.7);
            //query = Query.Exists("Tags.amenity", true);
            query = Query.And(
                Query.WithinRectangle("Loc", 15.6, 46.4, 15.7, 46.7),
                Query.Exists("Tags.amenity", true));

            counter = 0;
            foreach (Node node in nodes.Find(query))
                counter++;
            Assert.AreEqual(190, counter);
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private static MongoDatabase ConnectToMongo()
        {
            MongoConnectionStringBuilder connStringBuilder = new MongoConnectionStringBuilder();
            connStringBuilder.Server = new MongoServerAddress("localhost", 27017);

            MongoServer server = MongoServer.Create(connStringBuilder);
            return server.GetDatabase("osm");
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}