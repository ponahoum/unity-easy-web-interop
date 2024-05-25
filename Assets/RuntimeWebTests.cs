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

    public async Task<string> AsyncTaskStringFail()
    {
        await Task.Yield();
        throw new Exception("This is a test exception");
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
        MethodsRegistry.RegisterMethod<Func<Task>>(nameof(AsyncTaskStringFail), AsyncTaskStringFail);
        MethodsRegistry.RegisterMethod<Func<byte[], string>>(nameof(GetImageInformation), GetImageInformation);
    }
}
