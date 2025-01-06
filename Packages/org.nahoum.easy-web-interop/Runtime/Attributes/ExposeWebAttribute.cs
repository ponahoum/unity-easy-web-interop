using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine.Scripting;

namespace Nahoum.UnityJSInterop
{
    /// <summary>
    ///  Expose web attribute on methods and classes
    ///  Inherited because for example if we put in an a interface, we want to expose the methods in all inheriting classes from the interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Interface, Inherited = true)]
    public class ExposeWebAttribute : PreserveAttribute
    {
        // Called once when the class is loaded
        static HashSet<Type> exposedTypesCache = null;

        // An informative description of the method
        public string description { get; set; }

        // For efficiency, we cache the exposed methods for each method's containing type, separated by static and instance methods
        // We also have a dictionary with all the exposed methods grouped by type (static and instance together)
        // This way the reflection is only done once
        readonly static Dictionary<Type, HashSet<MethodInfo>> allExposedMethodsCache = new Dictionary<Type, HashSet<MethodInfo>>();

        /// <summary>
        /// Returns a list of all the possibles types containing methods with the ExposeWebAttribute appearing on them
        /// Wether those methods are static or not
        /// </summary>
        internal static IReadOnlyCollection<Type> GetAllTypesWithWebExposedMethods()
        {
            if (exposedTypesCache != null)
                return exposedTypesCache;

            exposedTypesCache = new HashSet<Type>();

            // Cache all the types with exposed methods
            IReadOnlyCollection<Type> availableTypes = ReflectionUtilities.GetAllAssembliesTypes();

            // Get all the types in the assembly
            foreach (Type targetType in availableTypes)
            {
                ISet<MethodInfo> exposedMethods = GetExposedMethods(targetType);

                // Skip if no exposed methods
                if (exposedMethods.Count == 0)
                    continue;

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
        internal static ISet<MethodInfo> GetExposedMethods(Type targetType)
        {
            if (allExposedMethodsCache.ContainsKey(targetType))
                return allExposedMethodsCache[targetType];

            HashSet<MethodInfo> result = GetExposedMethods(targetType, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            allExposedMethodsCache.Add(targetType, result);
            return result;
        }

        /// <summary>
        /// Given a type, return all the methods with the ExposeWebAttribute
        /// Wether those methods are static or not
        /// </summary>
        private static HashSet<MethodInfo> GetExposedMethods(Type targetType, BindingFlags flags)
        {
            HashSet<MethodInfo> result = new HashSet<MethodInfo>();

            // Get all methods with the ExposeWebAttribute directly in the type
            MethodInfo[] methods = targetType.GetMethods(flags);
            foreach (MethodInfo method in methods)
            {
                if (HasWebExposeAttribute(method, out ExposeWebAttribute attr))
                    result.Add(method);
            }

            return result;
        }

        /// <summary>
        /// Check if a method has the ExposeWebAttribute
        /// Will return the attribute if it has it
        /// </summary>
        internal static bool HasWebExposeAttribute(MethodInfo method, out ExposeWebAttribute attribute)
        {
            attribute = method.GetCustomAttribute<ExposeWebAttribute>(inherit: true);

            // If attribute is still null, try to get it from the method's containing class / type
            // BETA - Might not work in all cases
            if (attribute == null)
                attribute = method.DeclaringType.GetCustomAttribute<ExposeWebAttribute>(inherit: true);

            // At this point, if the attribute is still null, the method is not exposed
            if (attribute == null)
                return false;

            if (!method.IsPublic)
                throw new Exception($"Method {method.Name} in {method.DeclaringType} is not public. Only public methods can be exposed.");

            // Case the method is Generic like Method<T> or returns T like T Method()
            if (method.IsGenericMethod || method.ReturnType.IsGenericParameter)
                throw new Exception($"Method {method.Name} in {method.DeclaringType} is a generic method. Generic methods are not supported and cannot be exposed.");

            return true;
        }
    }
}