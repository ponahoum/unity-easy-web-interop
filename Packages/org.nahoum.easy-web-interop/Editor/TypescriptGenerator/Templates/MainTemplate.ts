// The following is generated from the main template
// Contains all hardcoded utility types and functions for the generated TypeScript file to work correctly

export type Utilities = {
    GetManagedAction(callback: (actionParameters: any) => void, managedTypesArray: string[]): any,
    GetManagedAction(callback: () => void): System.Action,
    GetManagedBool(targetBool: boolean): System.Boolean,
    GetManagedBoolArray(array: boolean[]): System.Boolean_CSharpArray,
    GetManagedByteArray(array: Uint8Array): System.Byte_CSharpArray,
    GetManagedDouble(targetNumber: number): System.Double,
    GetManagedDoubleArray(array: number[]): System.Double_CSharpArray,
    GetManagedFloat(targetNumber: number): System.Single,
    GetManagedFloatArray(array: number[]): System.Single_CSharpArray,
    GetManagedInt(targetNumber: number): System.Int32,
    GetManagedIntArray(array: number[]): System.Int32_CSharpArray,
    GetManagedLong(targetNumber: number): System.Int64,
    GetManagedString(jsStr: string): System.String,
    GetManagedStringArray(array: string[]): System.String_CSharpArray,
}

export type UnityInstance = {
    Module: {
        static: {
            /*STATIC_MODULE_PLACEHOLDER*/
        },
        utilities: Utilities
    }
}
