// The following is generated from the main template
// Contains all hardcoded utility types and functions for the generated TypeScript file to work correctly

export type CSharpArray<T> = {
    key: 'CSharpArray';
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
            /*STATIC_MODULE_PLACEHOLDER*/
        },
        utilities: Utilities
    }
}
