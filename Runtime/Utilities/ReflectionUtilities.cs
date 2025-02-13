using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Nahoum.UnityJSInterop
{
    internal static class ReflectionUtilities
    {
        /// <summary>
        /// Check if a delegate return type is task
        /// Will return false if the method does not return doesn't return a Task or Task<T>
        /// </summary>
        internal static bool DelegateReturnsTask(Delegate d)
        {
            bool hasReturnType = d.Method.ReturnType != typeof(void);

            // If the method does not return a value, it cannot be a task
            if (!hasReturnType)
                return false;

            // Case Task<T>
            if (d.Method.ReturnType.IsGenericType && d.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                return true;
            // Case Task
            else if (d.Method.ReturnType == typeof(Task))
                return true;
            return false;
        }

        /// <summary>
        /// Check if a type is a delegate
        /// If it is, it will return  true and the return type and the parameters types
        /// If not, will return false and null for the return type and parameters types
        /// </summary>
        internal static bool TypeIsDelegate(Type t, out Type returnType, out Type[] parametersTypes)
        {
            bool isDelegate = typeof(Delegate).IsAssignableFrom(t);
            returnType = null;
            parametersTypes = null;

            // Not sure of this part, might need to refine...
            if (isDelegate)
            {
                var methodInfo = t.GetMethod("Invoke");
                returnType = methodInfo.ReturnType;
                parametersTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            }
            return isDelegate;
        }

        /// <summary>
        /// Tells if the provided object is a Task or Task<T>
        /// </summary>
        /// <param name="hasReturnValue">True if the object is a generic Task<T> (T being the return type). False if the object is only a task</param>
        internal static bool IsTask(object objectToCheck, out bool hasReturnValue, out Task asTask)
        {
            hasReturnValue = false;
            asTask = null;

            // Check the object is a task
            if (objectToCheck is Task castAsTask)
                asTask = castAsTask;
            else
                return false;

            // Check if the task is a generic task (async Task == Task<VoidTaskResult>, so an async task will still be generic)
            Type objectType = objectToCheck.GetType();
            return IsTypeTask(objectType, out hasReturnValue, out _);
        }

        /// <summary>
        /// Tells if the provided type is a Task or Task<T>
        /// If there is a return value, hasReturnValue will be true and the return type will be in the out parameter returnType
        /// </summary>
        internal static bool IsTypeTask(Type type, out bool hasReturnValue, out Type taskReturnType)
        {
            hasReturnValue = false;
            taskReturnType = null;

            // Check if is regular Task
            if (type == typeof(Task))
                return true;
            // Check if the task is a generic task (async Task == Task<VoidTaskResult>, so an async task will still be generic)
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // The only case in which we typically have no return type but still the task is generic is when the task is async
                // C# wraps the "Task" return type in a Task<VoidTaskResult> when the method is async
                // Hence if we're in this case, we consider we have no return type
                bool returnTypeIsVoid = type.GetGenericArguments()[0].Name == "VoidTaskResult";
                hasReturnValue = !returnTypeIsVoid;
                taskReturnType = type.GetGenericArguments()[0];
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Fake methods to preserve the type of a Task<T> when using reflection
        /// Otherwise it's stripped by the IL2CPP compiler
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        private static T PreserveTaskT<T>(Task<T> task)
        {
            return task.Result;
        }

        /// <summary>
        /// Returns the result of a task
        /// </summary>
        internal static object GetTaskResult(Task task)
        {
            if (task.IsCompleted)
                return task.GetType().GetProperty("Result").GetValue(task);
            throw new Exception("The task is not completed");
        }

        /// <summary>
        /// Create a delegate from a methodinfo
        /// </summary>
        internal static Delegate CreateDelegate(MethodInfo methodInfo, object target)
        {
            Func<Type[], Type> getType;
            bool isAction = methodInfo.ReturnType.Equals(typeof(void));
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);

            if (isAction)
            {
                getType = Expression.GetActionType;
            }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] { methodInfo.ReturnType });
            }

            if (methodInfo.IsStatic)
                return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);

            return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
        }

        /// <summary>
        /// Given a method, tells if it has at least one parameter that is a delegate (like and Action or Action<string> etc.) with no return type
        /// </summary>
        /// <param name="method">The method to inspect</param>
        /// <param name="delegateParameterTypes">The list of parameters of the method that are delegates</param>
        /// <returns></returns>
        internal static bool MethodHasReturnlessDelegateParameter(MethodInfo method, out IReadOnlyList<ParameterInfo> delegateParameterTypes)
        {
            delegateParameterTypes = null;
            var parameters = method.GetParameters();

            // Skip immediatly if no parameters
            if (parameters.Length == 0)
                return false;

            List<ParameterInfo> result = new List<ParameterInfo>();
            delegateParameterTypes = result;

            // Go through each parameter and check if it's a delegate
            foreach (ParameterInfo parameter in parameters)
            {
                Type parameterType = parameter.ParameterType;
                if (typeof(Delegate).IsAssignableFrom(parameterType) && parameterType.GetMethod("Invoke").ReturnType == typeof(void))
                {
                    result.Add(parameter);
                }
            }

            return result.Count > 0;
        }

        /// <summary>
        /// Returns a gigantic list of all the types in all the available assemblies
        /// </summary>
        internal static IReadOnlyCollection<Type> GetAllAssembliesTypes(bool excludeCommonAssemblies = true)
        {
            HashSet<Type> types = new HashSet<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                // Exclude common assemblies that are no utility to the user
                if (excludeCommonAssemblies && excludeNamespace.Any(n => assembly.FullName.StartsWith(n)))
                    continue;

                // Get all the types in the assembly
                Type[] allTypes = assembly.GetTypes();
                foreach (Type targetType in allTypes)
                {
                    types.Add(targetType);
                }
            }
            return types;
        }

        /// <summary>
        /// A collection of namespaces to exclude from the GetAllAssembliesTypes method. This is because those namespaces are not relevant to the user and cause performance issues when exploring them
        /// </summary>
        private static readonly HashSet<string> excludeNamespace = new HashSet<string>()
        {
            "System",
            "UnityEngine",
            "UnityEditor",
            "Microsoft",
            "Mono",
            "JetBrains",
            "Roslyn",
            "NUnit",
            "Newtonsoft",
            "netstandard",
            "Unity",
            "nunit",
            "I18N",
            "mscorlib",
            "Bee",
            "ReportGeneratorMerged",
            "unityplastic",
            "Accessibility",
            "log4net"
        };
    }
}