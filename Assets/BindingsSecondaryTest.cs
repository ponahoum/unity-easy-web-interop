using System;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;


public class BindingsSecondaryTest : MonoBehaviour
{
    public string MyMethodReturningString() => "returning a random string";
    public double MyMethodReturningDouble() => 45005055454544545454545454545545455d;
    public int MyMethodReturningInt() => 450050554; 
    public string ConcatenateStrings(string a, string b) => a + " - added to -"+ b;

    public async Task<string> ComputeStuff() {
        int i = 0;
        Debug.Log("Starting to compute stuff");
        while(i < 3){
            await Task.Yield();
            Debug.Log("Computing stuff "+i);
            i++;
        }
        Debug.Log("Done computing stuff");
        return "Yeahh were done";
    }

    public T GetStringIfTaskComplete<T>(Task<T> targetTask){
        if(targetTask.IsCompleted){
            return targetTask.Result;
        }
        return default;
    }

    void Awake()
    {
        ServiceRegister.RegisterMethod("asimplereturnstringmethod", MyMethodReturningString);
        ServiceRegister.RegisterMethod("asimplereturndoublemethod", MyMethodReturningDouble);
        ServiceRegister.RegisterMethod("asimplereturnintmethod", MyMethodReturningInt);
        ServiceRegister.RegisterMethod("ComputeStuff", ComputeStuff);
        ServiceRegister.RegisterMethod<Task<string>, string>("GetStringIfTaskComplete", GetStringIfTaskComplete);
        ServiceRegister.RegisterMethod<string, string, string>("concatenatestrings", ConcatenateStrings);

        // Register value debugger
        ServiceRegister.RegisterDebugValueFromPtr();
        ServiceRegister.RegisterGetSerializedValueFromPtr();
    }
}
public static class ServiceRegister
{

    [DllImport("__Internal")]
    public static extern void RegisterMethodInRegistry(IntPtr methodPtr, string functionName, string functionSignature);

    public delegate IntPtr I();
    public delegate IntPtr II(IntPtr A);
    public delegate void VII(IntPtr A, IntPtr B);
    public delegate IntPtr III(IntPtr inputA, IntPtr inputB);

    static Dictionary<string, I> registeredMethodsWithOneReturnType = new ();

    static Dictionary<string, II> registeredMethodsII = new ();
    static Dictionary<string, III> registeredMethodsClassic = new ();


    [MonoPInvokeCallback]
    static IntPtr RegistryI([MarshalAs(UnmanagedType.LPStr)] string serviceKey) => registeredMethodsWithOneReturnType[serviceKey]();

    [MonoPInvokeCallback]
    static IntPtr RegistryIII([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA, IntPtr inputB)=> registeredMethodsClassic[serviceKey](inputA, inputB);

    [MonoPInvokeCallback]
    static IntPtr RegistryII([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr inputA) => registeredMethodsII[serviceKey](inputA);

    public static void RegisterMethod<T, U, V>(string name, Func<T, U, V> method){
        registeredMethodsClassic.Add(name, (IntPtr inputA, IntPtr inputB) =>
        {

            object objA = GCHandle.FromIntPtr(inputA).Target;
            object objB = GCHandle.FromIntPtr(inputB).Target;
            object result = method.Invoke((T)objA, (U)objB);
            var handle = GCHandle.Alloc(result);
            IntPtr ptr = GCHandle.ToIntPtr(handle);
            Debug.Log("Called method and generated intptr with id " + ptr);
            return ptr;
        });

        Func<string, IntPtr, IntPtr, IntPtr> registeredMethod = RegistryIII;
        IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate(registeredMethod);
        RegisterMethodInRegistry(registryCallPtr, name, "iiii");
    }

    public static void RegisterMethod<T, U>(string name, Func<T, U> method)
    {
        registeredMethodsII.Add(name, (IntPtr inputA) =>
        {
            object objA = GCHandle.FromIntPtr(inputA).Target;
            object result = method.Invoke((T)objA);
            var handle = GCHandle.Alloc(result);
            IntPtr ptr = GCHandle.ToIntPtr(handle);
            Debug.Log("Called method and generated intptr with id " + ptr);
            return ptr;
        });

        Func<string, IntPtr, IntPtr> registeredMethod = RegistryII;
        IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate(registeredMethod);
        RegisterMethodInRegistry(registryCallPtr, name, "iii");
    }
    
    /// <summary>
    /// Register a method that returns a value of type T with no parameters
    /// Signature: i
    /// </summary>
    public static void RegisterMethod<T>(string name, Func<T> method)
    {

        // Register method to the dictionary
        registeredMethodsWithOneReturnType.Add(name, () =>
        {
            // Call the method
            object result = method.Invoke();

            // Create a handle to the object. Tiis prevents the object from being garbage collected
            var handle = GCHandle.Alloc(result);

            // Convert the handle to IntPtr
            IntPtr ptr = GCHandle.ToIntPtr(handle);
            Debug.Log("Called method and generated intptr with id " + ptr);
            return ptr;
        });

        
        Func<string, IntPtr> registeredMethod = RegistryI;
        IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate(registeredMethod);
        RegisterMethodInRegistry(registryCallPtr, name, "ii");
    }

    /// <summary>
    /// Register the possibility to debug the value of an object in the console from a pointer
    /// </summary>
    public static void RegisterDebugValueFromPtr()
    {
        [MonoPInvokeCallback]
        static void RegisterDebugValueFromPtr(IntPtr _, IntPtr targetObject)
        {
            object obj = GCHandle.FromIntPtr(targetObject).Target;
            Debug.Log("Debuggin object value " + obj.ToString() + " and type " + obj.GetType().ToString());
        }

        VII registeredMethod = RegisterDebugValueFromPtr;
        IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate(registeredMethod);
        RegisterMethodInRegistry(registryCallPtr, "GatherValueFromPtr", "vii");
    }

    public static void RegisterGetSerializedValueFromPtr(){


        [MonoPInvokeCallback]
        static IntPtr RegisterGetSerializedValueFromPtr_internal(IntPtr _, IntPtr targetObject){
            object obj = GCHandle.FromIntPtr(targetObject).Target;

            // Serialize the object
            Debug.Log("Called serialized on "+obj);
            var json = new SerializableObject(obj).ToJson();
            // Convert to IntPtr using Ansi marshalling
            IntPtr ptr = Marshal.StringToHGlobalAnsi(json);

            return ptr;
        }

        III registeredMethod = RegisterGetSerializedValueFromPtr_internal;
        IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate(registeredMethod);
        RegisterMethodInRegistry(registryCallPtr, "GetSerializedValueFromPtr", "iii");
    }

        [Serializable]
        public class SerializableObject {
            public SerializableObject( object toSerialize){
                this.value = toSerialize;
            }
            public object value = null;

            public string ToJson(){
                string baseJson = "{'value':%value%}";
                if(value == null)
                    baseJson = baseJson.Replace("%value%", "null");
                else if(value is int || value is float || value is double || value is long)
                    baseJson = baseJson.Replace("%value%", value.ToString());
                else if(value is string)
                    baseJson = baseJson.Replace("%value%", "\""+value.ToString()+"\"");
                else if(value is bool)
                    baseJson = baseJson.Replace("%value%", value.ToString().ToLower());
                else
                    baseJson = baseJson.Replace("%value%", JsonUtility.ToJson(value));
                return baseJson;
            }
        }

}
