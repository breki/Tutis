using System.Collections.Generic;

namespace WeAreCollections.CustomDataStructures.Heaps
{
    public interface IHeap<T> : IEnumerable<T>
    {
        int Count { get; }

        void Add (T item);
        T PeekMin ();
        T RemoveMin ();
    }
}