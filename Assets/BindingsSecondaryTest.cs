using System;
using UnityEngine;
using System.Threading.Tasks;
using PoNah.EasyWebInterop;

public class BindingsSecondaryTest : MonoBehaviour
{
    public string MyMethodReturningString() => "returning a random string";
    public double MyMethodReturningDouble() => 45005055454544545454545454545545455d;
    public int MyMethodReturningInt() => 450050554;

    public string TestReturnNullValue() => null;
    public double AddOneToDouble(double a) => a + 1;
    public double[] MeMethodReturningDoubleArray() => new double[] { 123d, 789d, 101112d };
    public string ConcatenateStrings(string a, string b) => a + " - added to -" + b;
    public async Task<string> ComputeStuff()
    {
        int i = 0;
        Debug.Log("Starting to compute stuff");
        while (i < 3)
        {
            await Task.Yield();
            Debug.Log("Computing stuff " + i);
            i++;
        }
        Debug.Log("Done computing stuff");
        return "Yeahh were done";
    }

    public async Task<string> ComputeStuffAndRaiseCallback(Action<string> targetAction)
    {
        int i = 0;
        Debug.Log("Starting to compute stuff");
        while (i < 3)
        {
            await Task.Yield();
            Debug.Log("Computing stuff " + i);
            i++;
        }
        Debug.Log("Done computing stuff");
        targetAction("Yeahh were done");
        return "Yeahh were done";
    }

    public T GetStringIfTaskComplete<T>(Task<T> targetTask)
    {
        if (targetTask.IsCompleted)
        {
            return targetTask.Result;
        }
        return default;
    }

    void Awake()
    {
        // Setup the service register
        ServiceRegisterer.Setup();

        // Register methods we want to expose to the nJS side
        ServiceRegisterer.RegisterMethod(nameof(MyMethodReturningString), MyMethodReturningString);
        ServiceRegisterer.RegisterMethod(nameof(MyMethodReturningDouble), MyMethodReturningDouble);
        ServiceRegisterer.RegisterMethod(nameof(MyMethodReturningInt), MyMethodReturningInt);
        ServiceRegisterer.RegisterMethod(nameof(TestReturnNullValue), TestReturnNullValue);
        ServiceRegisterer.RegisterMethod(nameof(ComputeStuff), ComputeStuff);
        ServiceRegisterer.RegisterMethod(nameof(MeMethodReturningDoubleArray), MeMethodReturningDoubleArray);
        ServiceRegisterer.RegisterMethod<Task<string>, string>(nameof(GetStringIfTaskComplete), GetStringIfTaskComplete);
        ServiceRegisterer.RegisterMethod<Action<string>, Task<string>>(nameof(ComputeStuffAndRaiseCallback), ComputeStuffAndRaiseCallback);
        ServiceRegisterer.RegisterMethod<string, string, string>(nameof(ConcatenateStrings), ConcatenateStrings);
        ServiceRegisterer.RegisterMethod<double, double>(nameof(AddOneToDouble), AddOneToDouble);

        // Possibility to get an action
        ServiceRegisterer.RegisterGetActionStringFromPtr();
    }
}
