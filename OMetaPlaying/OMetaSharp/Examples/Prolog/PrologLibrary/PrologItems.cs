using System.Collections.Generic;
using System.Linq;

namespace OMetaSharp.Examples.Prolog
{
    public class PrologItems : IPrologItem, IEnumerable<IPrologItem>
    {
        private readonly IList<IPrologItem> m_Items;

        public PrologItems(IEnumerable<IPrologItem> items)
        {
            m_Items = new List<IPrologItem>(items);
        }

        public PrologItems(params IPrologItem[] items)
        {
            m_Items = new List<IPrologItem>(items);
        }

        public IPrologItem Rename(string nm)
        {
            return new PrologItems(m_Items.Select(x => x.Rename(nm)));
        }

        public IPrologItem Rewrite(IDictionary<string, IPrologItem> env)
        {
            return new PrologItems(m_Items.Select(i => i.Rewrite(env)));
        }

        public int Count
        {
            get
            {
                return m_Items.Count;
            }
        }

        public IPrologItem this[int index]
        {
            get
            {
                return m_Items[index];
            }
        }

        public void Push(IPrologItem item)
        {
            m_Items.Add(item);
        }

        public IPrologItem Pop()
        {
            var result = m_Items[m_Items.Count - 1];
            m_Items.RemoveAt(m_Items.Count - 1);
            return result;
        }

        #region IEnumerable<IPrologItem> Members

        public IEnumerator<IPrologItem> GetEnumerator()
        {
            foreach (IPrologItem item in m_Items)
            {
                yield return item;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            return string.Join(", ", m_Items.Select(x => x.ToString()).ToArray());
        }
    }
}
