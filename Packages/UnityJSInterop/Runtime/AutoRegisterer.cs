using System;
using System.Reflection;
using UnityEngine.Scripting;

namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// Helps auto registering all the methods with the ExposeWebAttribute
    /// Will auto register all static methods with the ExposeWebAttribute
    /// </summary>
    [Preserve]
    public static class AutoRegister
    {
        internal static void Setup(){
            RegisterAllStaticMethds();
        }

        private static void RegisterAllStaticMethds()
        {
            var allExposedTypes = ExposeWebAttribute.GetAllTypesWithWebExposeMethods();
            foreach (var exposedType in allExposedTypes)
            {
                var exposedStaticMethods = ExposeWebAttribute.GetExposedStaticMethods(exposedType);

                // Get all the static and public methods
                foreach (var method in exposedStaticMethods)
                {
                    MethodInfo staticMethod = method.Key;

                    // Method name is class name _ method name
                    string serviceName = exposedType.Name;
                    string[] servicePath = new string[] {  "static", serviceName, staticMethod.Name };
                    Delegate del = ReflectionUtilities.CreateDelegate(staticMethod, null);
                    MethodsRegistry.RegisterMethod(servicePath, del);
                }
            }
        }

        /// <summary>
        /// Register a service to a JS target
        /// </summary>
        internal static void RegisterSubService(IntPtr targetId, object instance)
        {
            var exposedMethods = ExposeWebAttribute.GetExposedInstanceMethods(instance.GetType());
            foreach (var methodWithExposeWeb in exposedMethods)
            {
                MethodInfo method = methodWithExposeWeb.Key;
                string[] servicePath = new string[] { method.Name };
                Delegate del = ReflectionUtilities.CreateDelegate(method, instance);
                MethodsRegistry.RegisterMethod(servicePath, del, targetId.ToInt32());
            }
        }
    }
}