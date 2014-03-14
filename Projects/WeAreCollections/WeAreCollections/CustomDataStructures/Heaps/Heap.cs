using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WeAreCollections.CustomDataStructures.Heaps
{
    // http://stackoverflow.com/questions/102398/priority-queue-in-net
    public abstract class Heap<T> : IHeap<T>
    {
        public int Count
        {
            get { return tail; }
        }

        public int Capacity
        {
            get { return capacity; }
        }

        public void Add (T item)
        {
            if (Count == Capacity)
                Grow ();

            heap[tail++] = item;
            BubbleUp (tail - 1);
        }

        public T PeekMin ()
        {
            if (Count == 0) 
                throw new InvalidOperationException ("Heap is empty");

            return heap[0];
        }

        public T RemoveMin ()
        {
            if (Count == 0) 
                throw new InvalidOperationException ("Heap is empty");

            T ret = heap[0];
            tail--;
            Swap (tail, 0);
            BubbleDown (0);
            return ret;
        }

        public IEnumerator<T> GetEnumerator ()
        {
            return heap.Take (Count).GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        protected Comparer<T> Comparer { get; private set; }
        protected abstract bool Dominates (T x, T y);

        protected Heap () : this (Comparer<T>.Default)
        {
        }

        protected Heap (Comparer<T> comparer) : this (Enumerable.Empty<T> (), comparer)
        {
        }

        protected Heap (IEnumerable<T> collection) : this (collection, Comparer<T>.Default)
        {
        }

        protected Heap (IEnumerable<T> collection, Comparer<T> comparer)
        {
            if (collection == null) 
                throw new ArgumentNullException ("collection");
            if (comparer == null) 
                throw new ArgumentNullException ("comparer");

            Comparer = comparer;

            foreach (var item in collection)
            {
                if (Count == Capacity)
                    Grow ();

                heap[tail++] = item;
            }

            for (int i = Parent (tail - 1); i >= 0; i--)
                BubbleDown (i);
        }

        private void BubbleUp (int i)
        {
            int parent = Parent(i);

            if (i == 0 || Dominates (heap[parent], heap[i]))
                return; //correct domination (or root)

            Swap (i, parent);
            BubbleUp (parent);
        }

        private void BubbleDown (int i)
        {
            int dominatingNode = Dominating (i);
            if (dominatingNode == i) 
                return;
            Swap (i, dominatingNode);
            BubbleDown (dominatingNode);
        }

        private int Dominating (int i)
        {
            int dominatingNode = i;
            dominatingNode = GetDominating (YoungChild (i), dominatingNode);
            dominatingNode = GetDominating (OldChild (i), dominatingNode);

            return dominatingNode;
        }

        private int GetDominating (int newNode, int dominatingNode)
        {
            if (newNode < tail && !Dominates (heap[dominatingNode], heap[newNode]))
                return newNode;

            return dominatingNode;
        }

        private void Swap (int i, int j)
        {
            T tmp = heap[i];
            heap[i] = heap[j];
            heap[j] = tmp;
        }

        private static int Parent (int i)
        {
            return ((i + 1) >> 1) - 1;
        }

        private static int YoungChild (int i)
        {
            return ((i + 1) << 1) - 1;
        }

        private static int OldChild (int i)
        {
            return YoungChild (i) + 1;
        }

        private void Grow ()
        {
            int newCapacity = capacity * GrowFactor + MinGrow;
            var newHeap = new T[newCapacity];
            Array.Copy (heap, newHeap, capacity);
            heap = newHeap;
            capacity = newCapacity;
        }

        private const int InitialCapacity = 0;
        private const int GrowFactor = 2;
        private const int MinGrow = 1;

        private int capacity = InitialCapacity;
        private T[] heap = new T[InitialCapacity];
        private int tail;
    }
}