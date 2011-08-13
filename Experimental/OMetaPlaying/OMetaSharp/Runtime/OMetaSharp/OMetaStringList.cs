using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OMetaSharp
{
    /// <summary>
    /// Performance enhanced way of referring to strings as a list of characters
    /// </summary>
    public class OMetaStringList : OMetaList<char>
    {
        private readonly string m_ActualString;
        private readonly int m_StartIndex;

        [DebuggerStepThrough]
        public OMetaStringList(string s)
            : this(s, 0)
        {
        }

        [DebuggerStepThrough]
        public OMetaStringList(string s, int startIndex)
        {
            m_ActualString = s;
            m_StartIndex = startIndex;
        }

        [DebuggerStepThrough]
        public OMetaStringList(char c)
            : this(c.ToString())
        {
        }

        public override int Count
        {
            get
            {
                return m_ActualString.Length - m_StartIndex;
            }
        }

        public override IEnumerator<OMetaList<char>> GetEnumerator()
        {
            for (int ix = m_StartIndex; ix < m_ActualString.Length; ix++)
            {
                yield return new OMetaList<char>(m_ActualString[ix]);
            }
        }

        public override bool HasTail
        {
            get
            {
                return Count > 1;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return Count == 0;
            }
        }


        public override OMetaList<char> Head
        {
            get
            {
                return new OMetaList<char>(m_ActualString[m_StartIndex]);
            }
        }

        public override int IndexOf(OMetaList<char> item)
        {
            char itemAsChar = (char)item;

            for (int ix = m_StartIndex; ix < m_ActualString.Length; ix++)
            {
                if (m_ActualString[ix] == itemAsChar)
                {
                    return ix - m_StartIndex;
                }
            }

            return -1;
        }

        public override bool IsSingleItem
        {
            get
            {
                return m_ActualString.Length == 1;
            }
        }

        public override OMetaList<char> Tail
        {
            get
            {
                if (Count <= 1)
                {
                    return OMetaList<char>.Nil;
                }

                return new OMetaStringList(m_ActualString, m_StartIndex + 1);
            }
        }

        public override OMetaList<char> this[int index]
        {
            get
            {
                return new OMetaList<char>(m_ActualString[index]);
            }
            set
            {
                base[index] = value;
            }
        }
                
        public override string ToString()
        {
            // REVIEW: Is this best? Should it look more listy?
            return "'" + m_ActualString.Substring(m_StartIndex) + "'";
        }

        protected override char SingleItem
        {
            get
            {
                return m_ActualString[m_StartIndex];
            }
        }

        public static explicit operator string(OMetaStringList l)
        {
            return l.m_ActualString.Substring(l.m_StartIndex);
        }

        public static explicit operator OMetaStringList(string s)
        {
            return new OMetaStringList(s);
        }

        public override bool Equals(object obj)
        {
            if (((string)this).Equals(obj))
            {
                return true;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ((string)this).GetHashCode();
        }
    }
}
