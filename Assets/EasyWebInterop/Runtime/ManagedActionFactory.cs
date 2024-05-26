using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using static Nahoum.EasyWebInterop.DyncallSignature;

namespace Nahoum.EasyWebInterop
{
    internal static class ManagedActionFactory
    {
        private static Dictionary<int, MethodInfo> methodInfosByParamCount;

        static ManagedActionFactory()
        {
            MethodInfo[] methods = typeof(ManagedActionFactory).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            methodInfosByParamCount = new Dictionary<int, MethodInfo>();
            foreach (MethodInfo thisClassMethod in methods)
            {
                if (thisClassMethod.Name.Contains(nameof(CreateAction)))
                {
                    // Get the number of generic parameters
                    int genericParamsCount = thisClassMethod.IsGenericMethod ? thisClassMethod.GetGenericArguments().Length : 0;
                    methodInfosByParamCount.Add(genericParamsCount, thisClassMethod);
                }
            }
        }

        public static object GetWrappedActionFromJsDelegate(string[] typesAsString, IntPtr jsDelegate)
        {
            // Get types
            Type[] realActionTypes = new Type[typesAsString.Length];
            for (int i = 0; i < typesAsString.Length; i++)
                realActionTypes[i] = Type.GetType(typesAsString[i]);

            Delegate delegateToCall = GetDelegateFromPtr(jsDelegate, realActionTypes.Length);

            // Get the generic method to create the action
            if (!methodInfosByParamCount.TryGetValue(realActionTypes.Length, out MethodInfo method))
                throw new ArgumentException("Count not find the method to create the action");

            // Case Action
            if(realActionTypes.Length == 0)
                return method.Invoke(null, new object[] { delegateToCall });
            // Case Action<U...W>
            else
                return method.MakeGenericMethod(realActionTypes).Invoke(null, new object[] { delegateToCall });
        }

        private static Action CreateAction(Delegate delegateToCall) => new Action(() => { DynamicInvoke(delegateToCall); });
        private static Action<T> CreateAction<T>(Delegate delegateToCall) => new Action<T>(a => { DynamicInvoke(delegateToCall, a); });
        private static Action<T, U> CreateAction<T, U>(Delegate delegateToCall) => new Action<T, U>((a, b) =>  { DynamicInvoke(delegateToCall, a, b); });
        private static Action<T, U, V> CreateAction<T, U, V>(Delegate delegateToCall) => new Action<T, U, V>((a, b, c) => { DynamicInvoke(delegateToCall, a, b, c); });
        private static Action<T, U, V, W> CreateAction<T, U, V, W>(Delegate delegateToCall) => new Action<T, U, V, W>((a, b, c, d) => { DynamicInvoke(delegateToCall, a, b, c, d); });
        private static Action<T, U, V, W, X> CreateAction<T, U, V, W, X>(Delegate delegateToCall) => new Action<T, U, V, W, X>((a, b, c, d, e) => { DynamicInvoke(delegateToCall, a, b, c, d, e); });

        private static void DynamicInvoke(Delegate del, params object[] targetData)
        {
            object[] args = new object[targetData.Length];
            for (int i = 0; i < targetData.Length; i++)
                args[i] = GCUtils.NewManagedObject(targetData[i]);
            del.DynamicInvoke(args);
        }

        private static Delegate GetDelegateFromPtr(IntPtr actionJsPtr, int paramCount)
        {
            if (paramCount == 0)
                return Marshal.GetDelegateForFunctionPointer<V>(actionJsPtr);
            else if (paramCount == 1)
                return Marshal.GetDelegateForFunctionPointer<VI>(actionJsPtr);
            else if (paramCount == 2)
                return Marshal.GetDelegateForFunctionPointer<VII>(actionJsPtr);
            else if (paramCount == 3)
                return Marshal.GetDelegateForFunctionPointer<VIII>(actionJsPtr);
            else if (paramCount == 4)
                return Marshal.GetDelegateForFunctionPointer<VIIII>(actionJsPtr);
            else if (paramCount == 5)
                return Marshal.GetDelegateForFunctionPointer<VIIIII>(actionJsPtr);

            throw new ArgumentException("The number of parameters for the action is not supported. We support 0, 1, 2, 3, 4, 5");
        }
    }
}