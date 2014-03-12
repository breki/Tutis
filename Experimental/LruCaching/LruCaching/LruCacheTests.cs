using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace LruCaching
{
    public class LruCacheTests
    {
        [Test]
        public void ItemAddedShouldBePresent()
        {
            const int Key = 1;
            cache.Add(Key, 2);
            Assert.AreEqual(2, cache.Get(Key));
        }

        [Test]
        public void ItemAddedTwiceShouldBeUpdated()
        {
            const int Key = 1;
            cache.Add (Key, 3);
            cache.Add (Key, 4);
            Assert.AreEqual (4, cache.Get (Key));
        }

        [Test]
        public void ItemAddedShouldBeCached()
        {
            cache.Add (1, 2);
            Assert.IsTrue(cache.IsCached(1));
        }

        [Test]
        public void WhenAddingCacheSizeShouldBeControlled()
        {
            cache.Add(1, 2);
            cache.Add(2, 3);
            Assert.AreEqual(1, cache.Count);
        }

        [Test]
        public void ItemAddedLaterShouldBeCached()
        {
            cache.Add (1, 2);
            Assert.IsTrue(cache.IsCached(1));
        }

        [Test]
        public void FetchingItemNotInCache()
        {
            cache.Add (1, 2);
            cache.Add (2, 3);
            Assert.AreEqual (2, cache.Get(1));
        }

        [Test, Explicit]
        public void RandomScenario()
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            cache = new LruCache<int, int> (100, ReadItemFunc, WriteItemAction);
            Random rnd = new Random (1);

            const int TotalItemsCount = 10000;
            for (int i = 0; i < TotalItemsCount; i++)
                cache.Add(i, 123);

            for (int i = 0; i < 30000000; i++)
            {
                cache.Get(rnd.Next(TotalItemsCount));
                cache.Add(rnd.Next(TotalItemsCount), 123);
            }

            Debug.WriteLine("t={0}", s.ElapsedMilliseconds);
        }

        [SetUp]
        public void Setup()
        {
            persistentStorage = new Dictionary<int, int>();
            cache = new LruCache<int, int>(1, ReadItemFunc, WriteItemAction);
        }

        private int ReadItemFunc(int key)
        {
            return persistentStorage[key];
        }

        private void WriteItemAction(int key, int value)
        {
            persistentStorage[key] = value;
        }

        private LruCache<int, int> cache;
        private Dictionary<int, int> persistentStorage;
    }
}