import { UnityInstance } from "../UnityInstance";
import { assertEquals, SampleValues } from "./test-utilities.js";

export function RunTests(unityInstance: UnityInstance) {

    // Get module
    const module = unityInstance.Module;

    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetBasicValues.GetTestString().value, SampleValues.TestString, "GetTestString in a static class as a classic method works");
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetBasicValues.GetTestDouble().value, SampleValues.TestDouble, "GetTestDouble in a static class as a classic method works");
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetBasicValues.GetTestInt().value, SampleValues.TestInt, "GetTestInt in a static class as a classic method works");
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetBasicValues.GetTestBoolTrue().value, true, "GetTestBoolTrue in a static class as a classic method works");
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetBasicValues.GetTestBoolFalse().value, false, "GetTestBoolFalse in a static class as a classic method works");
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetBasicValues.GetNullString().value, null, "GetNullString in a static class as a classic method works");
}