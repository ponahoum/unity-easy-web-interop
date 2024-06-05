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
        internal static IReadOnlyCollection<Type> GetAllTypesWithWebExposeMethods(){
            
            if(exposedTypesCache != null)
                return exposedTypesCache;

            exposedTypesCache = new HashSet<Type>();

            // Cache all the types with exposed methods
            List<Type> availableTypes = ReflectionUtilities.GetAllAssembliesTypes();

            // Get all the types in the assembly
            foreach (Type targetType in availableTypes)
            {
                var exposedMethods= GetExposedMethods(targetType);

                // We only allow to expose methods on real classes
                // Those class can be generic, but the type must be specified
                if (exposedMethods.Count > 0 && !targetType.IsAbstract && !targetType.IsInterface && !targetType.IsGenericTypeDefinition && targetType.IsClass)
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
        internal static Dictionary<MethodInfo, ExposeWebAttribute> GetExposedMethods(Type targetType){
            if(allExposedMethodsCache.ContainsKey(targetType))
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
            return attribute != null;
        }
    }
}