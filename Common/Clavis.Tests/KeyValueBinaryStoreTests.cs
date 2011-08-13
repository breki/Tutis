using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Clavis.Tests
{
    public class KeyValueBinaryStoreTests
    {
        [Test, ExpectedException(typeof(KeyNotFoundException), "Key 'key1' does not exist in store 'simple' in file 'settings.s3db'.")]
        public void GetShouldFailIfKeyDoesNotExist()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            ClavisKeyValueBinaryStore keyValueBinaryStore = file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            using (IClavisSession session = file.OpenSession())
            {
                keyValueBinaryStore.Get(session, "key1");
            }
        }

        [Test]
        public void AddingNewKey()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            IClavisKeyValueBinaryStore keyValueBinaryStore = file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            using (IClavisSession session = file.OpenSession())
            {
                using (IClavisTransaction transaction = session.BeginTransaction())
                {
                    keyValueBinaryStore.Add(session, "key1", "value1");
                    transaction.Commit();
                }
            }

            AssertKeyValueExists(file, "key1", "value1");
        }

        [Test, ExpectedException(typeof(InvalidOperationException), "Key 'key1' already exists in store 'simple' in file 'settings.s3db'.")]
        public void AddingNewKeyShouldFailIfKeyAlreadyExists()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            IClavisKeyValueBinaryStore keyValueBinaryStore = file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            using (IClavisSession session = file.OpenSession())
            {
                using (IClavisTransaction transaction = session.BeginTransaction())
                {
                    keyValueBinaryStore.Add(session, "key1", "value1");
                    keyValueBinaryStore.Add(session, "key1", "value1");
                    transaction.Commit();
                }
            }
        }

        [Test]
        public void AddNewObjectWithTags()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            IClavisKeyValueBinaryStore keyValueBinaryStore = file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            using (IClavisSession session = file.OpenSession())
            {
                using (IClavisTransaction transaction = session.BeginTransaction())
                {
                    keyValueBinaryStore.Add(session, "key1", "value1", "mytag1", "myvalue1", "mytag2", "myvalue2");
                    transaction.Commit();
                }
            }

            AssertKeyValueExists(file, "key1", "value1");
            Assert.AreEqual("value1", FindFirst(file, "mytag1", "myvalue1"));
            Assert.AreEqual("value1", FindFirst(file, "mytag2", "myvalue2"));
        }

        [Test]
        public void SetObjectOverridingExistingOne()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            SetKeyValue(file, "key1", "value1");
            SetKeyValue(file, "key1", "value2");
            AssertKeyValueExists(file, "key1", "value2");
        }

        [Test]
        public void SetObjectWhichIsNew()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            SetKeyValue(file, "key1", "value1");
            AssertKeyValueExists(file, "key1", "value1");
        }

        [Test]
        public void SetObjectWithTags()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            SetKeyValue(file, "key1", "value1");
            SetKeyValue(file, "key1", "value1", "mytag1", "myvalue1");
            Assert.AreEqual("value1", FindFirst(file, "mytag1", "myvalue1"));
        }

        [Test]
        public void FindFirstFindsFirst()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            SetKeyValue(file, "key1", "value1", "mytag1", "myvalue1");
            Assert.AreEqual("value1", FindFirst(file, "mytag1", "myvalue1"));
        }

        [Test]
        public void FindFirstFindsNothing()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            SetKeyValue(file, "key1", "value1", "mytag1", "myvalue1");
            Assert.IsNull(FindFirst(file, "mytag1", "myvalue2"));
        }

        [Test]
        public void DestroyStore()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);
            file.DeleteStore<ClavisKeyValueBinaryStore>(StoreName);
            Assert.IsFalse(file.HasStore(StoreName));
        }

        [Test]
        public void DestroyStoreWhenStoreDoesNotExist()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            file.DeleteStore<ClavisKeyValueBinaryStore>(StoreName);
        }

        [Test]
        public void MultithreadedReadWrite()
        {
            IClavisFile file = root.OpenFile(FileName, true);
            IClavisKeyValueBinaryStore keyValueBinaryStore = file.OpenStore<ClavisKeyValueBinaryStore>(StoreName, true);

            using (MultithreadedTestRunner runner = new MultithreadedTestRunner())
            {
                runner.AddThread(data => ThreadAction(file, data), 0);
                runner.AddThread(data => ThreadAction(file, data), 1);
                runner.AddThread(data => ThreadAction(file, data), 2);
                runner.AddThread(data => ThreadAction(file, data), 3);
                runner.Run(TimeSpan.FromSeconds(10));
                runner.AssertNoFailures();
            }

            using (IClavisSession session = file.OpenSession())
            {
                IDictionary<string, object> rows = keyValueBinaryStore.List(session);
                Assert.AreEqual(40, rows.Count);
            }
        }

        private static bool ThreadAction(IClavisFile file, object data)
        {
            IClavisKeyValueBinaryStore keyValueBinaryStore = new ClavisKeyValueBinaryStore();
            keyValueBinaryStore.Initialize(file, StoreName);

            for (int i = 0; i < 10; i++)
            {
                using (IClavisSession session = file.OpenSession())
                {
                    using (IClavisTransaction transaction = session.BeginTransaction())
                    {
                        int index = (i*4) + (int)data;
                        keyValueBinaryStore.Set(session, index, index);
                        transaction.Commit();
                    }
                }

                using (IClavisSession session = file.OpenSession())
                {
                    int index = (i * 4) + (int)data;
                    if ((int)keyValueBinaryStore.Get(session, index) != index)
                        throw new InvalidOperationException("BUG");
                }
            }

            return true;
        }

        [SetUp]
        private void Setup()
        {
            timeService = MockRepository.GenerateStub<ITimeService>();
            root = new ClavisRoot(string.Empty, timeService);
        }

        [TearDown]
        private void Teardown()
        {
            root.DeleteFile(FileName);
        }

        private static void SetKeyValue(IClavisFile file, string key, string value, params object[] tagsKeysValues)
        {
            IClavisKeyValueBinaryStore keyValueBinaryStore = new ClavisKeyValueBinaryStore();
            keyValueBinaryStore.Initialize(file, StoreName);

            using (IClavisSession session = file.OpenSession())
            {
                using (IClavisTransaction transaction = session.BeginTransaction())
                {
                    keyValueBinaryStore.Set(session, key, value, tagsKeysValues);
                    transaction.Commit();
                }
            }
        }

        private static object FindFirst(IClavisFile file, string key, string value)
        {
            IClavisKeyValueBinaryStore keyValueBinaryStore = new ClavisKeyValueBinaryStore();
            keyValueBinaryStore.Initialize(file, StoreName);

            using (IClavisSession session = file.OpenSession())
            {
                return keyValueBinaryStore.FindFirst(session, key, value);
            }
        }

        private static void AssertKeyValueExists(IClavisFile file, string key, string value)
        {
            IClavisKeyValueBinaryStore keyValueBinaryStore = new ClavisKeyValueBinaryStore();
            keyValueBinaryStore.Initialize(file, StoreName);

            using (IClavisSession session = file.OpenSession())
            {
                Assert.AreEqual(value, keyValueBinaryStore.Get(session, key));
            }
        }

        private IClavisRoot root;
        private ITimeService timeService;
        private const string FileName = "settings";
        private const string StoreName = "simple";
    }
}