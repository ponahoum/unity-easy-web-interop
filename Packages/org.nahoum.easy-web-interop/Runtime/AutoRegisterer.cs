using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace Nahoum.UnityJSInterop
{
    /// <summary>
    /// Helps auto registering all the methods with the ExposeWebAttribute
    /// Will auto register all static methods with the ExposeWebAttribute
    /// Will also expose some constructors of Delegate types
    /// </summary>
    [Preserve]
    public static class AutoRegister
    {
        internal static void Setup()
        {
            RegisterAllStaticMethods();
        }

        private static void RegisterAllStaticMethods()
        {
            IReadOnlyCollection<Type> allExposedTypes = ExposeWebAttribute.GetAllTypesWithWebExposedMethods();
            foreach (Type exposedType in allExposedTypes)
            {
                ISet<MethodInfo> exposedMethods = ExposeWebAttribute.GetExposedMethods(exposedType);

                foreach (MethodInfo method in exposedMethods)
                {
                    // Register static methods
                    if (method.IsStatic)
                    {
                        MethodInfo staticMethod = method;
                        Delegate del = ReflectionUtilities.CreateDelegate(staticMethod, null);
                        MethodsRegistry.RegisterMethod(NamingUtility.GetMethodJSPath(method), del);
                    }

                    // Also handles the registering of delegates constructors
                    if (ReflectionUtilities.MethodHasReturnlessDelegateParameter(method, out IReadOnlyList<ParameterInfo> delegateParameters))
                    {
                        foreach (ParameterInfo parameter in delegateParameters)
                            InternalInteropSetup.TryRegisterDelegateConstructor(parameter.ParameterType);
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