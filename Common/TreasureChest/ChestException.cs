using System;
using System.Runtime.Serialization;

namespace TreasureChest
{
    [Serializable]
    public class ChestException : Exception
    {
        public ChestException()
        {
        }

        public ChestException(string message) : base(message)
        {
        }

        public ChestException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ChestException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}