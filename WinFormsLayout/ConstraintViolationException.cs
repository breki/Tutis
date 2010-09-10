using System;
using System.Runtime.Serialization;

namespace App
{
    [Serializable]
    public class ConstraintViolationException : Exception
    {
        public ConstraintViolationException()
        {
        }

        public ConstraintViolationException(string message) : base(message)
        {
        }

        public ConstraintViolationException(string messageFormat, params object[] args)
            : this (string.Format(messageFormat, args))
        {
        }

        public ConstraintViolationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ConstraintViolationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}