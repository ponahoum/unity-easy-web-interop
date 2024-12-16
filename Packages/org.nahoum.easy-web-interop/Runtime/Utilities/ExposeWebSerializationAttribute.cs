
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Graphs;
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

    /// <summary>
    /// To put on a class to tell how a class should be exposed to the web
    /// Both for serialization and for generating the typescript
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class ExposeWebSerializationAttribute : PreserveAttribute
    {
        public IJsJsonSerializer serializer { get; }

        /// <summary>
        /// SerializerType be a type that must implement <see cref="IJsJsonSerializer"/>
        /// </summary>
        public ExposeWebSerializationAttribute(Type serializerType)
        {
            // Check the type is the one of an object that implements IJsJsonSerializer
            if (!typeof(IJsJsonSerializer).IsAssignableFrom(serializerType))
                throw new Exception($"Type {serializerType} does not implement {typeof(IJsJsonSerializer)}");

            // Create an instance of the serializer
            serializer = (IJsJsonSerializer)Activator.CreateInstance(serializerType);
        }

        internal IJsJsonSerializer GetSerializer()
        {
            return serializer;
        }

        /// <summary>
        /// Given a type, tries to get the serializer for it from the attribute <see cref="ExposeWebSerializationAttribute"/>
        /// </summary>
        internal static bool TryGetSerializer(Type targetType, out IJsJsonSerializer serializer)
        {
            serializer = null;

            ExposeWebSerializationAttribute attribute = targetType.GetCustomAttribute<ExposeWebSerializationAttribute>();
            if (attribute == null)
                return false;

            // If the serializer is not set, return null
            serializer = attribute.GetSerializer();
            if (serializer == null || !serializer.CanSerialize(targetType, out _))
            {
                serializer = null;
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Returns the form of the type in typescript
    /// </summary>
    public interface ITsTypeDescriptor
    {
        /// <summary>
        /// Returns the typescript type definition
        /// For example, for a Vector3, it would return {x: number, y: number, z: number}
        /// </summary>
        string GetTsTypeDefinition(Type targetType);
    }

    /// <summary>
    /// Returns a json 
    /// </summary>
    public interface IJsJsonSerializer
    {
        string Serialize(object targetObject);
        bool CanSerialize(Type targetType, out ITsTypeDescriptor typeDescriptor);
    }

    /// <summary>
    /// A default to easily serialize a type of type T to typescript
    /// </summary>
    [Preserve]
    public abstract class DefaultTypescriptSerializer<T> : IJsJsonSerializer
    {
        protected DefaultTypescriptSerializer()
        {

        }

        private class TsTypeDescriptor : ITsTypeDescriptor
        {
            string tsTypeDefinition;

            public TsTypeDescriptor(string tsTypeDefinition)
            {
                this.tsTypeDefinition = tsTypeDefinition;
            }

            public string GetTsTypeDefinition(Type targetType)
            {
                if (targetType == typeof(T))
                    return tsTypeDefinition;
                throw new Exception($"Type {targetType} is not supported by the TsTypeDescriptor");
            }
        }

        public bool CanSerialize(Type targetType, out ITsTypeDescriptor typeDescriptor)
        {
            typeDescriptor = new TsTypeDescriptor(GetTsTypeDefinition());
            return targetType == typeof(T);
        }

        public string Serialize(object targetObject)
        {
            // Check type
            if (!(targetObject is T asT))
                throw new Exception($"Expected type {typeof(T)} but got {targetObject.GetType()}");
            return SerializeToJavascript(asT);
        }

        /// <summary>
        /// Implement this to serialize the object to javascript
        /// </summary>
        protected abstract string SerializeToJavascript(T targetObject);

        /// <summary>
        /// Implement this to return the typescript type definition
        /// </summary>
        protected abstract string GetTsTypeDefinition();
    }
}