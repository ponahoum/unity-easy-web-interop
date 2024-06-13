export namespace Nahoum.UnityJSInterop.Tests {
    export type TestActionCallbacks = {
        key: 'Nahoum.UnityJSInterop.Tests.TestActionCallbacks';
        TestInvokeCallbackVoid(action: System.Action): void;
        TestInvokeCallbackInt(action: System.MulticastDelegate1$Int32): void;
        TestInvokeCallbackString(action: System.MulticastDelegate1$String): void;
        TestInvokeCallbackFloat(action: System.MulticastDelegate1$Single): void;
        TestInvokeDoubleStringCallback(action: System.MulticastDelegate2$Double$String): void;
    }
    export type TestTasks_static = {
        key: 'Nahoum.UnityJSInterop.Tests.TestTasks';
        GetInstance(): TestTasks;
    }
    export type TestTasks = {
        key: 'Nahoum.UnityJSInterop.Tests.TestTasks';
        TestTaskVoid(): Promise<void>;
        TestTaskString(): Promise<System.String>;
        TestTaskInt(): Promise<System.Int32>;
        TestTaskFloat(): Promise<System.Single>;
        AsyncVoidMethod(): void;
    }
        & TestTasks_static; export type TestExceptions_static = {
            key: 'Nahoum.UnityJSInterop.Tests.TestExceptions';
            ThrowSimpleException(): void;
        }
    export type TestExceptions = {
        key: 'Nahoum.UnityJSInterop.Tests.TestExceptions';
        TestUnraisedException(): void;
        ThrowSimpleExceptionAsync(): Promise<void>;
        TestUnraisedExceptionAsync(): Promise<void>;
    }
        & TestExceptions_static; export type TestGetBasicValues_static = {
            key: 'Nahoum.UnityJSInterop.Tests.TestGetBasicValues';
            GetTestString(): System.String;
            GetTestDouble(): System.Double;
            GetTestInt(): System.Int32;
            GetTestBoolTrue(): System.Boolean;
            GetTestBoolFalse(): System.Boolean;
            GetNullString(): System.String;
        }
    export type TestGetBasicValues = {
        key: 'Nahoum.UnityJSInterop.Tests.TestGetBasicValues';
    }
        & TestGetBasicValues_static; export type TestInstanceMethods_static = {
            key: 'Nahoum.UnityJSInterop.Tests.TestInstanceMethods';
            GetNewInstance(): TestInstanceMethods;
        }
    export type TestInstanceMethods = {
        key: 'Nahoum.UnityJSInterop.Tests.TestInstanceMethods';
        TestGetString(): System.String;
        TestGetDouble(): System.Double;
        TestGetInt(): System.Int32;
        TestGetFloat(): System.Single;
        TestGetFloatArray(): CSharpArray<System.Single>;
        TestGetDoubleArray(): CSharpArray<System.Double>;
        TestGetBoolTrue(): System.Boolean;
        TestGetBoolFalse(): System.Boolean;
        TestGetNullString(): System.String;
        TestAdditionInt(a: System.Int32, b: System.Int32): System.Int32;
        TestAdditionFloat(a: System.Single, b: System.Single): System.Single;
        TestAdditionDouble(a: System.Double, b: System.Double): System.Double;
        TestAdditionString(a: System.String, b: System.String): System.String;
    }
        & TestInstanceMethods_static; export type TestLoadTexture_static = {
            key: 'Nahoum.UnityJSInterop.Tests.TestLoadTexture';
            LoadTexture(imageBytes: CSharpArray<System.Byte>): UnityEngine.Texture2D;
            GetTextureResolution(tex: UnityEngine.Texture2D): System.String;
            DestroyTexture(tex: UnityEngine.Texture2D): void;
        }
}
export namespace System {
    export type MulticastDelegate1$Int32 = {
        key: 'System.Action`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]';
    }
    export type MulticastDelegate1$String = {
        key: 'System.Action`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]';
    }
    export type MulticastDelegate1$Single = {
        key: 'System.Action`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]';
    }
    export type MulticastDelegate2$Double$String = {
        key: 'System.Action`2[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]';
    }
}
export namespace UnityEngine {
    export type Texture2D = {
        key: 'UnityEngine.Texture2D';
    }
}
// The following is generated from the main template
// Contains all hardcoded utility types and functions for the generated TypeScript file to work correctly

export type CSharpArray<T> = {
    key: 'CSharpArray ';
    value: T[];
}

export namespace System {
    export type String = {
        key: 'System.String';
    }
    export type Double = {
        key: 'System.Double';
    }
    export type Int32 = {
        key: 'System.Int32';
    }
    export type Byte = {
        key: 'System.Byte';
    }
    export type Boolean = {
        key: 'System.Boolean';
    }
    export type Single = {
        key: 'System.Single';
    }
    export type Int64 = {
        key: 'System.Int64';
    }
    export type Action = {
        key: 'System.Action';
    }
}

export type Utilities = {
    GetManagedAction(callback: (actionParameters: any) => void, managedTypesArray: string[]): any,
    GetManagedAction(callback: () => void): System.Action,
    GetManagedBool(targetBool: boolean): System.Boolean,
    GetManagedBoolArray(array: boolean[]): CSharpArray<System.Boolean>,
    GetManagedByteArray(array: Uint8Array): CSharpArray<System.Byte>,
    GetManagedDouble(targetNumber: number): System.Double,
    GetManagedDoubleArray(array: number[]): CSharpArray<System.Double>,
    GetManagedFloat(targetNumber: number): System.Single,
    GetManagedFloatArray(array: number[]): CSharpArray<System.Single>,
    GetManagedInt(targetNumber: number): System.Int32,
    GetManagedIntArray(array: number[]): CSharpArray<System.Int32>,
    GetManagedLong(targetNumber: number): System.Int64,
    GetManagedString(jsStr: string): System.String,
    GetManagedStringArray(array: string[]): CSharpArray<System.String>,
}

export type UnityInstance = {
    Module: {
        static: {
            Nahoum: {
                UnityJSInterop: {
                    Tests: {
                        TestTasks: Nahoum.UnityJSInterop.Tests.TestTasks_static;
                        TestExceptions: Nahoum.UnityJSInterop.Tests.TestExceptions_static;
                        TestGetBasicValues: Nahoum.UnityJSInterop.Tests.TestGetBasicValues_static;
                        TestInstanceMethods: Nahoum.UnityJSInterop.Tests.TestInstanceMethods_static;
                        TestLoadTexture: Nahoum.UnityJSInterop.Tests.TestLoadTexture_static;
                    };
                };
            };
            System: {
            };
            UnityEngine: {
            };

        },
        utilities: Utilities
    }
}
