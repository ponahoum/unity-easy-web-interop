import { UnityInstance } from "../UnityJSInterop";
import { SampleValues } from "../test-utilities/sample-values";
import { assertEqual } from "../test-utilities/test-asserts";
export function RunTestCallbackActions(unityInstance: UnityInstance): void {
    const instance = unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestActionCallbacks.GetInstance();

    // Create callback that takes a string a an argument and invokes it
    var gatheredValueString = undefined;
    var actionString = unityInstance.Module.utilities.GetManagedAction((value) => {
        gatheredValueString = value.value
    }, ['System.String']);
    instance.TestInvokeCallbackString(actionString);
    assertEqual(gatheredValueString, SampleValues.TestString, "TestInvokeCallbackString");

    // Create callback that takes a float a an argument and invokes it
    var gatheredValueNumber = undefined;
    var actionNumber = unityInstance.Module.utilities.GetManagedAction((value) => {
        gatheredValueNumber = value.value
    }, ['System.Single']);
    instance.TestInvokeCallbackFloat(actionNumber);
    assertEqual(gatheredValueNumber, SampleValues.TestFloat, "TestInvokeCallbackFloat");

    // Next steps: find a way to get the actions and replace the ugly array on types
    // We notably have to test Action<Type outside of System namespace>
}