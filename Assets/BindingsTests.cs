using System;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using System.Collections.Generic;

// public class BindingsTests : MonoBehaviour
// {


//     [MonoPInvokeCallback]
//     public static string MyMethodToCall([MarshalAs(UnmanagedType.LPStr)] string input){
//         // This fires from JavaScript
//         Debug.Log("The method was rightfully called from JS ! ");
//         return "Hello darkness my old friend: "+input;
//     }
//     public void DebugStringPtr(IntPtr stringPointer){
//         string result = Marshal.PtrToStringAnsi(stringPointer);
//         Debug.Log("Got ptr of "+stringPointer);
//         Debug.Log("We got a string result of "+result);
//     }

//     [MonoPInvokeCallback]
//     public static void DebugAnIntArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] array, int length)
//     {
//         // Process the array
//         Debug.Log("Received int array: " + string.Join(", ", array));
//     }

//     delegate void BarFunc(string myArg);

//     [MonoPInvokeCallback]
//     public static void CallFunc(IntPtr action){
//         Debug.Log("Received action "+ action);
//         try
//         {
//             var targetAct = Marshal.GetDelegateForFunctionPointer<BarFunc>(action);
//             targetAct.Invoke("abcd");
//         }
//         catch (System.Exception e)
//         {
            
//             Debug.LogError(e.Message);
//         }
        
//     }

//     static Dictionary<string, Action<IntPtr>> lookupTables =  new();
//     void Awake(){
//         // Register test method
//         Func<string, string> myMethod = MyMethodToCall;
//         IntPtr functionPointer = Marshal.GetFunctionPointerForDelegate(myMethod);
//         RegisterMethodExternal(functionPointer, nameof(MyMethodToCall), "ii");

//         // Register debug method
//         lookupTables.Add(nameof(DebugStringPtr), DebugStringPtr);
        
//         [MonoPInvokeCallback]
//         static void Registry([MarshalAs(UnmanagedType.LPStr)] string serviceKey, IntPtr input){

//             Debug.Log("Got key call to "+serviceKey);
//             // Get string
//             lookupTables[serviceKey](input);
//         }

//         Action<string, IntPtr> debugMethod = Registry;
//         IntPtr registryCallPtr = Marshal.GetFunctionPointerForDelegate(debugMethod);
//         RegisterMethodExternal(registryCallPtr, "registryCall", "vii");

//         // Register test to pass array
//         Action<int[], int> debugIntArrayMethod = DebugAnIntArray;
//         IntPtr debugIntArrayMethodPtr = Marshal.GetFunctionPointerForDelegate(debugIntArrayMethod);
//         RegisterMethodExternal(debugIntArrayMethodPtr, nameof(DebugAnIntArray), "vii");

//         // Reigster js to csharp callback
//         Action<IntPtr> callbackJs = CallFunc;
//         IntPtr ptrToJsCallback = Marshal.GetFunctionPointerForDelegate(callbackJs);
//         RegisterMethodExternal(ptrToJsCallback, nameof(CallFunc), "vi");
//     }    
// }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MonoPInvokeCallbackAttribute : Attribute
{
    public MonoPInvokeCallbackAttribute()
    {
    }
}