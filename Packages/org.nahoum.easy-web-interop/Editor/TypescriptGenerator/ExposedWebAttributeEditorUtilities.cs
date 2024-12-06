using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Nahoum.UnityJSInterop.Editor
{
    internal static class ExposedWebAttributeEditorUtilities
    {
        /// <summary>
        /// Get all types that are exposed by the ExposeWeb attribute, ordered by namespace
        /// Also includes all types that are used as parameters or return types in the exposed methods
        /// This allows to easily generate typescript definitions for all required types
        /// </summary>
        internal static Dictionary<NamespaceDescriptor, HashSet<Type>> GetExposedTypesByNamespace(bool excludeTestsAssemblies = false)
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

                // Handle array case
                if (type.IsArray)
                {
                    return TryAddTypeToNamespace(type.GetElementType());
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
                    UnityEngine.Debug.Log("Detected dependent NUnit assembly, skipping: " + exposedType.Assembly.FullName);
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
            return typesByNamespace;
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