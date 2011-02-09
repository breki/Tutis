using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OMetaSharp
{
    // SMELL: Should this be needed for foreign grammars since we have immutable streams except for memomization?

    public class OMetaProxyStream<TInput> : OMetaStream<TInput>
    {
        private readonly OMetaStream<TInput> m_StreamToProxy;
        private OMetaProxyStream<TInput> m_CachedTail;

        // The argument stack is lazy initialized for performance reasons
        private OMetaList<HostExpression> m_ArgumentStack;

        [DebuggerStepThrough]
        public OMetaProxyStream(OMetaStream<TInput> streamToProxy)              
        {               
            m_StreamToProxy = streamToProxy;                     
        }

        // SMELL: The internal marking is here because it should only called by itself or OMetaStream
        [DebuggerStepThrough]
        internal OMetaProxyStream(OMetaStream<TInput> streamToProxy, OMetaList<HostExpression> argumentStack)
            : this(streamToProxy)
        {            
            m_ArgumentStack = (argumentStack == OMetaList<HostExpression>.Nil) ? null : argumentStack;
        }

                
        public override string ToString()
        {
            return m_StreamToProxy.ToString();
        }
                
        public override OMetaStream<TInput> TailStream
        {
            [DebuggerStepThrough]
            get
            {
                if (m_CachedTail == null)
                {
                    m_CachedTail = new OMetaProxyStream<TInput>(m_StreamToProxy.TailStream);
                }

                return m_CachedTail;
            }
        }
                
        public OMetaStream<TInput> TargetStream
        {
            [DebuggerStepThrough]
            get
            {
                return m_StreamToProxy;
            }
        }

        public override int Count
        {
            [DebuggerStepThrough]
            get
            {
                return m_StreamToProxy.Count;
            }
        }

        [DebuggerStepThrough]
        public override IEnumerator<TInput> GetEnumerator()
        {
            return m_StreamToProxy.GetEnumerator();
        }

        public override bool HasTail
        {
            [DebuggerStepThrough]
            get
            {
                return m_StreamToProxy.HasTail;
            }
        }

        public override OMetaList<TInput> Head
        {
            [DebuggerStepThrough]
            get
            {
                return m_StreamToProxy.Head;
            }
        }

        public override OMetaList<TInput> Tail
        {
            [DebuggerStepThrough]
            get
            {
                return m_StreamToProxy.Tail;
            }
        }

        public override TInput this[int index]
        {
            [DebuggerStepThrough]
            get
            {
                return m_StreamToProxy[index];
            }

            [DebuggerStepThrough]
            set
            {
                base[index] = value;
            }
        }

        public override OMetaList<TInput> AsList()
        {
            return m_StreamToProxy.AsList();
        }

        public override bool IsEnd
        {
            [DebuggerStepThrough]
            get
            {
                return !HasArguments && m_StreamToProxy.IsEnd;
            }
        }

        public override bool HasArguments
        {
            [DebuggerStepThrough]
            get
            {
                if (m_ArgumentStack != null)
                {
                    return true;
                }

                OMetaProxyStream<TInput> asProxy = m_StreamToProxy as OMetaProxyStream<TInput>;
                if (asProxy == null)
                {
                    return false;
                }

                return asProxy.HasArguments;
            }
        }

        [DebuggerStepThrough]
        public override OMetaStream<TInput> PushArguments(OMetaList<HostExpression> arguments)
        {
            // REVIEW: Should Arguments be reversed?
            var actualArguments = (m_ArgumentStack == null) ? arguments : OMetaList<HostExpression>.ConcatLists(arguments,m_ArgumentStack);
            return new OMetaProxyStream<TInput>(m_StreamToProxy, actualArguments);
        }

        [DebuggerStepThrough]
        public override OMetaStream<TInput> PopArgument(out OMetaList<HostExpression> argument)
        {
            if (m_ArgumentStack != null)
            {
                argument = m_ArgumentStack.Head;
                OMetaList<HostExpression> remainder = m_ArgumentStack.Tail;
                return new OMetaProxyStream<TInput>(m_StreamToProxy, remainder);
            }
            else
            {
                OMetaStream<TInput> modifiedStream = m_StreamToProxy.PopArgument(out argument);
                return new OMetaProxyStream<TInput>(modifiedStream, null);
            }
        }

        public override int ArgumentCount
        {
            [DebuggerStepThrough]
            get
            {
                int result = 0;

                OMetaProxyStream<TInput> asProxy = m_StreamToProxy as OMetaProxyStream<TInput>;
                if (asProxy != null)
                {
                    result += asProxy.ArgumentCount;
                }

                if (m_ArgumentStack != null)
                {
                    result += m_ArgumentStack.Count;
                }

                return result;
            }
        }
    }
}
