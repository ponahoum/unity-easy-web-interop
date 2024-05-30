using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine.Scripting;

namespace Nahoum.EasyWebInterop
{
    // Expose web attribute on methods and classes
    [AttributeUsage(AttributeTargets.Method)]
    public class ExposeWebAttribute : PreserveAttribute
    {

        public ExposeWebAttribute()
        {
        }

        // Called once when the class is loaded
        static HashSet<Type> exposedTypesCache = new HashSet<Type>();
        static ExposeWebAttribute()
        {
            // Setup the EasyWebInterop
            EasyWebInterop.Setup();

            // Cache all the types with exposed methods
            List<Type> availableTypes = ReflectionUtilities.GetAllAssembliesTypes();
            exposedTypesCache = new HashSet<Type>();

            // Get all the types in the assembly
            foreach (Type targetType in availableTypes)
            {
                var instanceExposes = GetExposedInstanceMethods(targetType);
                var staticExposed = GetExposedStaticMethods(targetType);

                if (instanceExposes.Count > 0 || staticExposed.Count > 0)
                    exposedTypesCache.Add(targetType);
            }

        }

        /// <summary>
        /// Returns a list of all the possibles types containing methods with the ExposeWebAttribute appearing on them
        /// Wether those methods are static or not
        /// </summary>
        internal static IReadOnlyCollection<Type> GetAllTypesWithWebExposeMethods() => exposedTypesCache;

        /// <summary>
        /// Tells if a type has exposed methods
        /// Wether those methods are static or not
        /// </summary>
        internal static bool HasExposedMethods(Type targetType) => exposedTypesCache.Contains(targetType);

        /// <summary>
        /// Get all exposed methods on an instance
        /// Will only return the instance methods, not the static ones
        /// </summary>
        internal static Dictionary<MethodInfo, ExposeWebAttribute> GetExposedInstanceMethods(Type targetType) => GetExposedMethods(targetType, BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// Returns all the static methods with the ExposeWebAttribute of a given type
        /// </summary>
        internal static Dictionary<MethodInfo, ExposeWebAttribute> GetExposedStaticMethods(Type targetType) => GetExposedMethods(targetType, BindingFlags.Static | BindingFlags.Public);

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