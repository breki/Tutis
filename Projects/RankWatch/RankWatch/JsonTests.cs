using System.IO;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;

namespace RankWatch
{
    public class JsonTests
    {
        [Test]
        public void Test()
        {
            RanksDb db = new RanksDb();

            RankHistory history = new RankHistory("xxx");
            history.RanksByDays.Add(0, 1);
            history.RanksByDays.Add(1, 10);
            history.RanksByDays.Add(3, 5);
            db.Keywords.Add(history.SearchKeyword, history);

            history = new RankHistory("yyy");
            history.RanksByDays.Add(0, 1);
            history.RanksByDays.Add(1, 10);
            history.RanksByDays.Add(3, 5);
            db.Keywords.Add(history.SearchKeyword, history);

            JsonSerializer serializer = new JsonSerializer();

            StringWriter sw = new StringWriter();
            serializer.Serialize (sw, db);
            Assert.AreEqual("22", sw.ToString());
        }        
    }
}