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
            string[] namespaceSplit = hasNamespace ? namespaceName.Split('.') : new string[0];
            string className = method.DeclaringType.Name;
            string methodName = method.Name;

            // Add aggregate path (concat namespace split and classname and method name)
            string[] res = new string[namespaceSplit.Length + 3];
            res[0] = staticPrefix;
            for (int i = 0; i < namespaceSplit.Length; i++)
            {
                res[i + 1] = namespaceSplit[i];
            }
            res[namespaceSplit.Length + 1] = className;
            res[namespaceSplit.Length + 2] = methodName;
            return res;

        }

    }
}