using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OMetaSharp
{
    /// <summary>
    /// An OMetaList is sort of like a LISP or Scheme cell. It can contain
    /// nested lists. Additionally, a list of a single element and the single
    /// element are treated as equal.
    /// </summary>
    /// <typeparam name="T">Fundamental cell item type.</typeparam>
    [DebuggerDisplay("{ToString()}")]
    public class OMetaList<T> : IList<OMetaList<T>>
    {
#line hidden
        private readonly OMetaList<T> m_Head;
        private readonly T m_HeadItem;
        private OMetaList<T> m_HeadItemAsList;

        private readonly OMetaList<T> m_Tail;

        // Nil list (sort of like an empty list, but a null empty list)
        // That is, you can construct an empty list, but all lists end in Nil
        public static readonly OMetaList<T> Nil = new OMetaList<T>(default(T));
        private static readonly string NilString = "nil<" + typeof(T).Name.ToString() + ">";
#line default

        [DebuggerStepThrough]
        public OMetaList(T head)
            : this(head, Nil)
        {
        }

        [DebuggerStepThrough]
        public OMetaList(T head, OMetaList<T> tail)
        {
            m_Head = null;
            m_HeadItem = head;
            m_Tail = tail;
        }

        [DebuggerStepThrough]
        public OMetaList(OMetaList<T> head, OMetaList<T> tail)
        {
            m_Head = head;
            m_Tail = tail;
        }

        [DebuggerStepThrough]
        public OMetaList(OMetaList<T> head)
            : this(head, Nil)
        {
        }

        [DebuggerStepThrough]
        protected OMetaList()
        {
        }
                
        public virtual OMetaList<T> Head
        {
            [DebuggerStepThrough]
            get
            {
                if (m_Head == null)
                {
                    if (m_HeadItemAsList == null)
                    {
                        m_HeadItemAsList = new OMetaList<T>(m_HeadItem);
                    }

                    return m_HeadItemAsList;
                }

                return m_Head;
            }
        }
                
        public virtual T HeadFirstItem
        {
            [DebuggerStepThrough]
            get
            {
                return (T)Head;
            }
        }
                
        public virtual OMetaList<T> Tail
        {
            [DebuggerStepThrough]
            get
            {
                return m_Tail;
            }
        }

        public virtual bool IsEmpty
        {
            [DebuggerStepThrough]
            get
            {
                if (this == OMetaList<T>.Nil)
                {
                    return true;
                }

                foreach (OMetaList<T> currentItem in this)
                {
                    return false;
                }

                return true;
            }
        }

        public virtual bool HasTail
        {
            [DebuggerStepThrough]
            get
            {
                return m_Tail != Nil;
            }
        }

        public virtual bool IsSingleItem
        {
            [DebuggerStepThrough]
            get
            {
                return m_Head == null;
            }
        }

        [DebuggerStepThrough]
        public static explicit operator T(OMetaList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (!list.IsSingleItem)
            {
                throw new ArgumentException("The list must have a single element", "list");
            }

            return list.SingleItem;
        }

        [DebuggerStepThrough]
        public static explicit operator OMetaList<T>(T item)
        {
            return new OMetaList<T>(item);
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            // We special case where count = 1 to just show the item.
            if (this == Nil)
            {
                return NilString;
            }

            // HACK
            OMetaList<HostExpression> asHostExprList = this as OMetaList<HostExpression>;
            if (asHostExprList != null)
            {
                string result;
                if (asHostExprList.TryHostExpressionCompressionOnString(out result))
                {
                    return result;
                }
            }
            
            var sb = new StringBuilder();
            sb.Append("[");

            OMetaList<T> currentList = this;
                        
            int count = 0;

            bool hasWrappingBrackets = true;

            while (currentList != Nil)
            {
                OMetaList<T> currentItem = currentList.Head;
                bool isLastItem = currentList.Tail == Nil;
                                
                if ((count == 0) && isLastItem && currentList.IsSingleItem)
                {
                    // get rid of opening bracket
                    hasWrappingBrackets = false;
                    sb.Length -= 1;
                }

                if (currentList.IsSingleItem)
                {
                    sb.Append(currentList.SingleItem);
                }
                else
                {                    
                    sb.Append(currentItem == null ? "(null)" : currentItem.ToString());
                }

                if (!isLastItem)
                {
                    sb.Append(", ");
                }

                currentList = currentList.Tail;
                count++;
            }

            if (hasWrappingBrackets)
            {
                sb.Append("]");
            }

            return sb.ToString();
        }

        #region IList<T> Members


        public virtual int IndexOf(OMetaList<T> item)
        {
            int ix = 0;
            foreach (OMetaList<T> currentItem in this)
            {
                if (object.Equals(currentItem, item))
                {
                    return ix;
                }
                ix++;
            }

            return -1;
        }

        public void Insert(int index, OMetaList<T> item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public virtual OMetaList<T> this[int index]
        {
            get
            {
                OMetaList<T> currentList = this;

                for (int ix = 0;
                    (ix < index) && (currentList != null);
                    currentList = currentList.Tail)
                {
                    ix++;
                }

                if (currentList == null)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return currentList.Head;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(OMetaList<T> item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(OMetaList<T> item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(OMetaList<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
                
        public virtual int Count
        {
            [DebuggerStepThrough]
            get
            {
                int total = 0;
                foreach (OMetaList<T> currentItem in this)
                {
                    total++;
                }

                return total;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool Remove(OMetaList<T> item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        [DebuggerStepThrough]
#line hidden
        public virtual IEnumerator<OMetaList<T>> GetEnumerator()
        {
            for (OMetaList<T> currentItem = this;
                 currentItem != Nil;
                 currentItem = currentItem.Tail)
            {
                yield return currentItem.Head;
            }
        }
#line default
        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public static OMetaList<T> Concat(params T[] items)
        {
            if ((items == null) || (items.Length == 0))
            {
                return Nil;
            }

            // start backwards
            OMetaList<T> currentList = new OMetaList<T>(items[items.Length - 1]);

            for (int ix = (items.Length - 2); ix >= 0; ix--)
            {
                currentList = new OMetaList<T>(items[ix], currentList);
            }

            return currentList;
        }

        // HACK: Need to look at all callsites to see if the semantics are clear, they
        // could use some work.
        public static OMetaList<T> ConcatLists(params OMetaList<T>[] listsToConcat)
        {            
            OMetaList<T> result = OMetaList<T>.Nil;

            if (listsToConcat.Length == 1)
            {
                return listsToConcat[0];
            }

            for (int i = listsToConcat.Length - 1; i >= 0; i--)
            {
                OMetaList<T> currentListToConcat = listsToConcat[i];
                if (currentListToConcat != OMetaList<T>.Nil)
                {
                    result = new OMetaList<T>(currentListToConcat, result);
                }
            }
            
            return result;
        }

        protected virtual T SingleItem
        {
            get
            {
                return m_HeadItem;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is T)
            {
                if (IsSingleItem)
                {
                    return SingleItem.Equals((T)obj);
                }
            }

            var otherList = obj as OMetaList<T>;
            if (otherList != null)
            {
                // This is "tricky" since enumeration can return itself.
                // And by "tricky", I mean, I designed it poorly :)
                // Can be cleaned up.

                // HACK: Overcome infinite recursion by special casing this.
                if (this.IsSingleItem && otherList.IsSingleItem)
                {
                    return this.HeadFirstItem.Equals(otherList.HeadFirstItem);
                }

                var selfEnumerator = this.GetEnumerator();
                var otherEnumerator = otherList.GetEnumerator();                

                while (selfEnumerator.MoveNext() && otherEnumerator.MoveNext())
                {
                    var currentSelfItem = selfEnumerator.Current;
                    var currentOtherItem = otherEnumerator.Current;

                    if (!currentSelfItem.Equals(currentOtherItem))
                    {
                        return false;
                    }
                }

                // Make sure no items remain.
                return !selfEnumerator.MoveNext() && !otherEnumerator.MoveNext();
            }

            if (IsSingleItem)
            {
                T firstItem = HeadFirstItem;
                if (!object.ReferenceEquals(firstItem, null))
                {
                    if (firstItem.Equals(obj))
                    {
                        return true;
                    }
                }

                // SMELL: Too many special cases
                if (!object.ReferenceEquals(obj, null))
                {
                    if (obj.Equals(firstItem))
                    {
                        return true;
                    }
                }
            }
            return base.Equals(obj);            
        }

        public override int GetHashCode()
        {
            if (IsSingleItem)
            {
                return SingleItem.GetHashCode();
            }
            return base.GetHashCode();
        }        
    }
}
