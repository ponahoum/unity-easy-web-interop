using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nahoum.EasyWebInterop;
using UnityEngine;
using UnityEngine.Scripting;


public static class RuntimeAutoRegisterTestsStatic{
    
    [ExposeWeb]
    public static string DoStuff(){
        return "Hello World";
    }
}

public class RuntimeAutoRegisterTests : MonoBehaviour
{
    void Awake(){
        AutoRegister.RegisterService("TestingAttributes", this);
    }

    [ExposeWeb]
    public string TestingThings() => "Hello World";
}