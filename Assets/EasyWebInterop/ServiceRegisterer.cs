using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PoNah.EasyWebInterop
{
    public static class ServiceRegisterer
    {

        [DllImport("__Internal")]
        public static extern void RegisterMethodInRegistry(IntPtr methodPtr, string functionName, string functionSignature);

        [DllImport("__Internal")]
        public static extern void RegisterStaticMethodInternalRegistry(IntPtr methodPtr, string functionName, string functionSignature);

        [DllImport("__Internal")]
        public static extern void Setup(IntPtr getIntPtrValueMethodPtr);

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

            // Register element get Type from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<IntPtr, IntPtr>>(GetManagedElementType), nameof(GetManagedElementType), "ii");
        }


        static Dictionary<string, Delegate> methodsRegistry = new();
        
        public delegate void V();
        public delegate void VI(IntPtr A);
        public delegate void VII(IntPtr A, IntPtr B);
        public delegate IntPtr I();
        public delegate IntPtr II(IntPtr A);
        public delegate IntPtr III(IntPtr inputA, IntPtr inputB);

        [MonoPInvokeCallback]
        static IntPtr RegistryI([MarshalAs(UnmanagedType.LPStr)] string serviceKey) => (methodsRegistry[serviceKey] as I)();
        static IntPtr registryICallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr>>(RegistryI);

        [MonoPInvokeCallback]
        static IntPtr RegistryII([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA) => (methodsRegistry[serviceKey] as II)(inputA);
        static IntPtr registryIICallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr, IntPtr>>(RegistryII);

        [MonoPInvokeCallback]
        static IntPtr RegistryIII([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA, IntPtr inputB) => (methodsRegistry[serviceKey] as III)(inputA, inputB);
        static IntPtr registryIIICallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr, IntPtr, IntPtr>>(RegistryIII);

        public static void RegisterMethod<T>(string name, Func<T> method)
        {
            I asDelegate = () => RegisterAndInvoke(method);
            methodsRegistry.Add(name, asDelegate);
            RegisterMethodInRegistry(registryICallPtr, name, "ii");
        }
        public static void RegisterMethod<TIn, UOut>(string name, Func<TIn, UOut> method)
        {
            II asDelegate = (IntPtr inputA) => RegisterAndInvoke(method, inputA);
            methodsRegistry.Add(name, asDelegate);
            RegisterMethodInRegistry(registryIICallPtr, name, "iii");
        }
        public static void RegisterMethod<T, U, VOut>(string name, Func<T, U, VOut> method)
        {
            III asDelegate = (IntPtr inputA, IntPtr inputB) => RegisterAndInvoke(method, inputA, inputB);
            methodsRegistry.Add(name, asDelegate);
            RegisterMethodInRegistry(registryIIICallPtr, name, "iiii");
        }

        private static IntPtr RegisterAndInvoke(Delegate method, params IntPtr[] args)
        {
            try
            {
                object[] argsCasted = new object[args.Length];
                for (int i = 0; i < args.Length; i++)
                    argsCasted[i] = GetManagedObjectFromPtr(args[i]);

                object result = method.DynamicInvoke(argsCasted);
                return NewManagedObject(result);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
        }

        /// <summary>
        /// Given an GCHandle ptr, return the string representation of the object as json
        /// The returned result is a pointer to an ansi encoded string
        /// </summary>
        [MonoPInvokeCallback]
        static IntPtr GetSerializedValueFromPtr(IntPtr targetObject)
        {
            // Gather the object from the GCHandle
            object obj = GetManagedObjectFromPtr(targetObject);

            // Serialize the object
            string json = ObjectSerializer.ToJson(obj);

            // Convert to IntPtr using Ansi marshalling
            IntPtr ptr = Marshal.StringToHGlobalAnsi(json);
            return ptr;
        }

        [MonoPInvokeCallback]
        static IntPtr GetManagedDoubleFromPtr([MarshalAs(UnmanagedType.R8)] double managedDouble) => NewManagedObject(managedDouble);

        [MonoPInvokeCallback]
        static IntPtr GetManagedDoubleArrayFromPtr([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] managedArray, int size) => NewManagedObject(managedArray);

        /// <summary>
        /// Given an array ptr and an index, return the element at the index
        /// </summary>
        /// <param name="arrayPtr">The pointer to the managed array</param>
        [MonoPInvokeCallback]
        static IntPtr GetManagedElementAtIndexFromManagedPtrArray(IntPtr arrayPtr, int index)
        {
            // Get object from ptr
            object array = GetManagedObjectFromPtr(arrayPtr);

            // Check array is array
            if (!array.GetType().IsArray)
                throw new Exception("The object is not an array");

            Array asArray = (Array)array;
            if (index >= asArray.Length)
                throw new IndexOutOfRangeException("Index out of range");

            // Get element at index
            return NewManagedObject(asArray.GetValue(index));
        }

        /// <summary>
        /// Returns the type of the managed element from the ptr
        /// </summary>
        [MonoPInvokeCallback]
        static IntPtr GetManagedElementType(IntPtr targetObject) => NewManagedObject(GetManagedObjectFromPtr(targetObject).GetType().ToString());

        /// <summary>
        /// Given an object, return a GCHandle ptr to the object
        /// </summary>
        static IntPtr NewManagedObject(object targetObject)
        {
            // Handle null case
            if (targetObject == null)
                return IntPtr.Zero;

            GCHandle elementHandle = GCHandle.Alloc(targetObject);
            return GCHandle.ToIntPtr(elementHandle);
        }

        /// <summary>
        /// Returns the managed object from the GCHandle ptr
        /// </summary>
        static object GetManagedObjectFromPtr(IntPtr targetObject)
        {
            if (targetObject == IntPtr.Zero)
                return null;

            return GCHandle.FromIntPtr(targetObject).Target;
        }

        public static void RegisterGetActionStringFromPtr()
        {
            [MonoPInvokeCallback]
            static IntPtr RegisterGetActionStringFromPtr_internal(IntPtr _, IntPtr actionAsPtr)
            {
                VI targetAct = Marshal.GetDelegateForFunctionPointer<VI>(actionAsPtr);
                Action<string> targetActAsActionString = (string value) =>
                {
                    targetAct.Invoke(NewManagedObject(value));
                };
                return NewManagedObject(targetActAsActionString);
            }

            IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate<III>(RegisterGetActionStringFromPtr_internal);
            RegisterMethodInRegistry(registryCallPtr, "GetActionStringFromPtr", "iii");
        }
    }
}