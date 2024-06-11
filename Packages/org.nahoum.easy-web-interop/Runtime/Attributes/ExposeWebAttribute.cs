using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine.Scripting;

namespace Nahoum.UnityJSInterop
{
    // Expose web attribute on methods and classes
    [AttributeUsage(AttributeTargets.Method)]
    public class ExposeWebAttribute : PreserveAttribute
    {
        // Called once when the class is loaded
        static HashSet<Type> exposedTypesCache = null;

        // For efficiency, we cache the exposed methods for each type, separated by static and instance methods
        // We also have a dictionary with all the exposed methods grouped by type (static and instance together)
        // This way the reflection is only done once
        readonly static Dictionary<Type, Dictionary<MethodInfo, ExposeWebAttribute>> allExposedMethodsCache = new Dictionary<Type, Dictionary<MethodInfo, ExposeWebAttribute>>();

        /// <summary>
        /// Returns a list of all the possibles types containing methods with the ExposeWebAttribute appearing on them
        /// Wether those methods are static or not
        /// </summary>
        internal static IReadOnlyCollection<Type> GetAllTypesWithWebExposeMethods()
        {

            if (exposedTypesCache != null)
                return exposedTypesCache;

            exposedTypesCache = new HashSet<Type>();

            // Cache all the types with exposed methods
            List<Type> availableTypes = ReflectionUtilities.GetAllAssembliesTypes();

            // Get all the types in the assembly
            foreach (Type targetType in availableTypes)
            {
                var exposedMethods = GetExposedMethods(targetType);

                // Skip if no exposed methods
                if (exposedMethods.Count == 0)
                    continue;

                // Protect against all non supported types
                if (targetType.IsGenericTypeDefinition)
                    throw new Exception($"Cannot expose generic type {targetType}. Generic types are not supported.");

                if (targetType.IsInterface)
                    throw new Exception($"Cannot expose interface {targetType}. Interfaces are not supported.");

                if (targetType.IsAbstract)
                    throw new Exception($"Cannot expose abstract class {targetType}. Abstract classes are not supported.");

                if (!targetType.IsClass)
                    throw new Exception($"Cannot expose non class type {targetType}. Only classes are supported.");

                exposedTypesCache.Add(targetType);
            }
            return exposedTypesCache;
        }

        /// <summary>
        /// Tells if a type has exposed methods
        /// Wether those methods are static or not
        /// </summary>
        internal static bool HasExposedMethods(Type targetType) => exposedTypesCache.Contains(targetType);

        /// <summary>
        /// Get all the exposed methods of a type, wether they are static or not
        /// </summary>
        internal static Dictionary<MethodInfo, ExposeWebAttribute> GetExposedMethods(Type targetType)
        {
            if (allExposedMethodsCache.ContainsKey(targetType))
                return allExposedMethodsCache[targetType];
            var result = GetExposedMethods(targetType, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            allExposedMethodsCache.Add(targetType, result);
            return result;
        }

        /// <summary>
        /// Given a type, return all the methods with the ExposeWebAttribute
        /// Wether those methods are static or not
        /// </summary>
        private static Dictionary<MethodInfo, ExposeWebAttribute> GetExposedMethods(Type targetType, BindingFlags flags)
        {
            var result = new Dictionary<MethodInfo, ExposeWebAttribute>();

            // Get all static methods
            MethodInfo[] methods = targetType.GetMethods(flags);
            foreach (MethodInfo method in methods)
            {
                if (HasWebExposeAttribute(method, out ExposeWebAttribute attr))
                    result.Add(method, attr);
            }

            return result;
        }

        /// <summary>
        /// Check if a method has the ExposeWebAttribute
        /// Will return the attribute if it has it
        /// </summary>
        internal static bool HasWebExposeAttribute(MethodInfo method, out ExposeWebAttribute attribute)
        {
            attribute = method.GetCustomAttribute<ExposeWebAttribute>();

            if(attribute == null)
                return false;
            
            if(!method.IsPublic)
                throw new Exception($"Method {method.Name} in {method.DeclaringType} is not public. Only public methods can be exposed.");

            if(method.IsGenericMethodDefinition)
                throw new Exception($"Method {method.Name} in {method.DeclaringType} is a generic method. Generic methods are not supported and cannot be exposed.");

            return true;
        }
    }
}