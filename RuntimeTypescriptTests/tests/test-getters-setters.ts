
import { UnityInstance } from "../UnityInstance.js";
import { assertEquals, SampleValues } from "./test-utilities.js";

/**
 * Test getters and setters on a very basic static class
 */
export function RunTests(unityInstance: UnityInstance) {

  // Get module
  const module = unityInstance.Module;

    // Get current value of string and assert it is samples
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetSets.get_TestString().value, SampleValues.TestString, "Getters work with get_TestString");

    // Now create a managed string
    const managedString = module.utilities.GetManagedString("123");

    // Set the value of string
    module.static["Nahoum.UnityJSInterop.Tests"].TestGetSets.set_TestString(managedString);

    // Get the value of string and assert it is the new value
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetSets.get_TestString().value, "123", "Setters work with set_TestString");


    // Now try decorator on the property itself
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetSets.get_TestString2().value, SampleValues.TestString2, "Getters on properties work with get_TestString2");

    // Now try to set the value
    module.static["Nahoum.UnityJSInterop.Tests"].TestGetSets.set_TestString2(managedString);

    // Get the value of string and assert it is the new value
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].TestGetSets.get_TestString2().value, "123", "Getters on properties work with set_TestString2");
}