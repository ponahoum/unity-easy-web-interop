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
        internal static void Setup()
        {
            RegisterAllStaticMethds();
        }

        private static void RegisterAllStaticMethds()
        {
            var allExposedTypes = ExposeWebAttribute.GetAllTypesWithWebExposeMethods();
            foreach (var exposedType in allExposedTypes)
            {
                System.Collections.Generic.ISet<MethodInfo> exposedStaticMethods = ExposeWebAttribute.GetExposedMethods(exposedType);

                // Get all the static and public methods
                foreach (MethodInfo method in exposedStaticMethods)
                {
                    if (method.IsStatic)
                    {
                        MethodInfo staticMethod = method;
                        Delegate del = ReflectionUtilities.CreateDelegate(staticMethod, null);
                        MethodsRegistry.RegisterMethod(NamingUtility.GetMethodJSPath(method), del);
                    }
                }
            }
        }

        /// <summary>
        /// Register a service to a JS target
        /// </summary>
        internal static void RegisterSubService(IntPtr targetId, object instance)
        {
            var exposedMethods = ExposeWebAttribute.GetExposedMethods(instance.GetType());

            // Inject instance methods
            foreach (MethodInfo methodWithExposeWeb in exposedMethods)
            {
                MethodInfo method = methodWithExposeWeb;
                string[] servicePath = new string[] { method.Name };
                Delegate del = ReflectionUtilities.CreateDelegate(method, method.IsStatic ? null : instance);
                MethodsRegistry.RegisterMethod(servicePath, del, targetId.ToInt32());
            }
        }
    }
}