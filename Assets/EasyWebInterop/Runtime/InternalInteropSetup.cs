using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Nahoum.EasyWebInterop.DyncallSignature;

namespace Nahoum.EasyWebInterop
{
    public static class InternalInteropSetup
    {
        [DllImport("__Internal")]
        internal static extern void RegisterStaticMethodInternalRegistry(IntPtr methodPtr, string functionName, string functionSignature);

        [DllImport("__Internal")]
        internal static extern void Setup(IntPtr getIntPtrValueMethodPtr);

        /// <summary>
        /// Setup the service register with the default method to get the string representation of an object
        /// To be called on startup
        /// </summary>
        public static void Setup()
        {
            Setup(Marshal.GetFunctionPointerForDelegate<II>(GetSerializedValueFromPtr));

            // Register get double from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<double, IntPtr>>(GetManagedDoubleFromPtr), nameof(GetManagedDoubleFromPtr), "id");

            // Register get double array from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<double[], int, IntPtr>>(GetManagedDoubleArrayFromPtr), nameof(GetManagedDoubleArrayFromPtr), "iii");

            // Register get element at index from ptr array
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<IntPtr, int, IntPtr>>(GetManagedElementAtIndexFromManagedPtrArray), nameof(GetManagedElementAtIndexFromManagedPtrArray), "iii");

            // Register wait for task to complete
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Action<IntPtr, IntPtr>>(WaitForTaskToComplete), nameof(WaitForTaskToComplete), "vii");
        }

        /// <summary>
        /// Given an GCHandle ptr, return the string representation of the object as json
        /// The returned result is a pointer to an ansi encoded string
        /// </summary>
        [MonoPInvokeCallback]
        static IntPtr GetSerializedValueFromPtr(IntPtr targetObject)
        {
            // Gather the object from the GCHandle
            object obj = GCUtils.GetManagedObjectFromPtr(targetObject);

            // Serialize the object
            string json = ObjectSerializer.ToJson(obj);

            // Convert to IntPtr using Ansi marshalling
            IntPtr ptr = Marshal.StringToHGlobalAnsi(json);
            return ptr;
        }

        [MonoPInvokeCallback]
        static IntPtr GetManagedDoubleFromPtr([MarshalAs(UnmanagedType.R8)] double managedDouble) => GCUtils.NewManagedObject(managedDouble);

        [MonoPInvokeCallback]
        static IntPtr GetManagedDoubleArrayFromPtr([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] managedArray, int size) => GCUtils.NewManagedObject(managedArray);

        /// <summary>
        /// Given an array ptr and an index, return the element at the index
        /// </summary>
        /// <param name="arrayPtr">The pointer to the managed array</param>
        [MonoPInvokeCallback]
        static IntPtr GetManagedElementAtIndexFromManagedPtrArray(IntPtr arrayPtr, int index)
        {
            // Get object from ptr
            object array = GCUtils.GetManagedObjectFromPtr(arrayPtr);

            // Check array is array
            if (!array.GetType().IsArray)
                throw new Exception("The object is not an array");

            Array asArray = (Array)array;
            if (index >= asArray.Length)
                throw new IndexOutOfRangeException("Index out of range");

            // Get element at index
            return GCUtils.NewManagedObject(asArray.GetValue(index));
        }

        /// <summary>
        /// Register a task completion callback with a managed action
        /// </summary>
        [MonoPInvokeCallback]
        static async void WaitForTaskToComplete(IntPtr taskPtr, IntPtr onCompletedActionCallback)
        {
            // Convert the managed task inptr to task
            object task = GCUtils.GetManagedObjectFromPtr(taskPtr);

            // Check the object is a task
            if (!ReflectionUtilities.IsTask(task, out bool hasReturnValue, out Task asTask))
                throw new Exception("The object is not a task");

            // Get the callback Action<IntPtr> from the ptr
            VI onCompleted = Marshal.GetDelegateForFunctionPointer<VI>(onCompletedActionCallback);

            // Wait for the task to complete
            while (!asTask.IsCompleted)
            {
                await Task.Yield();
            }

            // If task is faulted, return undefined encoded pointer
            if (asTask.IsFaulted)
            {
                UnityEngine.Debug.LogError(asTask.Exception);
                onCompleted.Invoke(IntPtrExtension.Exception);
                return;
            }

            // If has return type, return the wrapped object
            if (hasReturnValue)
            {
                var result = ReflectionUtilities.GetTaskResult(asTask);
                onCompleted.Invoke(GCUtils.NewManagedObject(result));
            }

            // Otherwise return undefined (-1 which means void)
            else
                onCompleted.Invoke(IntPtrExtension.Undefined);
        }

        [MonoPInvokeCallback]
        static IntPtr RegisterGetActionIntPtrFromJsPtr(IntPtr actionIntPtrAsPtr)
        {
            VI targetAct = Marshal.GetDelegateForFunctionPointer<VI>(actionIntPtrAsPtr);
            return GCUtils.NewManagedObject(targetAct);
        }
    }
}