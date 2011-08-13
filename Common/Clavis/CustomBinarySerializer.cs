using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Clavis
{
    public class CustomBinarySerializer : ISerializer
    {
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void RegisterTypeSerializer(Type type, ISerializer typeSerializer)
        {
            customTypeSerializers.Add(type.Name, typeSerializer);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public byte[] SerializeIntoByteArray(object value, out string serializationInfo)
        {
            if (value is string)
            {
                serializationInfo = "str";
                return Encoding.UTF8.GetBytes((string)value);
            }

            if (value is int)
            {
                serializationInfo = "int";
                return BitConverter.GetBytes((int)value);
            }

            if (value is double)
            {
                serializationInfo = "double";
                return BitConverter.GetBytes((double)value);
            }

            if (value is float)
            {
                serializationInfo = "float";
                return BitConverter.GetBytes((float)value);
            }

            Type type = value.GetType();
            string typeName = type.Name;

            if (!customTypeSerializers.ContainsKey(typeName))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot serialize type '{0}' since there is no custom serializer defined for it.",
                    type);
                throw new InvalidOperationException(message);
            }

            byte[] data = customTypeSerializers[typeName].SerializeIntoByteArray(value, out serializationInfo);
            // override the serialization info
            serializationInfo = typeName;
            return data;
        }

        public object DeserializeFromByteArray(byte[] data, string serializationInfo)
        {
            switch (serializationInfo)
            {
                case "str":
                    return Encoding.UTF8.GetString(data);
                case "int":
                    return BitConverter.ToInt32(data, 0);
                case "double":
                    return BitConverter.ToDouble(data, 0);
                case "float":
                    return BitConverter.ToSingle(data, 0);
            }

            if (!customTypeSerializers.ContainsKey(serializationInfo))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot deserialize type '{0}' since there is no custom serializer defined for it.",
                    serializationInfo);
                throw new InvalidOperationException(message);
            }

            return customTypeSerializers[serializationInfo].DeserializeFromByteArray(data, serializationInfo);
        }

        private Dictionary<string, ISerializer> customTypeSerializers = new Dictionary<string, ISerializer>();
    }
}