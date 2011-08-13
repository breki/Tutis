using System;
using System.Collections.Generic;
using System.Linq;

namespace OMetaSharp
{
    /// <summary>
    /// Stores details about variables that are created during a typical compilation phase 
    /// of a language.
    /// </summary>
    public class VariableSet : IEnumerable<string>
    {
        private Dictionary<string, string> m_NameToType = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public VariableSet(string defaultType)        
        {
            this.DefaultType = defaultType ?? string.Empty;
        }

        public VariableSet()
            : this(string.Empty)
        {            
        }

        public string DefaultType
        {
            get;
            set;
        }

        public int Count
        {
            get
            {
                return m_NameToType.Count;
            }
        }

        public string GetType(string variableName)
        {
            string type;
            if (m_NameToType.TryGetValue(variableName, out type))
            {
                return string.IsNullOrEmpty(type) ? DefaultType : type;
            }

            return DefaultType;
        }

        public void Add(string variableName, string type)
        {
            string existingType;
            if (m_NameToType.TryGetValue(variableName, out existingType))
            {
                if (string.IsNullOrEmpty(existingType) && !string.IsNullOrEmpty(type))
                {
                    m_NameToType[variableName] = type;
                }

                return;
            }
                        
            m_NameToType[variableName] = type;
        }

        public void Add(string variableName)
        {
            if (!m_NameToType.ContainsKey(variableName))
            {
                m_NameToType[variableName] = string.Empty;
            }
        }

        public override string ToString()
        {            
            return string.Concat(
                "[var",
                m_NameToType.Count == 0 ? "" : " " + string.Join(", ", m_NameToType.Keys.OrderBy(k => k).ToArray()),
                "]"
                );
        }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string key in m_NameToType.Keys)
            {
                yield return key;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
