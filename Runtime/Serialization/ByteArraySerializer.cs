using System;
using System.Text;

namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// Serializes byte[] to a simple JS number array, e.g. [1,2,3,255]
    /// </summary>
    internal class ByteArraySerializer : IJsJsonSerializer
    {
        internal class ByteArrayTsDescriptor : ITsTypeDescriptor
        {
            public string GetTsTypeDefinition(Type targetType)
            {
                if (targetType == typeof(byte[]))
                    return "number[]"; // simple array of numbers
                throw new Exception($"Type {targetType} is not supported by the ByteArrayTsDescriptor");
            }
        }

        public bool CanSerialize(Type targetType, out ITsTypeDescriptor typeDescriptor)
        {
            if (targetType == typeof(byte[]))
            {
                typeDescriptor = new ByteArrayTsDescriptor();
                return true;
            }

            typeDescriptor = null;
            return false;
        }

        public string Serialize(object targetObject)
        {
            if (targetObject == null)
                return "null";

            if (targetObject is not byte[] data)
                throw new Exception($"Type {targetObject.GetType()} is not supported by the ByteArraySerializer");

            // Estimate string length: 
            // prefix + suffix ("[]") = 2 chars
            // each byte: up to 3 chars ("255") + 1 comma = 4 chars
            int estimatedCapacity = 2 + (data.Length * 4);

            var sb = new StringBuilder(estimatedCapacity);
            sb.Append("[");
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString());
                if (i < data.Length - 1)
                    sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
