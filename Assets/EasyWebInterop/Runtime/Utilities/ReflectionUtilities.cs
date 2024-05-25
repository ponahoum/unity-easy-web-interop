using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


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
            bool isAsync = d.Method.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Length > 0;

            // If the method does not return a value, it cannot be a task
            if (!hasReturnType || !isAsync)
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
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Task<>))
            {

                // The only case in which we typically have no return type but still the task is generic is when the task is async
                // C# wraps the "Task" return type in a Task<VoidTaskResult> when the method is async
                // Hence if we're in this case, we consider we have no return type
                bool returnTypeIsVoid = objectType.GetGenericArguments()[0].Name == "VoidTaskResult";
                hasReturnValue = !returnTypeIsVoid;
            }

            return true;
        }

        /// <summary>
        /// Fake methods to preserve the type of a Task<T> when using reflection
        /// Otherwise it's stripped by the IL2CPP compiler
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        private static T PreserveTaskT<T>(Task<T> task){
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
    }
}