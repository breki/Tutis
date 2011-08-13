using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMetaSharp
{
    public class OMetaException : Exception
    {
        private readonly string m_StackTrace;

        private OMetaException(Exception innerException)
            : base("Some sort of OMeta error happened :-(", innerException)
        {
        }

        public OMetaException(string stackTrace)
            : this(new Exception(stackTrace))
        {
            m_StackTrace = stackTrace;
        }

        public OMetaException()
            : this(null as Exception)
        {            
        }
    }
}
