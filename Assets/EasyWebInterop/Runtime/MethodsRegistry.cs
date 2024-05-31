using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using static Nahoum.EasyWebInterop.DyncallSignature;

namespace Nahoum.EasyWebInterop
{
    [Preserve]
    public class MethodsRegistry
    {
        /// <summary>
        /// Allows to pass a delegate pointer to the JS side so it can be invoked from there
        /// </summary>
        [DllImport("__Internal")]
        static extern void RegisterMethodInRegistry(IntPtr targetId, IntPtr methodPtr, IntPtr functionName, IntPtr pathToFunctionArrPtr, int pathToFunctionArrLength, IntPtr functionSignature, IntPtr isAsyncTask);

        // Keep reference to all exposed delegates (static or not)
        static Dictionary<int, Delegate> methodsRegistry = new();
        static int currentDelegateIndex = 0;

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
            currentDelegateIndex++;

            // Internal function key used to retrieve the method from the registry

            // Check if the method has a return value or if it is void
            bool hasReturn = method.Method.ReturnType != typeof(void);

            // Get arguments count
            int parametersCount = method.Method.GetParameters().Length;

            // Define the registry data
            (IntPtr registryPtr, Delegate asDelegate) registryData;

            // Define the registry data based on the number of parameters
            registryData = parametersCount switch
            {
                0 => (registryICallPtr, (I)(() => InvokeWrapped(method))),
                1 => (registryIICallPtr, (II)((IntPtr inputA) => InvokeWrapped(method, inputA))),
                2 => (registryIIICallPtr, (III)((IntPtr inputA, IntPtr inputB) => InvokeWrapped(method, inputA, inputB))),
                3 => (registryIIIICallPtr, (IIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC) => InvokeWrapped(method, inputA, inputB, inputC))),
                4 => (registryIIIIICallPtr, (IIIII)((IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD) => InvokeWrapped(method, inputA, inputB, inputC, inputD))),
                _ => throw new Exception("Method has too many parameters")
            };

            // Add the method to the static registry that will later be used to invoke the method from its name
            methodsRegistry.Add(currentDelegateIndex, registryData.asDelegate);

            // Notice the js side that this named method is available under this signature
            IntPtr arrayPtr = MarshalUtilities.MarshalStringArray(pathToMethod, out int length, out Action freeArrayPtr);
            IntPtr functionSignature = MarshalUtilities.MarshalString(GetRegistrySignatureFromDelegate(registryData.asDelegate), out Action freeFunctionSignaturePtr);

            RegisterMethodInRegistry(new IntPtr(targetId), registryData.registryPtr, new IntPtr(currentDelegateIndex), arrayPtr, length, functionSignature,  MarshalUtilities.EncodeBool(ReflectionUtilities.IsDelegateAsyncTask(method)));

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
            if(methodsRegistry.Remove(delegateKey)){
                Debug.Log("Removed delegate from registry: " + delegateKey);
            } else {
                throw new Exception("Delegate to delete not found in registry: " + delegateKey);
            }
        }
    }
}