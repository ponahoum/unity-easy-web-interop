using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("com.nahoum.UnityJSInterop.Tests")]
namespace Nahoum.UnityJSInterop
{
    internal class ExceptionsUtilities
    {

        /// <summary>
        /// Given an exception, log the error message in the console and return IntPtr.Exception understood by the JS side
        /// </summary>
        internal static IntPtr HandleExceptionWithIntPtr(Exception e)
        {
            // Handle case the exception was raised by DynamicInvoke (the TargetInvocationException being itself of not great help)
            if (e is TargetInvocationException asTarget)
            {
                // Handle case the exception was raised from the DynamicInvoke inside the DynamicInvoke
                if (asTarget.InnerException is TargetInvocationException asTargetInner)
                    Debug.LogError(asTargetInner.InnerException.Message);
                else
                    Debug.LogError(asTarget.InnerException.Message);
            }
            else
                Debug.LogError(e.Message);
            return IntPtrExtension.Exception;
        }
    }
}