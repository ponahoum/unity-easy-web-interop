using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using static PoNah.EasyWebInterop.DyncallSignature;

namespace PoNah.EasyWebInterop
{
    public class MethodsRegistry
    {
        static MethodsRegistry()
        {
            InternalInteropSetup.Setup();
        }

        /// <summary>
        /// Allows to pass a delegate pointer to the JS side so it can be invoked from there
        /// </summary>
        [DllImport("__Internal")]
        internal static extern void RegisterMethodInRegistry(IntPtr methodPtr, string functionName, string functionSignature);

        // Keep reference to all exposed delegates (static or not)
        static Dictionary<string, Delegate> methodsRegistry = new();

        [MonoPInvokeCallback]
        static IntPtr RegistryI(IntPtr serviceKey) => InvokeFromRegistry<I>(serviceKey);
        static IntPtr registryICallPtr = Marshal.GetFunctionPointerForDelegate<II>(RegistryI);

        [MonoPInvokeCallback]
        static IntPtr RegistryII(IntPtr serviceKey, IntPtr inputA) => InvokeFromRegistry<II>(serviceKey, inputA);
        static IntPtr registryIICallPtr = Marshal.GetFunctionPointerForDelegate<III>(RegistryII);

        [MonoPInvokeCallback]
        static IntPtr RegistryIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB) => InvokeFromRegistry<III>(serviceKey, inputA, inputB);
        static IntPtr registryIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIII>(RegistryIII);

        [MonoPInvokeCallback]
        static IntPtr RegistryIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC) => InvokeFromRegistry<IIII>(serviceKey, inputA, inputB, inputC);
        static IntPtr registryIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIII>(RegistryIIII);

        [MonoPInvokeCallback]
        static IntPtr RegistryIIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD) => InvokeFromRegistry<IIIII>(serviceKey, inputA, inputB, inputC, inputD);
        static IntPtr registryIIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIIII>(RegistryIIIII);


        [MonoPInvokeCallback]
        static void RegistryV(IntPtr serviceKey) => InvokeFromRegistry<V>(serviceKey);
        static IntPtr registryVCallPtr = Marshal.GetFunctionPointerForDelegate<VI>(RegistryV);

        [MonoPInvokeCallback]
        static void RegistryVI(IntPtr serviceKey, IntPtr inputA) => InvokeFromRegistry<VI>(serviceKey, inputA);
        static IntPtr registryVICallPtr = Marshal.GetFunctionPointerForDelegate<VII>(RegistryVI);


        /// <summary>
        /// Find a method in the registry and invoke it with the real points
        /// The pointers passed here must be the real pointers to the objects, not the wrapped GCHandle pointers
        /// Only used for internal calls
        /// </summary>
        private static IntPtr InvokeFromRegistry<T>(IntPtr serviceKey, params IntPtr[] args)
        where T : Delegate
        {
            // Convert servicekey to string
            string serviceKeyStr = Marshal.PtrToStringUTF8(serviceKey);

            // Get the delegate from the registry
            Delegate targetDelegate = methodsRegistry[serviceKeyStr];

            // Invoke it dynamically
            try
            {
                object result;
                if (args.Length == 0)
                    result = targetDelegate.DynamicInvoke();
                else
                    result = targetDelegate.DynamicInvoke(args);

                if (result is IntPtr asIntPtr)
                    return asIntPtr;
                else if (result is null)
                    return IntPtr.Zero;
                else
                    throw new Exception("Return type not supported");

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
        }

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
                else if (parametersCount == 4)
                {
                    IIIII asDelegate = (IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD) => InvokeWrapped(method, inputA, inputB, inputC, inputD);
                    methodsRegistry.Add(name, asDelegate);
                    RegisterMethodInRegistry(registryIIIIICallPtr, name, GetRegistryMethodSignature<IIIII>());
                }
                else
                    throw new Exception("Method has too many parameters");
            }
            else
            {
                if (parametersCount == 0)
                {
                    V asDelegate = () => InvokeWrappedInternal(method);
                    methodsRegistry.Add(name, asDelegate);
                    RegisterMethodInRegistry(registryVCallPtr, name, GetRegistryMethodSignature<V>());
                }
                else if (parametersCount == 1)
                {
                    VI asDelegate = (IntPtr inputA) => InvokeWrappedInternal(method, inputA);
                    methodsRegistry.Add(name, asDelegate);
                    RegisterMethodInRegistry(registryVICallPtr, name, GetRegistryMethodSignature<VI>());
                }
            }
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
    }
}