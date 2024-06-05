namespace  {
    export type TestGenericClass<T>_static = {
        GetTypeName(): System.String;
    }
    export type RuntimeWebTests_static = {
        GetNewTestInstance(): RuntimeWebTests;
        TestStaticMethodReturnsString(): System.String;
    }
    export type RuntimeWebTests = {
        MyMethodReturningString(): System.String;
        MyMethodReturningDouble(): System.Double;
        MyMethodReturningInt(): System.Int32;
        TestReturnNullValue(): System.String;
        AddOneToDouble(a: System.Double): System.Double;
        MethodReturningDoubleArray(): System.System.DoubleArr;
        ConcatenateStrings(a: System.String, b: System.String): System.String;
        AsyncTaskReturnString(): System.Threading.Tasks.Task<System.String>;
        AsyncTaskStringExplicitelyFail(): System.Threading.Tasks.Task<System.String>;
        AsyncTaskUnraisedException(): System.Threading.Tasks.Task;
        AsyncTaskVoidMethod(): System.Threading.Tasks.Task;
        AsyncVoidMethod(): System.Void;
        GetImageInformation(imageBytes: System.System.ByteArr): System.String;
        InvokeCallbackForTest(action: System.Action): System.Void;
        InvokeCallbackWithActionString(action: System.Action<System.String>): System.Void;
        InvokeCallbackWithActionStringDouble(action: System.Action<System.String, System.Double>): System.Void;
        TestInvokedException(): System.Void;
        TestUnraisedException(): System.Void;
    }
        & RuntimeWebTests_static
}
namespace Nahoum.UnityJSInterop {
    export type ACoolObject_static = {
        GetNewInstance(): ACoolObject;
    }
    export type ACoolObject = {
        GetName(): System.String;
        Age(): System.Int32;
    }
        & ACoolObject_static
}


type UnityInstance = {
    Module: {
        static: {
            RuntimeWebTests: RuntimeWebTests_static;
        },
    }

}

export { UnityInstance };