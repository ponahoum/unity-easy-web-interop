
using System;
using System.Collections.Generic;
namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// This serializer handles Lists of basic types, similar to how BasicArraySerializer handles arrays.
    /// </summary>
    internal class BasicListSerializer : IJsJsonSerializer
    {
        internal class BasicListSerializerTsDescriptor : ITsTypeDescriptor
        {
            public string GetTsTypeDefinition(Type targetType)
            {
                BasicTypesSerializer basicTypesSerializer = new BasicTypesSerializer();

                // Check if the type is a generic List<T>
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type elementType = targetType.GetGenericArguments()[0];
                    if (basicTypesSerializer.CanSerialize(elementType, out ITsTypeDescriptor typeDescriptor))
                    {
                        return typeDescriptor.GetTsTypeDefinition(elementType) + "[]";
                    }
                }

                throw new Exception($"Type {targetType} is not supported by the BasicListSerializerTsDescriptor");
            }
        }

        private static string SerializeList(object listObj)
        {
            BasicTypesSerializer basicTypesSerializer = new BasicTypesSerializer();

            // Ensure that the object is a List<T>
            Type objType = listObj.GetType();
            if (!objType.IsGenericType || objType.GetGenericTypeDefinition() != typeof(List<>))
                throw new ArgumentException("Object is not a List<T>", nameof(listObj));

            // Enumerate the list items
            var asList = (System.Collections.IEnumerable)listObj;
            string result = "[";
            foreach (var item in asList)
            {
                result += basicTypesSerializer.Serialize(item) + ",";
            }

            if (result.Length > 1) // means we added at least one item
                result = result.Remove(result.Length - 1); // remove trailing comma

            result += "]";
            return result;
        }

        public bool CanSerialize(Type targetType, out ITsTypeDescriptor typeDescriptor)
        {
            typeDescriptor = null;
            // Check if target type is List<T> for a supported T
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = targetType.GetGenericArguments()[0];
                if (BasicTypesSerializer.CanSerialize(elementType))
                {
                    typeDescriptor = new BasicListSerializerTsDescriptor();
                    return true;
                }
            }

            return false;
        }

        public string Serialize(object targetObject)
        {
            if (targetObject == null)
                return "null";

            // Handle List<T>
            Type objType = targetObject.GetType();
            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>))
                return SerializeList(targetObject);

            // Other cases are not supported by this serializer
            throw new Exception($"Type {targetObject.GetType()} is not supported by the BasicListSerializer");
        }
    }


    /// <summary>
    /// This is a basic serializer that serializes Arrays of basic types
    /// </summary>
    internal class BasicArraySerializer : IJsJsonSerializer
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
                        // Important to add () if the object is nullable (aka string |null for example)
                        return $"({typeDescriptor.GetTsTypeDefinition(elementType)})[]";
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

            return false;
        }

        public string Serialize(object targetObject)
        {
            if (targetObject == null)
                return "null";

            // Case array
            if (targetObject.GetType().IsArray)
                return SerializeArray(targetObject);

            // Other cases are not supported
            throw new Exception($"Type {targetObject.GetType()} is not supported by the BasicCollectionSerializer");
        }
    }
}