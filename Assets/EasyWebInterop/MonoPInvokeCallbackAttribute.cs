using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MonoPInvokeCallbackAttribute : Attribute
{
    public MonoPInvokeCallbackAttribute()
    {
    }
}