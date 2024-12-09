import { assertEquals, assertArrayContentsEquals, assertObjectEqualsByValues  } from "./testing-utilities.js";
import { SampleValues } from "./testing-utilities.js";

/**
 * Test getters and setters on a very basic static class
 */
export function RunTests(module) {

    // Get current value of string and assert it is samples
    assertEquals(window.Module.static["Nahoum.UnityJSInterop.Tests"].TestGetSets.get_TestString().value, SampleValues.TestString, "Getters work with get_TestString");

    // Now create a managed string
    const managedString = window.Module.utilities.GetManagedString("123");

    // Set the value of string
    window.Module.static["Nahoum.UnityJSInterop.Tests"].TestGetSets.set_TestString(managedString);

    // Get the value of string and assert it is the new value
    assertEquals(window.Module.static["Nahoum.UnityJSInterop.Tests"].TestGetSets.get_TestString().value, "123", "Setters work with set_TestString");
}