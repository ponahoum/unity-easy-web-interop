using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

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
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<double, IntPtr>>(GetDoubleFromPtr), nameof(GetDoubleFromPtr), "id");

            // Register get double array from ptr
            RegisterStaticMethodInternalRegistry(Marshal.GetFunctionPointerForDelegate<Func<double[], int, IntPtr>>(GetDoubleArrayFromPtr), nameof(GetDoubleArrayFromPtr), "iii");
        }

        public delegate void V();
        public delegate void VI(IntPtr A);
        public delegate void VII(IntPtr A, IntPtr B);
        public delegate IntPtr I();
        public delegate IntPtr II(IntPtr A);
        public delegate IntPtr III(IntPtr inputA, IntPtr inputB);

        static Dictionary<string, Delegate> methodsRegistry = new();

        [MonoPInvokeCallback]
        static IntPtr RegistryI([MarshalAs(UnmanagedType.LPStr)] string serviceKey) => (methodsRegistry[serviceKey] as I)();
        [MonoPInvokeCallback]
        static IntPtr RegistryII([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA) => (methodsRegistry[serviceKey] as II)(inputA);

        [MonoPInvokeCallback]
        static IntPtr RegistryIII([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA, IntPtr inputB) => (methodsRegistry[serviceKey] as III)(inputA, inputB);

        public static void RegisterMethod<T, U, V>(string name, Func<T, U, V> method)
        {
            III asDelegate = (IntPtr inputA, IntPtr inputB) =>
            {
                object objA = GCHandle.FromIntPtr(inputA).Target;
                object objB = GCHandle.FromIntPtr(inputB).Target;
                object result = method.Invoke((T)objA, (U)objB);
                var handle = GCHandle.Alloc(result);
                IntPtr ptr = GCHandle.ToIntPtr(handle);
                Debug.Log("Called method and generated intptr with id " + ptr);
                return ptr;
            };

            methodsRegistry.Add(name, asDelegate);
            IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr, IntPtr, IntPtr>>(RegistryIII);
            RegisterMethodInRegistry(registryCallPtr, name, "iiii");
        }

        public static void RegisterMethod<TIn, UOut>(string name, Func<TIn, UOut> method)
        {
            II asDelegate = (IntPtr inputA) =>
            {
                object objA = GCHandle.FromIntPtr(inputA).Target;
                UOut result = method.Invoke((TIn)objA);
                GCHandle handle = GCHandle.Alloc(result);
                IntPtr ptr = GCHandle.ToIntPtr(handle);
                return ptr;
            };

            methodsRegistry.Add(name, asDelegate);
            IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr, IntPtr>>(RegistryII);
            RegisterMethodInRegistry(registryCallPtr, name, "iii");
        }

        public static void RegisterMethod<T>(string name, Func<T> method)
        {
            I asDelegate = () =>
            {
                T result = method.Invoke();
                GCHandle handle = GCHandle.Alloc(result);
                IntPtr ptr = GCHandle.ToIntPtr(handle);
                return ptr;
            };

            // Register method to the dictionary
            methodsRegistry.Add(name, asDelegate);
            IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr>>(RegistryI);
            RegisterMethodInRegistry(registryCallPtr, name, "ii");
        }

        /// <summary>
        /// Given an GCHandle ptr, return the string representation of the object as json
        /// The returned result is a pointer to an ansi encoded string
        /// </summary>
        [MonoPInvokeCallback]
        static IntPtr GetSerializedValueFromPtr(IntPtr targetObject)
        {
            // Gather the object from the GCHandle
            object obj = GCHandle.FromIntPtr(targetObject).Target;

            // Serialize the object
            string json = ObjectSerializer.ToJson(obj);

            // Convert to IntPtr using Ansi marshalling
            IntPtr ptr = Marshal.StringToHGlobalAnsi(json);
            return ptr;
        }

        [MonoPInvokeCallback]
        static IntPtr GetDoubleFromPtr([MarshalAs(UnmanagedType.R8)] double managedDouble)
        {
            GCHandle handle = GCHandle.Alloc(managedDouble);
            return GCHandle.ToIntPtr(handle);
        }

        [MonoPInvokeCallback]
        static IntPtr GetDoubleArrayFromPtr([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] managedArray, int size)
        {
            GCHandle handle = GCHandle.Alloc(managedArray);
            return GCHandle.ToIntPtr(handle);
        }

        public static void RegisterGetActionStringFromPtr()
        {
            [MonoPInvokeCallback]
            static IntPtr RegisterGetActionStringFromPtr_internal(IntPtr _, IntPtr actionAsPtr)
            {
                var targetAct = Marshal.GetDelegateForFunctionPointer<VI>(actionAsPtr);
                Action<string> targetActAsActionString = (string value) =>
                {
                    GCHandle handle = GCHandle.Alloc(value);
                    targetAct.Invoke(GCHandle.ToIntPtr(handle));
                };
                GCHandle handle = GCHandle.Alloc(targetActAsActionString);
                return GCHandle.ToIntPtr(handle); ;
            }

            IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate<III>(RegisterGetActionStringFromPtr_internal);
            RegisterMethodInRegistry(registryCallPtr, "GetActionStringFromPtr", "iii");
        }
    }
}