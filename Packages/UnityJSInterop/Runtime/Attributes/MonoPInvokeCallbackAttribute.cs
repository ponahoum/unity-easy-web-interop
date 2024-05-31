using System;

namespace Nahoum.UnityJSInterop
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute()
        {
        }
    }
}