using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;

namespace Nahoum.UnityJSInterop
{
    public class ExposeWebRoslyn
    {
        /// <summary>
        /// A collection of sanity checks to be run in the editor to ensure that the usage of ExposeWebAttribute is correct
        /// Will be replaced by a Roslyn analyzer in the future
        /// </summary>
        [InitializeOnLoadMethod]
        public static void ExecuteSanityChecks()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            AssertAllExposedMethodsFromInterfaceAreExposed();
            sw.Stop();
            UnityEngine.Debug.Log($"Sanity checks took {sw.ElapsedMilliseconds}ms. To be improved.");
        }

        /// <summary>
        /// Ensure that all methods exposed from an interface are also ExposeWeb in all inheriting types
        /// </summary>
        public static void AssertAllExposedMethodsFromInterfaceAreExposed()
        {
            IReadOnlyCollection<Type> allExposedTypes = ExposeWebAttribute.GetAllTypesWithWebExposedMethods();
            foreach (Type exposedType in allExposedTypes)
            {
                if (!exposedType.IsInterface)
                    continue;

                ISet<MethodInfo> exposedMethods = ExposeWebAttribute.GetExposedMethods(exposedType);

                foreach (var method in exposedMethods)
                {
                    AssertMethodImplementedInAllInheritingTypes(method);
                }
            }
        }
        private static void AssertMethodImplementedInAllInheritingTypes(MethodInfo method)
        {
            // Check method is part of an interface, otherwise skip
            if (!method.DeclaringType.IsInterface)
                return;

            Type interfaceType = method.DeclaringType;

            // If it is an interface, we need to gather all type implementing this interface
            IReadOnlyCollection<Type> allTypes = ReflectionUtilities.GetAllAssembliesTypes();

            // Check if the method is implemented in all inheriting types
            foreach (Type impl in allTypes)
            {
                // Skip if the type does not implement the interface
                if (!interfaceType.IsAssignableFrom(impl) || impl.IsInterface)
                    continue;

                // Get the methods of the interface that are implemented in the type
                var interfaceMethods = impl.GetInterfaceMap(interfaceType).InterfaceMethods;

                // Ensure that if the interface method has the ExposeWebAttribute, the implementing method also has it
                if (ContainsMethod(method, interfaceMethods, out int index))
                {
                    MethodInfo implementingMethod = impl.GetInterfaceMap(interfaceType).TargetMethods[index];
                    if (!ExposeWebAttribute.HasWebExposeAttribute(implementingMethod, out ExposeWebAttribute attr))
                        throw new Exception($"Exposed to the web method {implementingMethod.Name} in {implementingMethod.DeclaringType} is implemented from interface {interfaceType} but doesn't have the [ExposeWeb] attribute. Please add the [ExposeWeb] attribute to the method {method.Name} in {impl}");
                }
            }
        }

        private static bool ContainsMethod(MethodInfo method, MethodInfo[] methods, out int index)
        {
            index = -1;
            foreach (MethodInfo m in methods)
            {
                index++;
                if (m.Name == method.Name)
                    return true;
            }
            return false;
        }
    }
}