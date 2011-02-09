using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OMetaSharp
{
    // Ultimately, this should be something like HostExpression<THostLanguage>
    public class HostExpression
    {
        private readonly object m_Value;

        [DebuggerStepThrough]
        public HostExpression(object value)
        {
            m_Value = value;
        }

        public static OMetaList<HostExpression> From<TInput>(OMetaList<TInput> input)
        {
            return new OMetaList<HostExpression>(new HostExpression((TInput)input));
        }

        public bool TryCast<T>(out T castedValue)
        {
            Type want = typeof(T);
            Type have = m_Value.GetType();

            // SMELL: Differences in Type and RuntimeType cause the first not to work
            if (want.Equals(have) || (have is T) || (want.IsAssignableFrom(have)))
            {
                castedValue = (T)m_Value;
                return true;
            }            
            else if (want == typeof(char))
            {
                if (m_Value is char)
                {
                    castedValue = As<T>();
                    return true;
                }
            }
            else if (want == typeof(int))
            {
                if (m_Value is char)
                {
                    char haveChar = (char)m_Value;
                    object o = (int)(haveChar - '0');

                    castedValue = (T)o;
                    return true;
                }
            }
            else if (want == typeof(string))
            {
                if (m_Value is char)
                {
                    castedValue = (T)((object)m_Value.ToString());
                    return true;
                }
            }
            castedValue = default(T);
            return false;
        }

        public OMetaList<T> ToList<T>()
        {
            Type to = typeof(T);
            if (m_Value is string) 
            {
                if (to == typeof(char))
                {
                    return (new OMetaStringList(m_Value as string)) as OMetaList<T>;
                }
                else if (to == typeof(HostExpression))
                {
                    return new OMetaList<HostExpression>(new HostExpression(m_Value)) as OMetaList<T>;
                }
            }

            if (m_Value is T)
            {
                return new OMetaList<T>((T)m_Value);
            }
            throw new NotSupportedException();
        }

        public bool Is<T>()
        {
            return m_Value is T;
        }

        public T As<T>()
        {
            return (T)m_Value;
        }

        public bool Satisfies<T>(Predicate<T> test)
        {
            return Is<T>() && test(As<T>());
        }

        public override bool Equals(object obj)
        {
            // SMELL: Only the first should be needed
            if (m_Value.Equals(obj) || ((obj != null) && (obj.Equals(m_Value))))
            {
                return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (m_Value == null)
            {
                return 1;
            }

            return m_Value.GetHashCode();
        }

        public override string ToString()
        {
            return m_Value.ToString();
        }                
    }

    public static class HostExpressionListExtensionMethods
    {
        public static T As<T>(this OMetaList<HostExpression> exprList)
        {   
            // HACK
            if (typeof(T).Equals(typeof(string)))
            {
                return (T)(object)exprList.ToString();
            }

            HostExpression expr = (HostExpression)exprList;
            T outValue;
            if (!expr.TryCast<T>(out outValue))
            {
                throw new Exception("Conversion error");
            }

            return outValue;
        }

        public static bool Is<T>(this OMetaList<HostExpression> exprList)
        {
            if (exprList.Count != 1)
            {
                return false;
            }

            return exprList.HeadFirstItem.Is<T>();
        }

        public static IEnumerable<T> ToIEnumerable<T>(this OMetaList<HostExpression> exprList)
        {            
            return exprList.Select(x => x.As<T>());
        }
        
        public static OMetaList<HostExpression> AsList<T>(this OMetaList<HostExpression> exprList)
        {
            return new OMetaList<HostExpression>(new HostExpression(exprList.As<T>()));
        }

        public static string Join(this OMetaList<HostExpression> exprList, string separator)
        {
            return string.Join(separator, exprList.ToIEnumerable<string>().ToArray());
        }

        [DebuggerStepThrough]
        public static OMetaList<HostExpression> AsHostExpressionList(this object o)
        {
            var asList = o as OMetaList<HostExpression>;
            if (asList != null)
            {
                return asList;
            }
            return new OMetaList<HostExpression>(new HostExpression(o));
        }
        
        // HACK
        public static bool TryHostExpressionCompressionOnString(this OMetaList<HostExpression> list, out string result)
        {
            result = null;
            var sb = new StringBuilder();
            foreach (var currentItem in list)
            {
                if (!currentItem.IsSingleItem)
                {
                    return false;
                }

                var hostExpr = currentItem.HeadFirstItem;
                if (!((hostExpr.Is<char>()) || (hostExpr.Is<string>())))
                {
                    return false;
                }

                var toAppend = hostExpr.ToString();
                if (toAppend.Length != 1)
                {
                    return false;
                }
                sb.Append(hostExpr.ToString());
            }

            result = sb.ToString();
            return true;
        }        
    }    
}
