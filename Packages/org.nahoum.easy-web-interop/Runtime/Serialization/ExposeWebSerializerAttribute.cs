
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;
namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// To put on a serializer class so that it serializes a target type to javascript
    /// Can put multiple ones on a serializer class.
    /// For example, to serialize double3, you'll add ExposeWebSerializerAttribute(typeof(double3)) on the double3Serializer class
    /// This is typically used to declare serializers for types that are not directly accessible in the current assembly
    /// Use <ref href="ExposeWebSerializationAttribute"/> to declare a serializer for a type that is in the current assembly and that you can access, it's more efficient
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ExposeWebSerializerAttribute : PreserveAttribute
    {
        public Type objectToSerializeType { get; }

        // Static cache
        static bool initialized = false;
        static Dictionary<Type, IJsJsonSerializer> serializers = new Dictionary<Type, IJsJsonSerializer>();

        /// <summary>
        /// SerializerType be a type that must implement <see cref="IJsJsonSerializer"/>
        /// </summary>
        public ExposeWebSerializerAttribute(Type objectToSerializeType)
        {
            this.objectToSerializeType = objectToSerializeType;
        }

        private static void GetAllAttributesByTypes()
        {
            if (initialized)
                return;

            IReadOnlyCollection<Type> allTypes = ReflectionUtilities.GetAllAssembliesTypes();
            foreach (Type type in allTypes)
            {

                // Get all the attributes
                IEnumerable<ExposeWebSerializerAttribute> attributes = type.GetCustomAttributes<ExposeWebSerializerAttribute>();
                foreach (ExposeWebSerializerAttribute attribute in attributes)
                {
                    // Skip if the type is not a class and doesn't implement IJsJsonSerializer
                    if (!type.IsClass || !typeof(IJsJsonSerializer).IsAssignableFrom(type))
                        throw new Exception($"Type {type} does not implement {typeof(IJsJsonSerializer)}. You cannot add the attribute {typeof(ExposeWebSerializerAttribute)} to it");

                    // Add the serializer to the cache
                    if (!serializers.ContainsKey(attribute.objectToSerializeType))
                    {
                        // Create isntance of serializer
                        serializers.Add(attribute.objectToSerializeType, (IJsJsonSerializer)Activator.CreateInstance(type));
                    }
                    else
                    {
                        throw new Exception($"A serializer for type {attribute.objectToSerializeType} is already registered via the attribute {typeof(ExposeWebSerializerAttribute)}");
                    }
                }
            }
            initialized = true;
        }

        /// <summary>
        /// Tries to get a serializer for a given type
        /// </summary>
        internal static bool TryGetSerializer(Type targetType, out IJsJsonSerializer serializer)
        {
            // Recache if needed
            GetAllAttributesByTypes();

            // Try to get the serializer
            if (serializers.TryGetValue(targetType, out serializer))
                return true;

            return false;
        }
    }
}