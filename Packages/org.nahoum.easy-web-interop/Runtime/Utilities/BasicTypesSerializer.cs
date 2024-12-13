
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// A basic serialize to serialize native types aka primitives types in js
    /// </summary>
    internal class BasicTypesSerializer : IJsJsonSerializer
    {
        internal class BasicTypesDescriptorTsGenerator : ITsTypeDescriptor
        {
            public string GetTsTypeDefinition(Type targetType)
            {
                if (targetType == typeof(int))
                    return "number";
                else if (targetType == typeof(float))
                    return "number";
                else if (targetType == typeof(double))
                    return "number";
                else if (targetType == typeof(long))
                    return "number";
                else if (targetType == typeof(string))
                    return "string | null";
                else if (targetType == typeof(byte))
                    return "number";
                else if (targetType == typeof(sbyte))
                    return "number";
                else if (targetType == typeof(bool))
                    return "boolean";
                else if (targetType == typeof(Type))
                    return "string";
                else if (targetType == typeof(Vector2))
                    return "{x: number, y: number}";
                else if (targetType == typeof(Vector3))
                    return "{x: number, y: number, z: number}";
                else if (targetType == typeof(Vector4))
                    return "{x: number, y: number, z: number, w: number}";
                else if (targetType == typeof(Quaternion))
                    return "{x: number, y: number, z: number, w: number}";
                else if (targetType == typeof(Color))
                    return "{r: number, g: number, b: number, a: number}";
                else if (targetType == typeof(Color32))
                    return "{r: number, g: number, b: number, a: number}";
                else if (targetType == typeof(Bounds))
                    return "{center: " + GetTsTypeDefinition(typeof(Vector3)) + ", extents: " + GetTsTypeDefinition(typeof(Vector3)) + "}";
                else
                    throw new Exception($"Type {targetType} is not supported by the BasicTypesDescriptorTsGenerator");
            }
        }

        public bool CanSerialize(Type targetType, out ITsTypeDescriptor typeDescriptor)
        {
            typeDescriptor = null;
            if (supportedTypes.Contains(targetType))
            {
                typeDescriptor = new BasicTypesDescriptorTsGenerator();
                return true;
            }

            return false;
        }

        public string Serialize(object targetObject)
        {
            if (targetObject == null)
                return "null";
            else if (targetObject is int || targetObject is float || targetObject is double || targetObject is long)
                return targetObject.ToString().Replace(",", ".");
            else if (targetObject is string)
                return "\"" + targetObject.ToString() + "\"";
            else if (targetObject is byte || targetObject is sbyte)
                return targetObject.ToString();
            else if (targetObject is sbyte)
                return targetObject.ToString();
            else if (targetObject is bool)
                return targetObject.ToString().ToLower();
            else if (targetObject is Type asType)
                return "\"" + asType.FullName + "\"";
            else if (targetObject is Bounds asBounds)
            {
                Vector3 center = asBounds.center;
                Vector3 extents = asBounds.extents;
                string result = "{";
                result += $"\"center\": {JsonUtility.ToJson(center)},";
                result += $"\"extents\": {JsonUtility.ToJson(extents)}";
                result += "}";
                return result;
            }
            // Basic unity types, use Unity serialization
            else if (targetObject is Vector2 || targetObject is Vector3 || targetObject is Vector4 || targetObject is Quaternion || targetObject is Color || targetObject is Color32)
            {
                return JsonUtility.ToJson(targetObject);
            }
            else
                throw new Exception($"Type {targetObject.GetType()} is not supported by the BasicTypesSerializer");
        }

        internal static bool CanSerialize(Type targetType)
        {
            return supportedTypes.Contains(targetType);
        }

        internal static HashSet<Type> GetSupportedTypes()
        {
            // Copy the hashset to avoid modification. In c# 9 we could use a readonly hashset "ReadOnlySet"
            return new HashSet<Type>(supportedTypes);
        }

        private static readonly HashSet<Type> supportedTypes = new HashSet<Type>()
            {
                typeof(int),
                typeof(float),
                typeof(double),
                typeof(long),
                typeof(string),
                typeof(byte),
                typeof(sbyte),
                typeof(bool),
                typeof(Type).GetType(),
                typeof(Vector2),
                typeof(Vector3),
                typeof(Vector4),
                typeof(Quaternion),
                typeof(Color),
                typeof(Color32),
                typeof(Bounds),
            };
    }
}