using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Scripting;

namespace Nahoum.EasyWebInterop
{
    [AttributeUsage(AttributeTargets.Method), Preserve]
    public class ExposeWebAttribute : PreserveAttribute
    {
        public ExposeWebAttribute()
        {
        }
    }

    /// <summary>
    /// Helps auto registering all the methods with the ExposeWebAttribute
    /// Will auto register all static methods with the ExposeWebAttribute
    /// Will help register per-instance methods with the ExposeWebAttribute
    /// </summary>
    [Preserve]
    public static class AutoRegister
    {
        // Constructor
        static AutoRegister()
        {
            // Get all the assembly
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                // Get all the types in the assembly
                Type[] types = assembly.GetTypes();
                foreach (var className in types)
                {
                    // Get all the static and public methods
                    MethodInfo[] methods = className.GetMethods(BindingFlags.Static | BindingFlags.Public);
                    foreach (MethodInfo method in methods)
                    {
                        // Check if method static
                        // Get all the attributes in the method
                        object[] attributes = method.GetCustomAttributes(typeof(ExposeWebAttribute), true);
                        if (attributes.Length > 0)
                        {
                            // Method name is class name _ method name
                            string serviceName = className.Name;
                            string[] servicePath = new string[] { serviceName, method.Name };
                            Delegate del = ReflectionUtilities.CreateDelegate(method, null);
                            MethodsRegistry.RegisterMethod(servicePath, del);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Register a service instance
        /// </summary>
        /// <param name="serviceName">The name you want to give to your exposed service</param>
        /// <param name="instance">The instance of the service you wish to expose</param>

        public static void RegisterService(string serviceName, object instance)
        {
            // Get all instance methods
            MethodInfo[] methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                var attributes = method.GetCustomAttributes<ExposeWebAttribute>(true);
                if (attributes.Count() > 0)
                {
                    // Method name is class name _ method name
                    string[] servicePath = new string[] { serviceName, method.Name };
                    Delegate del = ReflectionUtilities.CreateDelegate(method, instance);
                    MethodsRegistry.RegisterMethod(servicePath, del);
                }
            }
        }
    }
}