using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMetaSharp
{
    /// <summary>
    /// Flattened list (not a tree).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This class is a performance enhanced version of <c>OMetaList</c>
    /// </remarks>
    public class OMetaFlatList<T> : OMetaList<T>
    {
        private readonly int m_StartIndex;
        private readonly IList<T> m_ListToWrap;
        private OMetaList<T> m_CachedTail;

        public OMetaFlatList(params T[] innerList)
            : this((IList<T>)innerList)
        {
        }

        public OMetaFlatList(IList<T> listToWrap)
            : this(listToWrap, 0)
        {
        }

        public OMetaFlatList(IList<T> listToWrap, int startIndex)
        {
            m_StartIndex = startIndex;
            m_ListToWrap = listToWrap;
        }

        public override int Count
        {
            get
            {
                return m_ListToWrap.Count - m_StartIndex;
            }
        }

        public override bool HasTail
        {
            get
            {
                return (m_StartIndex + 1) < m_ListToWrap.Count;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return Count == 0;
            }
        }

        public override OMetaList<T> Head
        {
            get
            {
                return new OMetaList<T>(m_ListToWrap[m_StartIndex]);
            }
        }

        public override OMetaList<T> Tail
        {
            get
            {
                if (m_CachedTail == null)
                {
                    if (HasTail)
                    {
                        m_CachedTail = new OMetaFlatList<T>(m_ListToWrap, m_StartIndex + 1);
                    }
                    else
                    {
                        return Nil;
                    }
                }

                return m_CachedTail;
            }
        }

        public override OMetaList<T> this[int index]
        {
            get
            {
                return new OMetaList<T>(m_ListToWrap[m_StartIndex + index]);
            }
            set
            {
                base[index] = value;
            }
        }

        public override int IndexOf(OMetaList<T> item)
        {
            // Can't use m_ListToWrap.IndexOf(...) since it might return
            // a value before m_StartIndex

            for (int ix = m_StartIndex; ix < m_ListToWrap.Count; ix++)
            {
                T currentItem = m_ListToWrap[ix];
                if (object.Equals(new OMetaList<T>(currentItem), item))
                {
                    return ix - m_StartIndex;
                }
            }

            return -1;
        }

        protected override T SingleItem
        {
            get
            {
                return m_ListToWrap[m_StartIndex];
            }
        }

        public override bool IsSingleItem
        {
            get
            {
                return Count == 1;
            }
        }

        public override string ToString()
        {
            // Slight performance optimization simply because it's used so much.
            if (this == Nil)
            {
                return base.ToString();
            }

            // HACK
            // SMELL: Duplicated code with normal one
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

            if (Count != 1)
            {
                sb.Append("[");
            }

            OMetaList<T> currentList = this;

            for (int ix = m_StartIndex; ix < m_ListToWrap.Count; ix++)
            {
                T currentItem = m_ListToWrap[ix];
                sb.Append(currentItem == null ? "(null)" : currentItem.ToString());

                if (ix < (m_ListToWrap.Count - 1))
                {
                    sb.Append(", ");
                }
            }

            if (Count != 1)
            {
                sb.Append("]");
            }        

            return sb.ToString();
        }
    }
}
