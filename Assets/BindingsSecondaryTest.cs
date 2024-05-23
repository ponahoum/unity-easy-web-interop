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

    public async void InfiniteAsyncFunction(){
        while(true){
            // Wait for 60 frames
            for(int i = 0; i < 60; i++){
                await Task.Yield();
            }
            Debug.Log("I am running forever");
        }
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
        ServiceRegisterer.RegisterMethod<Func<string>>(nameof(MyMethodReturningString), MyMethodReturningString);
        ServiceRegisterer.RegisterMethod<Func<double>>(nameof(MyMethodReturningDouble), MyMethodReturningDouble);
        ServiceRegisterer.RegisterMethod<Func<int>>(nameof(MyMethodReturningInt), MyMethodReturningInt);
        ServiceRegisterer.RegisterMethod<Func<string>>(nameof(TestReturnNullValue), TestReturnNullValue);
        ServiceRegisterer.RegisterMethod<Func<Task<string>>>(nameof(ComputeStuff), ComputeStuff);
        ServiceRegisterer.RegisterMethod<Func<double[]>>(nameof(MeMethodReturningDoubleArray), MeMethodReturningDoubleArray);
        ServiceRegisterer.RegisterMethod<Func<Task<string>, string>>(nameof(GetStringIfTaskComplete), GetStringIfTaskComplete);
        ServiceRegisterer.RegisterMethod<Func<Action<string>, Task<string>>>(nameof(ComputeStuffAndRaiseCallback), ComputeStuffAndRaiseCallback);
        ServiceRegisterer.RegisterMethod<Func<string, string, string>>(nameof(ConcatenateStrings), ConcatenateStrings);
        ServiceRegisterer.RegisterMethod<Func<double, double>>(nameof(AddOneToDouble), AddOneToDouble);
        ServiceRegisterer.RegisterMethod<Action>(nameof(InfiniteAsyncFunction), InfiniteAsyncFunction);

        // Possibility to get an action
        ServiceRegisterer.RegisterGetActionStringFromPtr();
    }
}
