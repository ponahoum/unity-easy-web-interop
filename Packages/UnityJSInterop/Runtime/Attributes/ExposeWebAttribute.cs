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
        // This way the reflection is only done once
        readonly static Dictionary<Type, Dictionary<MethodInfo, ExposeWebAttribute>> staticExposedMethodsCache = new Dictionary<Type, Dictionary<MethodInfo, ExposeWebAttribute>>();
        readonly static Dictionary<Type, Dictionary<MethodInfo, ExposeWebAttribute>> instanceExposedMethodsCache = new Dictionary<Type, Dictionary<MethodInfo, ExposeWebAttribute>>();
        
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
                var instanceExposes = GetExposedInstanceMethods(targetType);
                var staticExposed = GetExposedStaticMethods(targetType);

                if (instanceExposes.Count > 0 || staticExposed.Count > 0)
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
        /// Get all exposed methods on an instance
        /// Will only return the instance methods, not the static ones
        /// </summary>
        internal static Dictionary<MethodInfo, ExposeWebAttribute> GetExposedInstanceMethods(Type targetType){
            if(instanceExposedMethodsCache.ContainsKey(targetType))
                return instanceExposedMethodsCache[targetType];
            var result = GetExposedMethods(targetType, BindingFlags.Instance | BindingFlags.Public);
            instanceExposedMethodsCache.Add(targetType, result);
            return result;
        }

        /// <summary>
        /// Returns all the static methods with the ExposeWebAttribute of a given type
        /// </summary>
        internal static Dictionary<MethodInfo, ExposeWebAttribute> GetExposedStaticMethods(Type targetType){
            if(staticExposedMethodsCache.ContainsKey(targetType))
                return staticExposedMethodsCache[targetType];
            var result = GetExposedMethods(targetType, BindingFlags.Static | BindingFlags.Public);
            staticExposedMethodsCache.Add(targetType, result);
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