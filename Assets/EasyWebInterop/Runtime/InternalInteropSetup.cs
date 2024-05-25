using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Nahoum.EasyWebInterop.DyncallSignature;

namespace Nahoum.EasyWebInterop
{
    internal static class InternalInteropSetup
    {
        [DllImport("__Internal")]
        internal static extern void RegisterStaticMethodInternalRegistry(IntPtr methodPtr, string functionName, string functionSignature);

        [DllImport("__Internal")]
        internal static extern void Setup(IntPtr getIntPtrValueMethodPtr, IntPtr collectManagedPtrMethodPtr);

        /// <summary>
        /// Setup the service register with the default method to get the string representation of an object
        /// To be called on startup
        /// </summary>
        internal static void SetupInternal()
        {
            Setup(Marshal.GetFunctionPointerForDelegate<II>(GetSerializedValueFromManagedPtr), Marshal.GetFunctionPointerForDelegate<VI>(CollectManagedPtr));

            // Register get type from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<IntPtr, IntPtr>>(GetManagedTypeFromManagedPtr), nameof(GetManagedTypeFromManagedPtr), "ii");

            // Register get bool from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<bool, IntPtr>>(GetManagedBool), nameof(GetManagedBool), "ii");

            // Register get bool array from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<sbyte[], int, IntPtr>>(GetManagedBoolArray), nameof(GetManagedBoolArray), "iii");

            // Register get string from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr>>(GetManagedString), nameof(GetManagedString), "ii");

            // Register get string array from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<string[], int, IntPtr>>(GetManagedStringArray), nameof(GetManagedStringArray), "iii");

            // Register int from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<int, IntPtr>>(GetManagedInt), nameof(GetManagedInt), "ii");

            // Register get int array from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<int[], int, IntPtr>>(GetManagedIntArray), nameof(GetManagedIntArray), "iii");

            // Register get long from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<long, IntPtr>>(GetManagedLong), nameof(GetManagedLong), "ij");

            // Register get float from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<float, IntPtr>>(GetManagedFloat), nameof(GetManagedFloat), "if");

            // Register get float array from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<float[], int, IntPtr>>(GetManagedFloatArray), nameof(GetManagedFloatArray), "iii");

            // Register get double from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<double, IntPtr>>(GetManagedDouble), nameof(GetManagedDouble), "id");

            // Register get double array from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<double[], int, IntPtr>>(GetManagedDoubleArray), nameof(GetManagedDoubleArray), "iii");

            // Register get element at index from ptr array
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<IntPtr, int, IntPtr>>(GetManagedElementAtIndexFromManagedArray), nameof(GetManagedElementAtIndexFromManagedArray), "iii");

            // Register wait for task to complete
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Action<IntPtr, IntPtr>>(WaitForTaskToComplete), nameof(WaitForTaskToComplete), "vii");

