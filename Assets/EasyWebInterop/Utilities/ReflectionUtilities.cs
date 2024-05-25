using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

namespace Nahoum.EasyWebInterop
{
    internal static class ReflectionUtilities
    {
        /// <summary>
        /// Check if a delegate is an async task
        /// Will return false if the method does not return a value (async only) or if it doesn't return a Task or Task<T>
        /// </summary>
        internal static bool IsDelegateAsyncTask(Delegate d)
        {
            bool hasReturnType = d.Method.ReturnType != typeof(void);
            bool isAsync = d.Method.GetAttribute<AsyncStateMachineAttribute>() != null;

            // If the method does not return a value, it cannot be a task
            if (!hasReturnType || !isAsync)
                return false;

            // Case Task<T>
            if (d.Method.ReturnType.IsGenericType && d.Method.ReturnType.GetGenericTypeDefinition() == typeof(System.Threading.Tasks.Task<>))
                return true;
            // Case Task
            else if (d.Method.ReturnType == typeof(System.Threading.Tasks.Task))
                return true;
            return false;
        }
    }
}