using System.Collections.Generic;

namespace WeAreCollections.CustomDataStructures.Heaps
{
    public class MaxHeap<T> : Heap<T>
    {
        public MaxHeap ()
            : this (Comparer<T>.Default)
        {
        }

        public MaxHeap (Comparer<T> comparer)
            : base (comparer)
        {
        }

        public MaxHeap (IEnumerable<T> collection, Comparer<T> comparer)
            : base (collection, comparer)
        {
        }

        public MaxHeap (IEnumerable<T> collection)
            : base (collection)
        {
        }

        protected override bool Dominates (T x, T y)
        {
            return Comparer.Compare (x, y) >= 0;
        }
    }
}