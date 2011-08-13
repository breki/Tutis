using System;
using MbUnit.Framework;

namespace Clavis.Tests
{
    public class ClavisFileTests
    {
        [Test]
        public void HasStoreShouldReturnFalseWhenStoreDoesNotExist()
        {
            Assert.IsFalse(file.HasStore("store"));
        }

        [Test]
        public void HasStoreShouldReturnTrueWhenStoreExists()
        {
            file.OpenStore<ClavisKeyValueBinaryStore>("store", true);
            Assert.IsTrue(file.HasStore("store"));
        }

        [Test]
        public void GetStoreShouldNotFailIfAlreadyExists()
        {
            Assert.IsNotNull(file.OpenStore<ClavisKeyValueBinaryStore>("store", true));
            Assert.IsNotNull(file.OpenStore<ClavisKeyValueBinaryStore>("store", true));
        }

        [Test, ExpectedException(typeof(InvalidOperationException), "Clavis store 'store' does not exist.")]
        public void GetStoreShouldFailIfStoreDoesNotExists()
        {
            file.OpenStore<ClavisKeyValueBinaryStore>("store", false);
        }

        [SetUp]
        private void Setup()
        {
            root = new ClavisRoot("testdb", new TimeService());
            file = root.OpenFile(FileName, true);
        }

        [TearDown]
        private void Teardown()
        {
            root.DeleteFile(FileName);
        }

        private IClavisRoot root;
        private IClavisFile file;
        private const string FileName = "settings";
    }
}