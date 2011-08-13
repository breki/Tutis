using System.Diagnostics.CodeAnalysis;

namespace Clavis
{
    public interface ISerializer
    {
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        byte[] SerializeIntoByteArray(object value, out string serializationInfo);
        object DeserializeFromByteArray(byte[] data, string serializationInfo);
    }
}