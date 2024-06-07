export type RuntimeWebTests_static = {
    key: 'RuntimeWebTests';
    GetNewTestInstance(): RuntimeWebTests;
    TestStaticMethodReturnsString(): System.String;
}
export type RuntimeWebTests = {
    key: 'RuntimeWebTests';
    MyMethodReturningString(): System.String;
    MyMethodReturningDouble(): System.Double;
    MyMethodReturningInt(): System.Int32;
    TestReturnNullValue(): System.String;
    AddOneToDouble(a: System.Double): System.Double;
    MethodReturningDoubleArray(): CSharpArr<System.System.Double>;
    ConcatenateStrings(a: System.String, b: System.String): System.String;
    AsyncTaskReturnString(): System.Threading.Tasks.Task1$System_String;
    AsyncTaskStringExplicitelyFail(): System.Threading.Tasks.Task1$System_String;
    AsyncTaskUnraisedException(): System.Threading.Tasks.Task;
    AsyncTaskVoidMethod(): System.Threading.Tasks.Task;
    AsyncVoidMethod(): System.Void;
    GetImageInformation(imageBytes: System.System.ByteArr): System.String;
    InvokeCallbackForTest(action: System.Action): System.Void;
    InvokeCallbackWithActionString(action: System.MulticastDelegate1$String): System.Void;
    InvokeCallbackWithActionStringDouble(action: System.MulticastDelegate2$String$Double): System.Void;
    TestInvokedException(): System.Void;
    TestUnraisedException(): System.Void;
}
    & RuntimeWebTests_static; export namespace System {
        export type String = {
            key: 'System.String';
        }
        export type Double = {
            key: 'System.Double';
        }
        export type Int32 = {
            key: 'System.Int32';
        }
        export type DoubleArr = {
            key: 'System.Double[]';
        }
        export type Void = {
            key: 'System.Void';
        }
        export type ByteArr = {
            key: 'System.Byte[]';
        }
        export type Action = {
            key: 'System.Action';
        }
        export type MulticastDelegate1$String = {
            key: 'System.Action`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]';
        }
        export type MulticastDelegate2$String$Double = {
            key: 'System.Action`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]';
        }
    }
export namespace System.Threading.Tasks {
    export type Task1$System_String = {
        key: 'System.Threading.Tasks.Task`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]';
    }
    export type Task = {
        key: 'System.Threading.Tasks.Task';
    }
}
export namespace Nahoum.UnityJSInterop {
    export type ACoolObject_static = {
        key: 'Nahoum.UnityJSInterop.ACoolObject';
        GetNewInstance(): ACoolObject;
    }
    export type ACoolObject = {
        key: 'Nahoum.UnityJSInterop.ACoolObject';
        GetName(): System.String;
        Age(): System.Int32;
    }
        & ACoolObject_static;
}



type UnityInstance = {
    Module: {
        static: {
            RuntimeWebTests: RuntimeWebTests_static;
            Nahoum: {
                UnityJSInterop: {
                    ACoolObject: Nahoum.UnityJSInterop.ACoolObject_static;
                }
            }
        },
    }

}

export { UnityInstance };