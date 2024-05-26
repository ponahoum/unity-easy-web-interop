using System;
using UnityEngine.Scripting;

[AttributeUsage(AttributeTargets.Method)]
public class ExposeWebAttribute : PreserveAttribute
{
    public ExposeWebAttribute()
    {
    }
}