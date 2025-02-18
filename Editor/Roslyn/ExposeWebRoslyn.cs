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

            // If time exceeds 30ms, we should improve the performance of the checks
            if (sw.ElapsedMilliseconds > 30)
                UnityEngine.Debug.LogWarning($"Sanity checks took {sw.ElapsedMilliseconds}ms. To be improved. Check that not too large assemblies are being explored");
        }

        /// <summary>
        /// Ensure that all methods exposed from an interface are also ExposeWeb in all inheriting types
        /// </summary>
        public static void AssertAllExposedMethodsFromInterfaceAreExposed()
        {
            IReadOnlyCollection<Type> allExposedTypes = ExposeWebAttribute.GetAllTypesWithWebExposedMethods();

            // Cache the types available in the assemblies to avoid performance issues
            IReadOnlyCollection<Type> allTypes = ReflectionUtilities.GetAllAssembliesTypes();

            foreach (Type exposedType in allExposedTypes)
            {
                if (!exposedType.IsInterface)
                    continue;

                ISet<MethodInfo> exposedMethods = ExposeWebAttribute.GetExposedMethods(exposedType);

                foreach (var method in exposedMethods)
                {
                    AssertMethodImplementedInAllInheritingTypes(method, ref allTypes);
                }
            }
        }

        /// <summary>
        /// Given a method declared in an interface, ensures that it is well implemented in all implementing types, and that the implementation has the needed [ExposeWeb] attribute
        /// </summary>
        private static void AssertMethodImplementedInAllInheritingTypes(MethodInfo methodFromInterface, ref IReadOnlyCollection<Type> comparisonTypes)
        {
            // Check method is part of an interface, otherwise skip
            if (!methodFromInterface.ReflectedType.IsInterface)
                return;

            Type interfaceType = methodFromInterface.ReflectedType;

            // Check if the method is implemented in all inheriting types
            foreach (Type impl in comparisonTypes)
            {
                // Skip if the type does not implement the interface
                if (!interfaceType.IsAssignableFrom(impl) || impl.IsInterface)
                    continue;

                // Get the methods of the interface that are implemented in the type
                var interfaceMethods = impl.GetInterfaceMap(interfaceType).InterfaceMethods;

                // Ensure that if the interface method has the ExposeWebAttribute, the implementing method also has it
                if (ContainsMethod(methodFromInterface, interfaceMethods, out int index))
                {
                    MethodInfo implementingMethod = impl.GetInterfaceMap(interfaceType).TargetMethods[index];
                    UnityEngine.Debug.Log(implementingMethod.Name);
                    if (!ExposeWebAttribute.HasWebExposeAttribute(implementingMethod, out ExposeWebAttribute attr)){
UnityEngine.Debug.Log(impl + " "+interfaceType);
                        throw new Exception($"Exposed to the web method {implementingMethod.Name} in {implementingMethod.ReflectedType} is implemented from interface {interfaceType} but doesn't have the [ExposeWeb] attribute. Please add the [ExposeWeb] attribute to the method {methodFromInterface.Name} in {impl}");
                    }
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