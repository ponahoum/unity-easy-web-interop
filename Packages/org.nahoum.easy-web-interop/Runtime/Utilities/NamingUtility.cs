using System;
using System.Reflection;

namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// A utility for giving the right names to the exposed methods
    /// </summary>
    public static class NamingUtility
    {
        /// <summary>
        /// Get the path of a method on the JS side
        /// Only use for static methods
        /// </summary>
        public static string[] GetMethodJSPath(MethodInfo method)
        {
            // Check method is static
            if (!method.IsStatic)
                throw new System.Exception("Method must be static");

            string namespaceName = method.DeclaringType.Namespace;
            bool hasNamespace = !string.IsNullOrEmpty(namespaceName);

            // Split the namespace to get the class name
            string staticPrefix = "static";
            string className = method.DeclaringType.Name;
            string methodName = method.Name;

            // Add aggregate path (concat namespace split and classname and method name)
            if (hasNamespace)
                return new string[] { staticPrefix, namespaceName, className, methodName };
            else
                return new string[] { staticPrefix, className, methodName };

        }

        /// <summary>
        /// Generates a well formatted name for a type relative to a provided namespace
        /// Typically used to generate generic types names that look good (Action<string> instead of Action`1)
        /// </summary>
        public static string GenerateWellFormattedJSNameForType(Type type, string relativeNamespaceName = null)
        {
            // Get the default name
            string typeName = type.Name;

            // Take the namespace of the type if not provided
            if (relativeNamespaceName == null)
                relativeNamespaceName = type.Namespace;

            // Case generic type: Action<string> -> "Action<string>"
            if (type.IsGenericType)
            {
                typeName = type.Name.Substring(0, type.Name.IndexOf('`'));
                typeName += "<";
                int index = 0;
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    typeName += GenerateWellFormattedJSNameForType(genericArgument, type.Namespace);
                    if (index < type.GetGenericArguments().Length - 1)
                        typeName += ", ";
                    index++;
                }
                typeName += ">";
            }

            // If the type is in the same namespace, we don't need to prefix it
            if (relativeNamespaceName == type.Namespace)
                return typeName;
            else
                return type.Namespace + "." + typeName;
        }
    }
}