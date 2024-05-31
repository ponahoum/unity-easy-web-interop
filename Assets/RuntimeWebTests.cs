using System;
using UnityEngine;
using System.Threading.Tasks;
using Nahoum.UnityJSInterop;

namespace Nahoum.UnityJSInterop
{
    public class ACoolObject{
        [ExposeWeb]
        public string GetName() => "A cool object";
        [ExposeWeb]
        public int Age() => 25;

        [ExposeWeb]
        public static ACoolObject GetNewInstance() => new ACoolObject();
    }
}

public class RuntimeWebTests
{
    
    [ExposeWeb]
    public static RuntimeWebTests GetNewTestInstance()
    {
        return new RuntimeWebTests();
    }
    
    [ExposeWeb]
    public string MyMethodReturningString() => "returning a random string";
    [ExposeWeb]
    public double MyMethodReturningDouble() => 2147483647125d;
    [ExposeWeb]
    public int MyMethodReturningInt() => 450050554;
    [ExposeWeb]
    public string TestReturnNullValue() => null;
    [ExposeWeb]
    public double AddOneToDouble(double a) => a + 1;
    [ExposeWeb]
    public double[] MethodReturningDoubleArray() => new double[] { 123d, 789d, 101112d };
    [ExposeWeb]
    public string ConcatenateStrings(string a, string b) => a + " - added to -" + b;
    [ExposeWeb]
    public async Task<string> AsyncTaskReturnString()
    {
        await Task.Yield();
        return "A cool result from an async task";
    }
    [ExposeWeb]
    public async Task<string> AsyncTaskStringExplicitelyFail()
    {
        await Task.Yield();
        throw new Exception("This is a test exception from a task");
    }
    [ExposeWeb]
    public async Task AsyncTaskUnraisedException()
    {
        await Task.Yield();
        // This raises an exception because obj is null
        string obj = null;
        obj.ToString();
    }

    [ExposeWeb]
    public async Task AsyncTaskVoidMethod() => await Task.Yield();
    [ExposeWeb]
    public async void AsyncVoidMethod() => await Task.Yield();
    [ExposeWeb]
    public string GetImageInformation(byte[] imageBytes)
    {
        // Load the PNG
        var tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        string result = $"Received image with {imageBytes.Length} bytes and resolution {tex.width}x{tex.height}";

        // Release the texture
        UnityEngine.Object.Destroy(tex);
        return result;
    }

    [ExposeWeb]

    public void InvokeCallbackForTest(Action action)
    {
        action();
    }

    [ExposeWeb]
    public void InvokeCallbackWithActionString(Action<string> action)
    {
        action("Some callback string");
    }

    [ExposeWeb]
    public void InvokeCallbackWithActionStringDouble(Action<string, double> action)
    {
        Debug.Log("Invoking callback with string and double");
        action("Some callback string", 12345);
    }

    [ExposeWeb]
    public void TestInvokedException()
    {
        throw new Exception("This is a test exception");
    }

    [ExposeWeb]
    public void TestUnraisedException()
    {
        string obj = null;
        obj.ToString();
    }
    [ExposeWeb]
    public static string TestStaticMethodReturnsString() => "This is a static method returning a string";
}
