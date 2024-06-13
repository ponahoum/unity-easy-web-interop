import { UnityInstance } from "../UnityJSInterop";
import { SampleValues } from "../test-utilities/sample-values";
import { assertArrayEqual, assertEqual } from "../test-utilities/test-asserts";

export function RunTestInstanceMethods(unityInstance: UnityInstance){
    const instance = unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestInstanceMethods.GetNewInstance();

    // Get tests
    assertEqual(instance.TestGetDouble().value, SampleValues.TestDouble, "TestInstanceMethods.TestGetDouble");
    assertEqual(instance.TestGetBoolFalse().value, SampleValues.TestBoolFalse, "TestInstanceMethods.TestGetBoolFalse");
    assertEqual(instance.TestGetBoolTrue().value, SampleValues.TestBoolTrue, "TestInstanceMethods.TestGetBoolTrue");
    assertEqual(instance.TestGetInt().value, SampleValues.TestInt, "TestInstanceMethods.TestGetInt");
    assertEqual(instance.TestGetString().value, SampleValues.TestString, "TestInstanceMethods.TestGetString");
    assertEqual(instance.TestGetNullString().value, null, "TestInstanceMethods.TestGetNullString");
    assertEqual(instance.TestGetFloat().value, SampleValues.TestFloat, "TestInstanceMethods.TestGetFloat");
    assertArrayEqual(instance.TestGetFloatArray().value, SampleValues.TestFloatArray, "TestInstanceMethods.TestGetFloatArray");

    // Addition double
    var doubleA = unityInstance.Module.utilities.GetManagedDouble(42);
    var doubleB = unityInstance.Module.utilities.GetManagedDouble(21);
    assertEqual(instance.TestAdditionDouble(doubleA, doubleB).value, 63, "TestInstanceMethods.TestAdditionDouble");

    // Addition float
    var floatA = unityInstance.Module.utilities.GetManagedFloat(42);
    var floatB = unityInstance.Module.utilities.GetManagedFloat(21);
    assertEqual(instance.TestAdditionFloat(floatA, floatB).value, 63, "TestInstanceMethods.TestAdditionFloat");

    // Addition string
    var stringA = unityInstance.Module.utilities.GetManagedString("Hello");
    var stringB = unityInstance.Module.utilities.GetManagedString(" World");
    assertEqual(instance.TestAdditionString(stringA, stringB).value, "Hello World", "TestInstanceMethods.TestAdditionString");
    
}