            // Managed byte array
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<byte[], int, IntPtr>>(GetManagedByteArray), nameof(GetManagedByteArray), "iii");
        }

        #region 
        /// <summary>
        /// Given an GCHandle ptr, return the string representation of the object as json
        /// The returned result is a pointer to an ansi encoded string
        /// </summary>
        [MonoPInvokeCallback]
        static IntPtr GetSerializedValueFromManagedPtr(IntPtr targetObject)
        {
            // Gather the object from the GCHandle
            object obj = GCUtils.GetManagedObjectFromPtr(targetObject);

            // Serialize the object
            string json = ObjectSerializer.ToJson(obj);

            // Convert to IntPtr using Ansi marshalling to unmanaged memory
            // This will be freed on the other side
            IntPtr ptr = Marshal.StringToHGlobalAnsi(json);
            return ptr;
        }

        /// <summary>
        /// Returns the managed type of the object pointed to by the GCHandle
        /// </summary>
        [MonoPInvokeCallback]
        static IntPtr GetManagedTypeFromManagedPtr(IntPtr targetObject)
        {
            // Gather the object from the GCHandle
            object obj = GCUtils.GetManagedObjectFromPtr(targetObject);
            return GCUtils.NewManagedObject(obj.GetType());
        }

        /// <summary>
        /// Collect the managed object from the GCHandle
        /// Will throw an exception if the GCHandle is not allocated
        /// </summary>
        /// <param name="ptr"></param>
        [MonoPInvokeCallback]
        static void CollectManagedPtr(IntPtr ptr)
        {
            GCUtils.CollectManagedObjectFromPtr(ptr);
        }

        #endregion
        #region Array helpers
        /// <summary>
        /// Given an array ptr and an index, return the element at the index
        /// </summary>
        /// <param name="arrayPtr">The pointer to the managed array</param>
        [MonoPInvokeCallback]
        static IntPtr GetManagedElementAtIndexFromManagedArray(IntPtr arrayPtr, int index)
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
        #endregion

        #region Task completion
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
                await Task.Yield();

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
                object result = ReflectionUtilities.GetTaskResult(asTask);
                onCompleted.Invoke(GCUtils.NewManagedObject(result));
            }
            // Otherwise return undefined (-1 which means void)
            else
                onCompleted.Invoke(IntPtrExtension.Void);
        }
        #endregion

        #region Primitive getter

        [MonoPInvokeCallback]
        static IntPtr GetManagedBool([MarshalAs(UnmanagedType.I1)] bool managedBool) => GCUtils.NewManagedObject(managedBool);
        [MonoPInvokeCallback]
        static IntPtr GetManagedBoolArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] sbyte[] managedArray, int size){
            bool[] boolArray = new bool[size];
            for (int i = 0; i < size; i++)
                boolArray[i] = managedArray[i] != 0;
            return GCUtils.NewManagedObject(boolArray);
        }

        [MonoPInvokeCallback]
        static IntPtr GetManagedString([MarshalAs(UnmanagedType.LPStr)] string managedString) => GCUtils.NewManagedObject(managedString);

        [MonoPInvokeCallback]
        static IntPtr GetManagedStringArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] string[] managedArray, int size) => GCUtils.NewManagedObject(managedArray);

        [MonoPInvokeCallback]
        static IntPtr GetManagedInt([MarshalAs(UnmanagedType.I4)] int managedInt) => GCUtils.NewManagedObject(managedInt);

        [MonoPInvokeCallback]
        static IntPtr GetManagedIntArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] managedArray, int size) => GCUtils.NewManagedObject(managedArray);

        [MonoPInvokeCallback]
        static IntPtr GetManagedLong([MarshalAs(UnmanagedType.I8)] long managedLong) => GCUtils.NewManagedObject(managedLong);

        [MonoPInvokeCallback]
        static IntPtr GetManagedFloat([MarshalAs(UnmanagedType.R4)] float managedFloat) => GCUtils.NewManagedObject(managedFloat);

        [MonoPInvokeCallback]
        static IntPtr GetManagedFloatArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] float[] managedArray, int size) => GCUtils.NewManagedObject(managedArray);

        [MonoPInvokeCallback]
        static IntPtr GetManagedDouble([MarshalAs(UnmanagedType.R8)] double managedDouble) => GCUtils.NewManagedObject(managedDouble);

        [MonoPInvokeCallback]
        static IntPtr GetManagedDoubleArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] managedArray, int size) => GCUtils.NewManagedObject(managedArray);

        [MonoPInvokeCallback]
        static IntPtr GetManagedByteArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] managedArray, int size) => GCUtils.NewManagedObject(managedArray);

        [MonoPInvokeCallback]
        static IntPtr RegisterGetActionIntPtrFromJsPtr(IntPtr actionIntPtrAsPtr)
        {
            VI targetAct = Marshal.GetDelegateForFunctionPointer<VI>(actionIntPtrAsPtr);
            return GCUtils.NewManagedObject(targetAct);
        }
        #endregion
    }
}