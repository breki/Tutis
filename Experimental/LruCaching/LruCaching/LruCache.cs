using System;
using System.Collections.Generic;

namespace LruCaching
{
    public class LruCache<K, V> : ILruCache<K, V>
    {
        public LruCache(int cacheSize, Func<K, V> readItemFunc, Action<K, V> writeItemAction)
        {
            this.cacheSize = cacheSize;
            this.readItemFunc = readItemFunc;
            this.writeItemAction = writeItemAction;
        }

        public int Count
        {
            get { return cachedItems.Count; }
        }

        public void Add(K key, V value)
        {
            CachedItem<K, V> cachedItem;

            // is the item already cached?
            if (cachedItems.TryGetValue(key, out cachedItem))
            {
                // replace the old value with new one
                cachedItem.Value = value;
                // mark the item as last used
                cachedItemsByUsage.Remove(cachedItem);
                cachedItemsByUsage.AddLast(cachedItem);
                return;
            }

            SaveNewItemToCache(key, value);
        }

        public V Get(K key)
        {
            CachedItem<K, V> item;

            // is the item already cached?
            if (cachedItems.TryGetValue(key, out item))
                return item.Value;

            V value = readItemFunc(key);
            SaveNewItemToCache(key, value);

            return value;
        }

        public bool IsCached(K key)
        {
            return cachedItems.ContainsKey(key);
        }

        private void SaveNewItemToCache(K key, V value)
        {
            CachedItem<K, V> newItem = new CachedItem<K, V>(key, value, true);
            cachedItems.Add(key, newItem);
            cachedItemsByUsage.AddLast(newItem);

            if (Count > cacheSize)
                RemoveItemFromCache();
        }

        private void RemoveItemFromCache()
        {
            LinkedListNode<CachedItem<K, V>> itemNodeToRemoveFromCache = cachedItemsByUsage.First;

            if (itemNodeToRemoveFromCache == null)
                throw new InvalidOperationException("BUG: cachedItemsByUsage is empty");

            CachedItem<K, V> itemToRemoveFromCache = itemNodeToRemoveFromCache.Value;
            cachedItemsByUsage.Remove(itemNodeToRemoveFromCache);
            if (!cachedItems.Remove(itemToRemoveFromCache.Key))
                throw new InvalidOperationException("BUG: the item was not in cachedItems");

            if (itemToRemoveFromCache.IsDirty)
                writeItemAction(itemToRemoveFromCache.Key, itemToRemoveFromCache.Value);
        }

        private class CachedItem<TKey, TValue>
        {
            public CachedItem (TKey key, TValue value, bool isDirty)
            {
                this.key = key;
                this.value = value;
                this.isDirty = isDirty;
            }

            public TKey Key
            {
                get { return key; }
            }

            public TValue Value
            {
                get
                {
                    return value;
                }

                set
                {
                    this.value = value;
                    isDirty = true;
                }
            }

            public bool IsDirty
            {
                get { return isDirty; }
            }

            private readonly TKey key;
            private TValue value;
            private bool isDirty;
        }

        private readonly int cacheSize;
        private readonly Func<K, V> readItemFunc;
        private readonly Action<K, V> writeItemAction;
        private Dictionary<K, CachedItem<K, V>> cachedItems = new Dictionary<K, CachedItem<K, V>>();
        private LinkedList<CachedItem<K, V>> cachedItemsByUsage = new LinkedList<CachedItem<K, V>>();
    }
}