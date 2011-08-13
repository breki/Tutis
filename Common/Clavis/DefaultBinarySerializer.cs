using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Clavis
{
    public class DefaultBinarySerializer : ISerializer
    {
        public byte[] SerializeIntoByteArray(object value, out string serializationInfo)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, value);
                serializationInfo = null;
                return stream.ToArray();
            }
        }

        public object DeserializeFromByteArray(byte[] data, string serializationInfo)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return binaryFormatter.Deserialize(stream);
            }
        }

        private BinaryFormatter binaryFormatter = new BinaryFormatter();
    }
}