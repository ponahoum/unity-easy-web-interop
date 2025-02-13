
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Nahoum.UnityJSInterop
{

    /// <summary>
    /// A simple serializer that converts an object to a JSON string
    /// </summary>
    public static class ObjectSerializer
    {
        static HashSet<IJsJsonSerializer> RegisteredSerializers = new HashSet<IJsJsonSerializer>();

        static ObjectSerializer()
        {
            // Add basic serializers for primitive types and arrays of primitive types
            RegisterSerializer(new BasicTypesSerializer());
            RegisterSerializer(new BasicArraySerializer());
            RegisterSerializer(new BasicListSerializer());
        }

        /// <summary>
        /// Register a serializer for a specific type
        /// You may also use the attribute <see cref="ExposeWebSerializationAttribute"/> to register a serializer on a given type
        /// </summary>
        public static void RegisterSerializer(IJsJsonSerializer serializer)
        {
            if (RegisteredSerializers.Contains(serializer))
                Debug.LogWarning($"A serializer {serializer} is already registered. Overwriting it");

            RegisteredSerializers.Add(serializer);
        }

        /// <summary>
        /// Returns a proper JS serialization for the provided object
        /// This method could be improved and cache the serializers for each type // TODO
        /// </summary>
        internal static bool TryGetSerializer(Type toSerializeType, out IJsJsonSerializer serializer)
        {
            serializer = null;

            // First try to get the serializer from the registered serializers
            foreach (IJsJsonSerializer registeredSerializer in RegisteredSerializers)
            {
                if (registeredSerializer.CanSerialize(toSerializeType, out _))
                {
                    serializer = registeredSerializer;
                    return true;
                }
            }

            // If no serializer is found, try to look for one in the attributes
            if (ExposeWebSerializationAttribute.TryGetSerializer(toSerializeType, out serializer))
            {
                // Register the serializer for future use
                RegisterSerializer(serializer);
                return true;
            }

            // If still, no serializer is found, try to get from the attributes put on the serializers themselves
            if (ExposeWebSerializerAttribute.TryGetSerializer(toSerializeType, out serializer))
            {
                // Register the serializer for future use
                RegisterSerializer(serializer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set this to override the default serializer
        /// </summary>
        internal static string ToJson(object toSerialize)
        {
            string baseJson = "{\"value\":%value%}";

            // If null, return null
            if (toSerialize == null)
                return baseJson.Replace("%value%", "null");

            // Otherwise look for a serializer
            if (TryGetSerializer(toSerialize.GetType(), out IJsJsonSerializer serializer))
            {
                baseJson = baseJson.Replace("%value%", serializer.Serialize(toSerialize));
                return baseJson;
            }

            // Otherwise return empty object
            Debug.LogWarning($"No serializer found for type {toSerialize.GetType()}. It's highly recommended to add a serializer for this type");
            return baseJson.Replace("%value%", "{}");
        }
    }
}