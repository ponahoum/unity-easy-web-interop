using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.DedicatedServer;
using UnityEngine.Scripting;
using static Nahoum.UnityJSInterop.DyncallSignature;

namespace Nahoum.UnityJSInterop
{
    [Preserve]
    public class MethodsRegistry
    {
        /// <summary>
        /// Allows to pass a delegate pointer to the JS side so it can be invoked from there
        /// </summary>
        [DllImport("__Internal")]
        static extern void RegisterMethodInRegistry(IntPtr targetId, IntPtr methodPtr, IntPtr functionName, IntPtr pathToFunctionArrPtr, int pathToFunctionArrLength, IntPtr functionSignature, IntPtr methodReturnsTask);

        // Keep reference to all exposed delegates (static or not)
        static Dictionary<int, Delegate> methodsRegistry = new();
        static int currentDelegateIndex = 0;

        // 0 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryI(IntPtr serviceKey) => InvokeFromRegistry<I>(serviceKey);
        static IntPtr registryICallPtr = Marshal.GetFunctionPointerForDelegate<II>(RegistryI);

        // 1 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryII(IntPtr serviceKey, IntPtr inputA) => InvokeFromRegistry<II>(serviceKey, inputA);
        static IntPtr registryIICallPtr = Marshal.GetFunctionPointerForDelegate<III>(RegistryII);

        // 2 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB) => InvokeFromRegistry<III>(serviceKey, inputA, inputB);
        static IntPtr registryIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIII>(RegistryIII);

        // 3 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC) => InvokeFromRegistry<IIII>(serviceKey, inputA, inputB, inputC);
        static IntPtr registryIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIII>(RegistryIIII);

        // 4 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryIIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD) => InvokeFromRegistry<IIIII>(serviceKey, inputA, inputB, inputC, inputD);
        static IntPtr registryIIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIIII>(RegistryIIIII);

        // 5 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryIIIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE) =>
            InvokeFromRegistry<IIIIII>(serviceKey, inputA, inputB, inputC, inputD, inputE);
        static IntPtr registryIIIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIIIII>(RegistryIIIIII);

        // 6 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryIIIIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF) =>
            InvokeFromRegistry<IIIIIII>(serviceKey, inputA, inputB, inputC, inputD, inputE, inputF);
        static IntPtr registryIIIIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIIIIII>(RegistryIIIIIII);

        // 7 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryIIIIIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG) =>
            InvokeFromRegistry<IIIIIIII>(serviceKey, inputA, inputB, inputC, inputD, inputE, inputF, inputG);
        static IntPtr registryIIIIIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIIIIIII>(RegistryIIIIIIII);

        // 8 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryIIIIIIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH) =>
            InvokeFromRegistry<IIIIIIIII>(serviceKey, inputA, inputB, inputC, inputD, inputE, inputF, inputG, inputH);
        static IntPtr registryIIIIIIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIIIIIIII>(RegistryIIIIIIIII);

        // 9 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryIIIIIIIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH, IntPtr inputI) =>
            InvokeFromRegistry<IIIIIIIIII>(serviceKey, inputA, inputB, inputC, inputD, inputE, inputF, inputG, inputH, inputI);
        static IntPtr registryIIIIIIIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIIIIIIIII>(RegistryIIIIIIIIII);

        // 10 arguments methods registry
        [MonoPInvokeCallback]
        static IntPtr RegistryIIIIIIIIIII(IntPtr serviceKey, IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH, IntPtr inputI, IntPtr inputJ) =>
            InvokeFromRegistry<IIIIIIIIIII>(serviceKey, inputA, inputB, inputC, inputD, inputE, inputF, inputG, inputH, inputI, inputJ);
        static IntPtr registryIIIIIIIIIIICallPtr = Marshal.GetFunctionPointerForDelegate<IIIIIIIIIIII>(RegistryIIIIIIIIIII);

        /// <summary>
        /// Find a method in the registry and invoke it with the real points
        /// The pointers passed here must be the real pointers to the objects, not the wrapped GCHandle pointers
        /// Only used for internal calls
        /// </summary>
        private static IntPtr InvokeFromRegistry<T>(IntPtr serviceKey, params IntPtr[] args)
        where T : Delegate
        {
            // Get the delegate from the registry
            Delegate targetDelegate = methodsRegistry[serviceKey.ToInt32()];

            // Invoke it dynamically
            try
            {
                // Copy the IntPtr array to an object array so that its passed as params and not an individual argument
                object[] newArgs = new object[args.Length];
                args.CopyTo(newArgs, 0);
                object result = targetDelegate.DynamicInvoke(newArgs);

                if (result is IntPtr asIntPtr)
                    return asIntPtr;
                else
                    throw new Exception("Return type not supported");
            }
            catch (Exception e)
            {
                return ExceptionsUtilities.HandleExceptionWithIntPtr(e);
            }
        }

