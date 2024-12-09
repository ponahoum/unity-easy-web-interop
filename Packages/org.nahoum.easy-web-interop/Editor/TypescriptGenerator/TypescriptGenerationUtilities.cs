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
        /// Get all types that are exposed by the ExposeWeb attribute, ordered by namespace
        /// Also includes all types that are used as parameters or return types in the exposed methods
        /// This allows to easily generate typescript definitions for all required types
        /// </summary>
        internal static Dictionary<NamespaceDescriptor, HashSet<Type>> GetExposedTypesByNamespace(bool excludeTestsAssemblies = false, System.Type[] additionalTypesToGenerate = null)
        {
            // Gather all types with exposed methods
            IReadOnlyCollection<Type> allTypesExposingMethods = ExposeWebAttribute.GetAllTypesWithWebExposeMethods();

            // Now order types by namespace
            Dictionary<NamespaceDescriptor, HashSet<Type>> typesByNamespace = new Dictionary<NamespaceDescriptor, HashSet<Type>>();

            bool TryAddTypeToNamespace(Type type)
            {
                NamespaceDescriptor namespaceName = new NamespaceDescriptor(type);

                // If void, don't add it
                if (type == typeof(void))
                    return false;

                // Handle case of task, which is the only case where we only care about the return type
                if (ReflectionUtilities.IsTypeTask(type, out bool hasReturnValue, out Type taskReturnType))
                {
                    if (hasReturnValue)
                    {
                        return TryAddTypeToNamespace(taskReturnType);
                    }
                    else
                        return false;
                }

                // Add key to the dictionary if it doesn't exist
                if (!typesByNamespace.ContainsKey(namespaceName))
                    typesByNamespace.Add(namespaceName, new HashSet<Type>());

                // Classify the type as part of the namespace
                return typesByNamespace[namespaceName].Add(type);
            }

            // Add all types we'll need to generate in a sorted dictionary
            foreach (Type exposedType in allTypesExposingMethods)
            {
                // Check if assembly contains nunit as dependency
                if (excludeTestsAssemblies && IsTypeInTestAssembly(exposedType))
                {
                    UnityEngine.Debug.Log("Detected dependent NUnit assembly, skipping: " + exposedType.Assembly.FullName + " for generating typescript");
                    continue;
                }

                // Get the namespace of the type and add it to the dictionary
                TryAddTypeToNamespace(exposedType);

                // Get exposed methods
                Dictionary<MethodInfo, ExposeWebAttribute> exposedMethods = ExposeWebAttribute.GetExposedMethods(exposedType);
                foreach (var method in exposedMethods)
                {
                    MethodInfo methodInfo = method.Key;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    foreach (ParameterInfo parameter in parameters)
                        TryAddTypeToNamespace(parameter.ParameterType);
                    TryAddTypeToNamespace(methodInfo.ReturnType);

                }
            }

            // Add additional types to generate
            if (additionalTypesToGenerate != null)
            {
                foreach (Type type in additionalTypesToGenerate)
                {
                    TryAddTypeToNamespace(type);
                }
            }

            return typesByNamespace;
        }

        /// <summary>
        /// Returns all exposed types in a flat list - Includes all types that are used as parameters or return types in the exposed methods, in addition to the types with exposed methods
        /// This is useful for generating typescript definitions
        /// </summary>
        internal static HashSet<Type> GetExposedTypesFlatenned(bool excludeTestsAssemblies = false)
        {
            Dictionary<NamespaceDescriptor, HashSet<Type>> typesByNamespace = GetExposedTypesByNamespace(excludeTestsAssemblies);
            HashSet<Type> exposedTypes = new HashSet<Type>();
            foreach (var namespaceTypes in typesByNamespace.Values)
            {
                foreach (var type in namespaceTypes)
                {
                    exposedTypes.Add(type);
                }
            }
            return exposedTypes;
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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Collect all types that directly inherit the target type
            var inheritingTypes = new HashSet<Type>();
            foreach (var assembly in assemblies)
            {
                var allTypesInAssembly = assembly.GetTypes();
                IEnumerable<Type> types = allTypesInAssembly.Where(type => target.BaseType == type || (type.IsInterface && target.GetInterfaces().Contains(type)));

                foreach (var type in types)
                    inheritingTypes.Add(type);
            }
            return inheritingTypes;
        }

        /// <summary>
        /// Tests if an assembly is a test assembly
        /// </summary>
        internal static bool IsTestAssembly(Assembly assembly)
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
        internal static bool IsTypeInTestAssembly(Type type)
        {
            return IsTestAssembly(type.Assembly);
        }

        /// <summary>
        /// Get all exposed methods for a type, sorted by static and instance methods
        /// Methods is here and not available at runtime because the sorting is not required at runtime
        /// </summary>
        internal static void GetExposedMethodsSorted(Type targetType, out Dictionary<MethodInfo, ExposeWebAttribute> staticMethods, out Dictionary<MethodInfo, ExposeWebAttribute> instanceMethods)
        {
            Dictionary<MethodInfo, ExposeWebAttribute> exposedMethods = ExposeWebAttribute.GetExposedMethods(targetType);
            staticMethods = new Dictionary<MethodInfo, ExposeWebAttribute>();
            instanceMethods = new Dictionary<MethodInfo, ExposeWebAttribute>();
            foreach (var method in exposedMethods)
            {
                if (method.Key.IsStatic)
                    staticMethods.Add(method.Key, method.Value);
                else
                    instanceMethods.Add(method.Key, method.Value);
            }
        }
    }
}