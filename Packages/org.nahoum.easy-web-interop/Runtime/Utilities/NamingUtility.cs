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
    }
}