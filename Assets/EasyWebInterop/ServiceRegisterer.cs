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
        public delegate void VIII(IntPtr A, IntPtr B, IntPtr C);
        public delegate void VIIII(IntPtr A, IntPtr B, IntPtr C, IntPtr D);
        public delegate void VIIIII(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E);
        public delegate IntPtr I();
        public delegate IntPtr II(IntPtr A);
        public delegate IntPtr III(IntPtr inputA, IntPtr inputB);
        public delegate IntPtr IIII(IntPtr inputA, IntPtr inputB, IntPtr inputC);
        public delegate IntPtr IIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD);
        public delegate IntPtr IIIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE);

        [MonoPInvokeCallback]
        static IntPtr RegistryI([MarshalAs(UnmanagedType.LPStr)] string serviceKey) => (methodsRegistry[serviceKey] as I)();
        static IntPtr registryICallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr>>(RegistryI);

        [MonoPInvokeCallback]
        static IntPtr RegistryII([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA) => (methodsRegistry[serviceKey] as II)(inputA);
        static IntPtr registryIICallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr, IntPtr>>(RegistryII);

        [MonoPInvokeCallback]
        static IntPtr RegistryIII([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA, IntPtr inputB) => (methodsRegistry[serviceKey] as III)(inputA, inputB);
        static IntPtr registryIIICallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr, IntPtr, IntPtr>>(RegistryIII);

        [MonoPInvokeCallback]
        static IntPtr RegistryIIII([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC) => (methodsRegistry[serviceKey] as IIII)(inputA, inputB, inputC);
        static IntPtr registryIIIICallPtr = Marshal.GetFunctionPointerForDelegate<Func<string, IntPtr, IntPtr, IntPtr, IntPtr>>(RegistryIIII);

        [MonoPInvokeCallback]
        static void RegistryV([MarshalAs(UnmanagedType.LPStr)] string serviceKey) => (methodsRegistry[serviceKey] as V)();
        static IntPtr registryVCallPtr = Marshal.GetFunctionPointerForDelegate<Action<string>>(RegistryV);

        [MonoPInvokeCallback]
        static void RegistryVI([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA) => (methodsRegistry[serviceKey] as VI)(inputA);
        static IntPtr registryVICallPtr = Marshal.GetFunctionPointerForDelegate<Action<string, IntPtr>>(RegistryVI);

        /// <summary>
        /// Exposes a method with a given name to the JS side
        /// </summary>
        public static void RegisterMethod<T>(string name, T method)
            where T : Delegate
        {
            // Check if a method with the same name is already registered
            if (methodsRegistry.ContainsKey(name))
                throw new Exception($"Method {name} already registered");

            // Get parameters count and check if there is a return value
            bool hasReturn = method.Method.ReturnType != typeof(void);
            int parametersCount = method.Method.GetParameters().Length;

            // Case has return
            if (hasReturn)
            {
                // No parameters
                if (parametersCount == 0)
                {
                    I asDelegate = () => InvokeWrapped(method);
                    methodsRegistry.Add(name, asDelegate);
                    RegisterMethodInRegistry(registryICallPtr, name, GetRegistryMethodSignature<I>());
                }
                // One parameter
                else if (parametersCount == 1)
                {
                    II asDelegate = (IntPtr inputA) => InvokeWrapped(method, inputA);
                    methodsRegistry.Add(name, asDelegate);
                    RegisterMethodInRegistry(registryIICallPtr, name, GetRegistryMethodSignature<II>());
                }
                else if (parametersCount == 2)
                {
                    III asDelegate = (IntPtr inputA, IntPtr inputB) => InvokeWrapped(method, inputA, inputB);
                    methodsRegistry.Add(name, asDelegate);
                    RegisterMethodInRegistry(registryIIICallPtr, name, GetRegistryMethodSignature<III>());
                }
                else if (parametersCount == 3)
                {
                    IIII asDelegate = (IntPtr inputA, IntPtr inputB, IntPtr inputC) => InvokeWrapped(method, inputA, inputB, inputC);
                    methodsRegistry.Add(name, asDelegate);
                    RegisterMethodInRegistry(registryIIIICallPtr, name, GetRegistryMethodSignature<IIII>());
                }
            }
            else
            {
                if (parametersCount == 0)
                {
                    V asDelegate = () => InvokeWrappedVoid(method);
                    methodsRegistry.Add(name, asDelegate);
                    RegisterMethodInRegistry(registryVCallPtr, name, GetRegistryMethodSignature<V>());
                }
                else if (parametersCount == 1)
                {
                    VI asDelegate = (IntPtr inputA) => InvokeWrappedVoid(method, inputA);
                    methodsRegistry.Add(name, asDelegate);
                    RegisterMethodInRegistry(registryVICallPtr, name, GetRegistryMethodSignature<VI>());
                }
            }
        }


        /// <summary>
        /// Given a delegate, return the signature of the method to be used in the registry
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static string GetRegistryMethodSignature<T>()
            where T : Delegate
        {
            // Get the invoke method
            MethodInfo invokeMethod = typeof(T).GetMethod("Invoke");

            // Get return type
            Type returnType = invokeMethod.ReturnType;
            bool hasReturnType = returnType != typeof(void);

            // Get parameters
            ParameterInfo[] parameters = invokeMethod.GetParameters();
            int parameterCount = parameters.Length;

            // There is n times the parameters i concatenated (+ one i for the name as the first parameter)
            string parameterSignature = "i";
            for (int i = 0; i < parameterCount; i++)
                parameterSignature += "i";

            if (hasReturnType)
                return "i" + parameterSignature;
            else
                return "v" + parameterSignature;
        }

        /// <summary>
        /// Given a method and its arguments, unwrap the arguments and invoke the method, return a wrapped result
        /// </summary>
        private static IntPtr InvokeWrapped(Delegate method, params IntPtr[] args)
        {
            // Check the method returns something
            if (method.Method.ReturnType == typeof(void))
                throw new Exception("Method returns void. It should return a value");

            object result = InvokeWrappedInternal(method, args);
            return NewManagedObject(result);
        }

        /// <summary>
        /// Given a method and its arguments, invoke the method and return nothing (void return)
        /// </summary>
        private static void InvokeWrappedVoid(Delegate method, params IntPtr[] args)
        {
            // Check the method returns void
            if (method.Method.ReturnType != typeof(void))
                throw new Exception("Method returns a value, It should return void");

            InvokeWrappedInternal(method, args);
        }
        /// <summary>
        /// Given a method and its arguments, invoke the method and return the result
        /// If the method does not return a value, throw an exception
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static object InvokeWrappedInternal(Delegate method, params IntPtr[] args)
        {
            try
            {
                if (method.Method.GetParameters().Length != args.Length)
                    throw new Exception("Method parameters count does not match the arguments count");

                object[] argsCasted = new object[args.Length];
                for (int i = 0; i < args.Length; i++)
                    argsCasted[i] = GetManagedObjectFromPtr(args[i]);

                return method.DynamicInvoke(argsCasted);
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