
using System;
namespace Nahoum.UnityJSInterop
{

    /// <summary>
    /// This is a basic serializer that serializes Arrays and Lists of basic types
    /// </summary>
    internal class BasicCollectionSerializer : IJsJsonSerializer
    {
        internal class BasicCollectionSerializerTsDescriptor : ITsTypeDescriptor
        {
            public string GetTsTypeDefinition(Type targetType)
            {
                BasicTypesSerializer basicTypesSerializer = new BasicTypesSerializer();

                if (targetType.IsArray)
                {
                    Type elementType = targetType.GetElementType();
                    if (basicTypesSerializer.CanSerialize(elementType, out ITsTypeDescriptor typeDescriptor))
                        return typeDescriptor.GetTsTypeDefinition(elementType) + "[]";

                }

                throw new Exception($"Type {targetType} is not supported by the BasicCollectionSerializerTsDescriptor");
            }
        }

        private static string SerializeArray(object array)
        {
            BasicTypesSerializer basicTypesSerializer = new BasicTypesSerializer();

            // Check object is an array or throw
            if (!array.GetType().IsArray)
                throw new ArgumentException("Object is not an array");

            string result = "[";
            var asArray = (Array)array;
            foreach (var item in asArray)
            {
                result += basicTypesSerializer.Serialize(item) + ",";
            }
            if (asArray.Length > 0)
                result = result.Remove(result.Length - 1);

            result += "]";
            return result;
        }

        public bool CanSerialize(Type targetType, out ITsTypeDescriptor typeDescriptor)
        {
            typeDescriptor = null;
            // Check if target type is Array, we support all arrays of primitive types
            if (targetType.IsArray && targetType.GetElementType() != null && BasicTypesSerializer.CanSerialize(targetType.GetElementType()))
            {
                typeDescriptor = new BasicCollectionSerializerTsDescriptor();
                return true;
            }

            // Case list: TODO

            return false;
        }

        public string Serialize(object targetObject)
        {
            if (targetObject == null)
                return "null";

            // Case array
            if (targetObject.GetType().IsArray)
                return SerializeArray(targetObject);

            // Case list: TODO

            // Other cases are not supported
            throw new Exception($"Type {targetObject.GetType()} is not supported by the BasicCollectionSerializer");
        }
    }
}