
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine.Scripting;
namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// Add this attribute anywhere to notice one may want to expose the method to the web
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class GlobalExposeWebSerializationAttribute : PreserveAttribute
    {
        static Dictionary<Type, IJsJsonSerializer> serializers = new Dictionary<Type, IJsJsonSerializer>();

        public GlobalExposeWebSerializationAttribute(Type serializerType, Type objectTypeToSerialize)
        {
            // Check the type is the one of an object that implements IJsJsonSerializer
            if (!typeof(IJsJsonSerializer).IsAssignableFrom(serializerType))
                throw new Exception($"Type {serializerType} does not implement {typeof(IJsJsonSerializer)}");

            if (serializers.ContainsKey(objectTypeToSerialize))
                throw new Exception($"A serializer for {objectTypeToSerialize} is already registered in global scope.");

            // Create an instance of the serializer
            IJsJsonSerializer serializer = (IJsJsonSerializer)Activator.CreateInstance(serializerType);

            // Check if the serializer can serialize the type
            if (!serializer.CanSerialize(objectTypeToSerialize, out _))
                throw new Exception($"Serializer {serializerType} cannot serialize {objectTypeToSerialize}");

            // Register the serializer for the type
            serializers[objectTypeToSerialize] = serializer;
        }

        /// <summary>
        /// Given a type, tries to get a global serializer for it
        /// </summary>
        internal static bool TryGetGlobalSerializer(Type targetType, out IJsJsonSerializer serializer, out ITsTypeDescriptor typescriptDescriptor)
        {
            serializer = null;
            typescriptDescriptor = null;

            if (serializers.TryGetValue(targetType, out serializer) && serializer.CanSerialize(targetType, out typescriptDescriptor))
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
        internal static bool TryGetSerializer(Type targetType, out IJsJsonSerializer serializer, out ITsTypeDescriptor typescriptDescriptor)
        {
            serializer = null;
            typescriptDescriptor = null;

            ExposeWebSerializationAttribute attribute = targetType.GetCustomAttribute<ExposeWebSerializationAttribute>();
            if (attribute == null)
                return false;

            // If the serializer is not set, return null
            serializer = attribute.GetSerializer();
            if (serializer == null || !serializer.CanSerialize(targetType, out typescriptDescriptor))
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