
using System;
using System.Reflection;
using UnityEngine.Scripting;
namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// To put on a class to tell how a class should be exposed to the web
    /// Both for serialization and for generating the typescript
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ExposeWebSerializationAttribute : PreserveAttribute
    {
        IJsJsonSerializer serializer;

        public ExposeWebSerializationAttribute(IJsJsonSerializer serializer)
        {
            this.serializer = serializer;
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
}