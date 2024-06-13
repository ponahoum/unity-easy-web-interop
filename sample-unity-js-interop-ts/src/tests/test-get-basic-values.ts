import { UnityInstance } from "../UnityJSInterop";
import { SampleValues } from "../test-utilities/sample-values";
import { assertEqual } from "../test-utilities/test-asserts";

export function RunTestGetBasicValues(unityInstance: UnityInstance){
    // New tests
    assertEqual(unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestGetBasicValues.GetTestInt().value, SampleValues.TestInt, "TestGetBasicValues.GetTestInt");
    assertEqual(unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestGetBasicValues.GetTestDouble().value, SampleValues.TestDouble, "TestGetBasicValues.GetTestDouble");
    assertEqual(unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestGetBasicValues.GetTestBoolFalse().value, SampleValues.TestBoolFalse, "TestGetBasicValues.GetTestBoolFalse");
    assertEqual(unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestGetBasicValues.GetTestBoolTrue().value, SampleValues.TestBoolTrue, "TestGetBasicValues.GetTestBoolTrue");
    assertEqual(unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestGetBasicValues.GetNullString().value, null, "TestGetBasicValues.GetNullString");
}