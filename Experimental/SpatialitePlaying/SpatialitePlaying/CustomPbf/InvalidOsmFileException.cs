using System;
using System.Runtime.Serialization;

namespace SpatialitePlaying.CustomPbf
{
    [Serializable]
    public class InvalidOsmFileException : Exception
    {
        public InvalidOsmFileException()
        {
        }

        public InvalidOsmFileException(string message) : base(message)
        {
        }

        public InvalidOsmFileException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidOsmFileException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}