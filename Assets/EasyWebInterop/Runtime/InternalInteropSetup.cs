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

            // Register task completed
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<IntPtr, IntPtr>>(IsTaskCompleted), nameof(IsTaskCompleted), "ii");

            // Register get task result
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<IntPtr, IntPtr>>(GetTaskResult), nameof(GetTaskResult), "ii");
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
        /// Tells if a task is completed
        /// </summary>
        [MonoPInvokeCallback]
        static IntPtr IsTaskCompleted(IntPtr taskPtr)
        {
            object task = GCUtils.GetManagedObjectFromPtr(taskPtr);
            if (task is Task asTask)
                return GCUtils.NewManagedObject(asTask.IsCompleted);
            throw new Exception("The object is not a task");
        }

        /// <summary>
        /// Gets the result of a task
        /// Only call this if the task is completed
        /// </summary>
        [MonoPInvokeCallback]
        static IntPtr GetTaskResult(IntPtr taskPtr)
        {
            object task = GCUtils.GetManagedObjectFromPtr(taskPtr);

            if (task is Task asTask && asTask.IsCompleted)
                return GCUtils.NewManagedObject(asTask.GetType().GetProperty("Result").GetValue(asTask));
            throw new Exception("The object is not a task or the task is not completed");
        }

        /// <summary>
        /// Register a task completion callback with a managed action
        /// </summary>
        [MonoPInvokeCallback]
        static void OnTaskComplete(Action<IntPtr> onCompleted)
        {
            // TODO

        }

        // public static void RegisterGetActionStringFromPtr()
        // {
        //     [MonoPInvokeCallback]
        //     static IntPtr RegisterGetActionStringFromPtr_internal(IntPtr _, IntPtr actionAsPtr)
        //     {
        //         VI targetAct = Marshal.GetDelegateForFunctionPointer<VI>(actionAsPtr);
        //         Action<string> targetActAsActionString = (string value) =>
        //         {
        //             targetAct.Invoke(NewManagedObject(value));
        //         };
        //         return NewManagedObject(targetActAsActionString);
        //     }

        //     IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate<III>(RegisterGetActionStringFromPtr_internal);
        //     RegisterMethodInRegistry(registryCallPtr, "GetActionStringFromPtr", "iii");
        // }
    }
}