using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Nahoum.UnityJSInterop.Editor
{
    internal static class TypescriptGenerationUtilities
    {
        /// <summary>
        /// Returns all types we should generate a type description for
        /// </summary>
        internal static ISet<Type> GetTypesToGenerateTypesFileFrom(bool excludeTestsAssemblies = false)
        {
            ISet<Type> typesToGenerate = new HashSet<Type>();

            bool TryAdd(Type type)
            {
                // If void, don't add it
                if (type == typeof(void))
                    return false;

                // Handle case of task, which is the only case where we only care about the return type
                if (ReflectionUtilities.IsTypeTask(type, out bool hasReturnValue, out Type taskReturnType))
                {
                    if (hasReturnValue)
                        return TryAdd(taskReturnType);
                    else
                        return false;
                }

                // Add the type
                return typesToGenerate.Add(type);
            }

            // Gather all types with exposed methods
            IReadOnlyCollection<Type> allTypesExposingMethods = ExposeWebAttribute.GetAllTypesWithWebExposedMethods();

            foreach (Type exposedType in allTypesExposingMethods)
            {
                // Check if assembly contains nunit as dependency - Skip if it does
                if (excludeTestsAssemblies && IsTypeInTestAssembly(exposedType))
                    continue;

                // Adds the type to the namespace
                TryAdd(exposedType);

                // Get exposed methods for the type and add their return types and parameters
                ISet<MethodInfo> exposedMethods = ExposeWebAttribute.GetExposedMethods(exposedType);
                foreach (var method in exposedMethods)
                {
                    MethodInfo methodInfo = method;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    foreach (ParameterInfo parameter in parameters)
                        TryAdd(parameter.ParameterType);
                    TryAdd(methodInfo.ReturnType);
                }
            }
            return typesToGenerate;
        }

        /// <summary>
        /// Gets all the types from which a provided type directly inherits (first level inheritance)
        /// This can return a list of abstract classes, classes or interfaces
        /// </summary>
        internal static HashSet<Type> GetAllInheritingTypes(Type target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            // Retrieve all loaded assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Collect all types that directly inherit the target type
            HashSet<Type> inheritingTypes = new HashSet<Type>();
            foreach (var assembly in assemblies)
            {
                Type[] allTypesInAssembly = assembly.GetTypes();
                IEnumerable<Type> types = allTypesInAssembly.Where(type => target.BaseType == type || (type.IsInterface && target.GetInterfaces().Contains(type)));

                foreach (var type in types)
                    inheritingTypes.Add(type);
            }
            return inheritingTypes;
        }

        /// <summary>
        /// Tests if an assembly is a test assembly
        /// </summary>
        private static bool IsTestAssembly(Assembly assembly)
        {
            var dependencies = assembly.GetReferencedAssemblies();
            foreach (var dependency in dependencies)
            {
                if (dependency.FullName.ToLower().Contains("nunit"))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Tests if a type is in a test assembly
        /// </summary>
        private static bool IsTypeInTestAssembly(Type type)
        {
            return IsTestAssembly(type.Assembly);
        }

        /// <summary>
        /// Get all exposed methods for a type, sorted by static and instance methods
        /// Methods is here and not available at runtime because the sorting is not required at runtime
        /// </summary>
        internal static void GetExposedMethodsSorted(Type targetType, out ISet<MethodInfo> staticMethods, out ISet<MethodInfo> instanceMethods)
        {
            ISet<MethodInfo> exposedMethods = ExposeWebAttribute.GetExposedMethods(targetType);
            staticMethods = new HashSet<MethodInfo>();
            instanceMethods = new HashSet<MethodInfo>();
            foreach (var method in exposedMethods)
            {
                if (method.IsStatic)
                    staticMethods.Add(method);
                else
                    instanceMethods.Add(method);
            }
        }
    }
}