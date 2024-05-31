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
                var exposedStaticMethods = ExposeWebAttribute.GetExposedStaticMethods(exposedType);

                // Get all the static and public methods
                foreach (var method in exposedStaticMethods)
                {
                    MethodInfo staticMethod = method.Key;
                    Delegate del = ReflectionUtilities.CreateDelegate(staticMethod, null);
                    MethodsRegistry.RegisterMethod(NamingUtility.GetMethodJSPath(method.Key), del);
                }
            }
        }

        /// <summary>
        /// Register a service to a JS target
        /// </summary>
        internal static void RegisterSubService(IntPtr targetId, object instance)
        {
            var exposedMethods = ExposeWebAttribute.GetExposedInstanceMethods(instance.GetType());
            var staticMethods = ExposeWebAttribute.GetExposedStaticMethods(instance.GetType());

            // Inject instance methods
            foreach (var methodWithExposeWeb in exposedMethods)
                InjectMethodIntoTarget(methodWithExposeWeb.Key, instance, targetId);

            // Inject static methods
            foreach (var methodWithExposeWeb in staticMethods)
                InjectMethodIntoTarget(methodWithExposeWeb.Key, null, targetId);
        }


        /// <summary>
        /// Inject a method in a JS object of if targetId. This is typically used to populate all methods of instance of returned object
        /// </summary>
        private static void InjectMethodIntoTarget(MethodInfo method, object instance, IntPtr targetId)
        {
            string[] servicePath = new string[] { method.Name };
            Delegate del = ReflectionUtilities.CreateDelegate(method, method.IsStatic ? null : instance);
            MethodsRegistry.RegisterMethod(servicePath, del, targetId.ToInt32());
        }
    }
}