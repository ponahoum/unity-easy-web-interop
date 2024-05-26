using System;
using UnityEngine;
using System.Threading.Tasks;
using Nahoum.EasyWebInterop;

public class RuntimeWebTests : MonoBehaviour
{
    public string MyMethodReturningString() => "returning a random string";
    public double MyMethodReturningDouble() => 2147483647125d;
    public int MyMethodReturningInt() => 450050554;
    public string TestReturnNullValue() => null;
    public double AddOneToDouble(double a) => a + 1;
    public double[] MethodReturningDoubleArray() => new double[] { 123d, 789d, 101112d };
    public string ConcatenateStrings(string a, string b) => a + " - added to -" + b;
    public async Task<string> AsyncTaskReturnString()
    {
        await Task.Yield();
        return "A cool result from an async task";
    }

    public async Task<string> AsyncTaskStringExplicitelyFail()
    {
        await Task.Yield();
        throw new Exception("This is a test exception from a task");
    }

    public async Task AsyncTaskUnraisedException()
    {
        await Task.Yield();
        // This raises an exception because obj is null
        string obj = null;
        obj.ToString();
    }

    public async Task AsyncTaskVoidMethod() => await Task.Yield();
    public async void AsyncVoidMethod() => await Task.Yield();

    public string GetImageInformation(byte[] imageBytes)
    {
        // Load the PNG
        var tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        string result = $"Received image with {imageBytes.Length} bytes and resolution {tex.width}x{tex.height}";

        // Release the texture
        Destroy(tex);
        return result;
    }

    public void InvokeCallbackForTest(Action action)
    {
        action();
    }

    public void InvokeCallbackWithActionString(Action<string> action)
    {
        action("Some callback string");
    }

    public void InvokeCallbackWithActionStringDouble(Action<string, double> action)
    {
        Debug.Log("Invoking callback with string and double");
        action("Some callback string", 12345);
    }

    public void TestInvokedException(){
        throw new Exception("This is a test exception");
    }

    public void TestUnraisedException(){
        Debug.Log("Getting a natural exception");
        string obj = null;
        obj.ToString();
    }

    void Awake()
    {
        // Register methods we want to expose to the nJS side
        MethodsRegistry.RegisterMethod<Func<string>>(nameof(MyMethodReturningString), MyMethodReturningString);
        MethodsRegistry.RegisterMethod<Func<double>>(nameof(MyMethodReturningDouble), MyMethodReturningDouble);
        MethodsRegistry.RegisterMethod<Func<int>>(nameof(MyMethodReturningInt), MyMethodReturningInt);
        MethodsRegistry.RegisterMethod<Func<string>>(nameof(TestReturnNullValue), TestReturnNullValue);
        MethodsRegistry.RegisterMethod<Func<Task<string>>>(nameof(AsyncTaskReturnString), AsyncTaskReturnString);
        MethodsRegistry.RegisterMethod<Func<Task>>(nameof(AsyncTaskVoidMethod), AsyncTaskVoidMethod);
        MethodsRegistry.RegisterMethod<Action>(nameof(AsyncVoidMethod), AsyncVoidMethod);
        MethodsRegistry.RegisterMethod<Func<double[]>>(nameof(MethodReturningDoubleArray), MethodReturningDoubleArray);
        MethodsRegistry.RegisterMethod<Func<string, string, string>>(nameof(ConcatenateStrings), ConcatenateStrings);
        MethodsRegistry.RegisterMethod<Func<double, double>>(nameof(AddOneToDouble), AddOneToDouble);
        MethodsRegistry.RegisterMethod<Func<Task>>(nameof(AsyncTaskStringExplicitelyFail), AsyncTaskStringExplicitelyFail);
        MethodsRegistry.RegisterMethod<Func<byte[], string>>(nameof(GetImageInformation), GetImageInformation);
        MethodsRegistry.RegisterMethod<Action<Action>>(nameof(InvokeCallbackForTest), InvokeCallbackForTest);
        MethodsRegistry.RegisterMethod<Action<Action<string>>>(nameof(InvokeCallbackWithActionString), InvokeCallbackWithActionString);
        MethodsRegistry.RegisterMethod<Action<Action<string, double>>>(nameof(InvokeCallbackWithActionStringDouble), InvokeCallbackWithActionStringDouble);
        MethodsRegistry.RegisterMethod<Action>(nameof(TestInvokedException), TestInvokedException);
        MethodsRegistry.RegisterMethod<Action>(nameof(TestUnraisedException), TestUnraisedException);
        MethodsRegistry.RegisterMethod<Func<Task>>(nameof(AsyncTaskStringExplicitelyFail), AsyncTaskStringExplicitelyFail);
    }
}