        /// <summary>
        /// Exposes a method to the JS side from its name
        /// </summary>
        internal static void RegisterMethod(string[] pathToMethod, Delegate method, int targetId = -1)
        {
            // Increment key that is used as a UID to retrieve the method from the registry
            currentDelegateIndex++;

            // Check if the method has a return value or if it is void
            bool hasReturn = method.Method.ReturnType != typeof(void);

            // Get arguments count
            int parametersCount = method.Method.GetParameters().Length;

            // Define the registry data
            (IntPtr registryPtr, Delegate asDelegate) registryData;

            // Define the registry data based on the number of parameters
            registryData = parametersCount switch
            {
                0 => (registryICallPtr, (I)(() => InvokeWrapped(method))), // No argument methods
                1 => (registryIICallPtr, (II)((IntPtr inputA) => InvokeWrapped(method, inputA))), // 1 argument methods
                2 => (registryIIICallPtr, (III)((IntPtr inputA, IntPtr inputB) => InvokeWrapped(method, inputA, inputB))), // 2 arguments methods
                3 => (registryIIIICallPtr, (IIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC) => InvokeWrapped(method, inputA, inputB, inputC))), // 3 arguments methods etc...
                4 => (registryIIIIICallPtr, (IIIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD) => InvokeWrapped(method, inputA, inputB, inputC, inputD))),
                5 => (registryIIIIIICallPtr, (IIIIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE) => InvokeWrapped(method, inputA, inputB, inputC, inputD, inputE))),
                6 => (registryIIIIIIICallPtr, (IIIIIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF) => InvokeWrapped(method, inputA, inputB, inputC, inputD, inputE, inputF))),
                7 => (registryIIIIIIIICallPtr, (IIIIIIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG) => InvokeWrapped(method, inputA, inputB, inputC, inputD, inputE, inputF, inputG))),
                8 => (registryIIIIIIIIICallPtr, (IIIIIIIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH) => InvokeWrapped(method, inputA, inputB, inputC, inputD, inputE, inputF, inputG, inputH))),
                9 => (registryIIIIIIIIIICallPtr, (IIIIIIIIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH, IntPtr inputI) => InvokeWrapped(method, inputA, inputB, inputC, inputD, inputE, inputF, inputG, inputH, inputI))),
                10 => (registryIIIIIIIIIIICallPtr, (IIIIIIIIIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH, IntPtr inputI, IntPtr inputJ) => InvokeWrapped(method, inputA, inputB, inputC, inputD, inputE, inputF, inputG, inputH, inputI, inputJ))),
                _ => throw new Exception($"Method {method.Method.Name} can't be registered because it has too many parameters ({parametersCount}). The limit is 10.")
            }; ;

            // Add the method to the static registry that will later be used to invoke the method from its name
            methodsRegistry.Add(currentDelegateIndex, registryData.asDelegate);

            // Notice the js side that this named method is available under this signature
            IntPtr arrayPtr = MarshalUtilities.MarshalStringArray(pathToMethod, out int length, out Action freeArrayPtr);
            IntPtr functionSignature = MarshalUtilities.MarshalString(GetRegistrySignatureFromDelegate(registryData.asDelegate), out Action freeFunctionSignaturePtr);

            RegisterMethodInRegistry(new IntPtr(targetId), registryData.registryPtr, new IntPtr(currentDelegateIndex), arrayPtr, length, functionSignature, MarshalUtilities.EncodeBool(ReflectionUtilities.DelegateReturnsTask(method)));

            // Free the allocated pointers except the function key which is needed to be called later on
            // The rest has been marshalled by js so we can free themÒ
            freeArrayPtr();
            freeFunctionSignaturePtr();
        }

        /// <summary>
        /// Given a delegate, return the signature of the method to be used in the registry
        /// Typically, ends with i for int and v for void return types and add an i for the service name key intptr
        /// </summary>
        private static string GetRegistrySignatureFromDelegate(Delegate d)
        {
            // Get the invoke method
            MethodInfo invokeMethod = d.Method;

            // Get parameters
            ParameterInfo[] parameters = invokeMethod.GetParameters();
            int parameterCount = parameters.Length;

            // There is n times the parameters i concatenated (+ one i for the name as the first parameter, and i for the return, even for void)
            string parameterSignature = "ii";
            for (int i = 0; i < parameterCount; i++)
                parameterSignature += "i";
            return parameterSignature;
        }

        /// <summary>
        /// Given a method and its arguments, invoke the method and return the result
        /// All the arguments are wrapped in GCHandles before
        /// If the method does not return a value, throw an exception
        /// </summary>
        private static IntPtr InvokeWrapped(Delegate method, params IntPtr[] args)
        {
            if (method.Method.GetParameters().Length != args.Length)
                throw new Exception("Method parameters count does not match the arguments count");

            object[] argsCasted = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
                argsCasted[i] = GCUtils.GetManagedObjectFromPtr(args[i]);

            bool methodHasReturn = method.Method.ReturnType != typeof(void);

            // Invoke instance method
            object result = method.DynamicInvoke(argsCasted);

            if (!methodHasReturn)
                return IntPtrExtension.Void;
            else
                return GCUtils.NewManagedObject(result);
        }

        /// <summary>
        /// Removes a delegate from the registry given a int key
        /// Throws an exception if the key is not found
        /// </summary>
        internal static void FreeDelegate(int delegateKey)
        {
            if (!methodsRegistry.Remove(delegateKey))
            {
                throw new Exception("Delegate to delete not found in registry: " + delegateKey);
            }
        }

        /// <summary>
        /// Get the count of the delegates in the registry, for debugging purposes
        /// </summary>
        internal static int GetDelegateCount() => methodsRegistry.Count;
    }
}