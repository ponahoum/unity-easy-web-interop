using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using static Nahoum.EasyWebInterop.DyncallSignature;

[assembly: InternalsVisibleTo("com.nahoum.EasyWebInterop.Tests")]
namespace Nahoum.EasyWebInterop
{
    public class MethodsRegistry
    {
        static MethodsRegistry()
        {
            InternalInteropSetup.SetupInternal();
        }

        /// <summary>
        /// Allows to pass a delegate pointer to the JS side so it can be invoked from there
        /// </summary>
        [DllImport("__Internal")]
        internal static extern void RegisterMethodInRegistry(IntPtr methodPtr, string functionName, string functionSignature, IntPtr isAsyncTask);

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
                // Copy the IntPtr array to an object array so that its passed as params and not an individual argument
                object[] newArgs = new object[args.Length];
                args.CopyTo(newArgs, 0);
                object result = targetDelegate.DynamicInvoke(newArgs);

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
                return IntPtrExtension.Exception;
            }
        }

        /// <summary>
        /// Exposes a method to the JS side from its name
        /// </summary>
        public static void RegisterMethod<T>(string name, T method) where T : Delegate
        {
            // Ensure the method is not already registered
            if (methodsRegistry.ContainsKey(name))
                throw new Exception($"Method {name} already registered");

            // Check if the method has a return value or if it is void
            bool hasReturn = method.Method.ReturnType != typeof(void);

            // Get arguments count
            int parametersCount = method.Method.GetParameters().Length;

            // Define the registry data
            (IntPtr registryPtr, Delegate asDelegate) registryData;

            // If the methods returns something, use the appropriate registry
            if (hasReturn)
            {
                registryData = parametersCount switch
                {
                    0 => (registryICallPtr, (I)(() => InvokeWrapped(method))),
                    1 => (registryIICallPtr, (II)((IntPtr inputA) => InvokeWrapped(method, inputA))),
                    2 => (registryIIICallPtr, (III)((IntPtr inputA, IntPtr inputB) => InvokeWrapped(method, inputA, inputB))),
                    3 => (registryIIIICallPtr, (IIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC) => InvokeWrapped(method, inputA, inputB, inputC))),
                    4 => (registryIIIIICallPtr, (IIIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD) => InvokeWrapped(method, inputA, inputB, inputC, inputD))),
                    _ => throw new Exception("Method has too many parameters")
                };
            }

            // If the return type is void, use the appropriate registry
            else
            {
                registryData = parametersCount switch
                {
                    0 => (registryVCallPtr, (V)(() => InvokeWrapped(method))),
                    1 => (registryVICallPtr, (VI)((IntPtr inputA) => InvokeWrapped(method, inputA))),
                    _ => throw new Exception("Method has too many parameters")
                };
            }

            // Add the method to the static registry that will later be used to invoke the method from its name
            methodsRegistry.Add(name, registryData.asDelegate);

            // Notice the js side that this named method is available under this signature
            RegisterMethodInRegistry(registryData.registryPtr, name, GetRegistrySignatureFromDelegate(registryData.asDelegate), new IntPtr(ReflectionUtilities.IsDelegateAsyncTask(method) ? 1 : 0));
        }

        /// <summary>
        /// Given a delegate, return the signature of the method to be used in the registry
        /// Typically, ends with i for int and v for void return types and add an i for the service name key intptr
        /// </summary>
        private static string GetRegistrySignatureFromDelegate(Delegate d)
        {
            // Get the invoke method
            MethodInfo invokeMethod = d.Method;

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
        /// All the arguments are wrapped in GCHandles before
        /// If the method does not return a value, throw an exception
        /// </summary>
        private static IntPtr InvokeWrapped(Delegate method, params IntPtr[] args)
        {
            try
            {
                if (method.Method.GetParameters().Length != args.Length)
                    throw new Exception("Method parameters count does not match the arguments count");

                object[] argsCasted = new object[args.Length];
                for (int i = 0; i < args.Length; i++)
                    argsCasted[i] = GCUtils.GetManagedObjectFromPtr(args[i]);

                object result = method.DynamicInvoke(argsCasted);
                return GCUtils.NewManagedObject(result);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw e;
            }
        }

    }
}