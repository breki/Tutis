using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OMetaSharp
{
    /// <summary>
    /// An OMetaStream is the flow of input into the OMeta parser. The underlying
    /// type can be anything.
    /// </summary>
    /// <typeparam name="T">Type of the underlying OMeta stream.</typeparam>    
    public class OMetaStream<TInput>
    {
        // TODO: Don't allow ANY mutations, rather return a new stream after every memoization 
        // that adds the item and a pointer to the previous copy.


        // HOTSPOT: Since the dictionary is used heavily, we can set the capacity to be
        // an average based on usage.
        private Dictionary<Rule<TInput>, MemoizedResult<TInput>> m_MemoizationStore;
        private const int AverageMemoizedResults = 7;

        private OMetaList<TInput> m_InnerList;
        private OMetaStream<TInput> m_CachedTailStream;

        public static readonly OMetaStream<TInput> End = new OMetaStream<TInput>(OMetaList<TInput>.Nil);

        [DebuggerStepThrough]
        public OMetaStream(TInput singleItem)
            : this(new OMetaList<TInput>(singleItem))
        {
        }

        [DebuggerStepThrough]
        public OMetaStream(OMetaList<TInput> innerList)
        {
            m_InnerList = innerList;
        }

        [DebuggerStepThrough]
        protected OMetaStream()
        {
            // used for derived classes only
        }

        [DebuggerStepThrough]  
        public bool HasMemoizedRule(Rule<TInput> rule) 
        {
            if (m_MemoizationStore == null)
            {
                return false;
            }
            else
            {
                return m_MemoizationStore.ContainsKey(rule);
            }
        }
        
        [DebuggerStepThrough]
        public bool TryGetMemoizedResult(Rule<TInput> rule, out MemoizedResult<TInput> value)
        {
            // HOTSPOT.            
            if (m_MemoizationStore == null)
            {
                value = null;
                return false;                
            }

            return m_MemoizationStore.TryGetValue(rule, out value);            
        }

        [DebuggerStepThrough]
        public void SetMemoizedResult(Rule<TInput> rule, MemoizedResult<TInput> result)
        {
            // HOTSPOT
            if (m_MemoizationStore == null)
            {
                m_MemoizationStore = new Dictionary<Rule<TInput>, MemoizedResult<TInput>>(AverageMemoizedResults);
            }
            m_MemoizationStore[rule] = result;
        }
        
        public virtual int Count
        {
            [DebuggerStepThrough]
            get
            {
                return m_InnerList.Count;
            }
        }

        public virtual IEnumerator<TInput> GetEnumerator()
        {
            foreach (var listItem in m_InnerList)
            {
                yield return (TInput)listItem;
            }
        }

        public virtual bool HasTail
        {
            [DebuggerStepThrough]
            get
            {
                return m_InnerList.HasTail;
            }
        }

        public virtual OMetaList<TInput> Head
        {
            [DebuggerStepThrough]
            get
            {
                return m_InnerList.Head;
            }
        }
                
        public virtual TInput this[int index]
        {
            [DebuggerStepThrough]
            get
            {
                return (TInput)m_InnerList[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }
                
        public virtual OMetaList<TInput> Tail
        {
            [DebuggerStepThrough]
            get
            {
                return m_InnerList.Tail;
            }
        }
                
        public virtual bool IsEnd
        {
            [DebuggerStepThrough]
            get
            {
                bool isEnd = (m_InnerList == OMetaList<TInput>.Nil) || m_InnerList.IsEmpty;
                return isEnd;
            }
        }

        
        public virtual OMetaStream<TInput> TailStream
        {
            [DebuggerStepThrough]
            get
            {
                if (m_CachedTailStream == null)
                {
                    m_CachedTailStream = new OMetaStream<TInput>(Tail);
                }

                return m_CachedTailStream;                
            }
        }

        public override string ToString()
        {
            return m_InnerList.ToString();
        }

        public virtual OMetaList<TInput> AsList()
        {
            return m_InnerList;
        }

        // TODO: Factor out to interface?
        // The Proxy stream really implements the functionality
        public virtual bool HasArguments
        {
            [DebuggerStepThrough]
            get
            {
                return false;
            }
        }

        [DebuggerStepThrough]
        public virtual OMetaStream<TInput> PushArguments(OMetaList<HostExpression> arguments)
        {
            return new OMetaProxyStream<TInput>(this, arguments);            
        }
                
        public virtual OMetaStream<TInput> PopArgument(out OMetaList<HostExpression> argument)
        {
            throw new NotImplementedException();        
        }

        public virtual int ArgumentCount
        {
            get
            {
                return 0;
            }
        }
    }    
}