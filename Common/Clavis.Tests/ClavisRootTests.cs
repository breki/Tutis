using System.Collections.Generic;
using System.IO;
using MbUnit.Framework;

namespace Clavis.Tests
{
    public class ClavisRootTests
    {
        [Test]
        public void CreateANewFile()
        {
            IClavisFile file = root.OpenFile(FileName, true);
        }

        [Test]
        public void HasFileShouldReturnFalseIfThereIsNoFile()
        {
            Assert.IsFalse(root.HasFile(FileName));
        }

        [Test]
        public void HasFileShouldReturnTreIfThereIsFile()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            Assert.IsTrue(root.HasFile(FileName));
        }

        [Test]
        public void RecreatingFileShouldWork()
        {
            root.OpenFile(FileName, true);
            root.OpenFile(FileName, true);
        }

        [SetUp]
        private void Setup()
        {
            if (Directory.Exists(RootDir))
                Directory.Delete(RootDir, true);

            root = new ClavisRoot(RootDir, new TimeService());
        }

        [TearDown]
        private void Teardown()
        {
            root.DeleteFile(FileName);

            if (Directory.Exists(RootDir))
                Directory.Delete(RootDir, true);
        }

        private IClavisRoot root;
        private const string RootDir = "db";
        private const string FileName = "settings";
    }
}