namespace LruCaching
{
    public interface ILruCache<K,V>
    {
        void Add(K key, V value);
        V Get(K key);
        bool IsCached(K key);
    }
}