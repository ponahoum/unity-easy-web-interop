using System;

namespace Nahoum.EasyWebInterop
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute()
        {
        }
    }
}