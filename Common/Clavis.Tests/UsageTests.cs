using MbUnit.Framework;

namespace Clavis.Tests
{
    public class UsageTests
    {
        [Test]
        public void Test()
        {
            IClavisRoot root = new ClavisRoot("test", new TimeService());
            IClavisFile file = root.OpenFile("file", true);
            IClavisKeyValueBinaryStore store = file.OpenStore<ClavisKeyValueBinaryStore>("store", true);
            using (IClavisSession session = store.OpenSession())
            using (IClavisTransaction trans = session.BeginTransaction())
            {
                store.Set(session, "key", "value");
                trans.Commit();
            }
        }
    }
}