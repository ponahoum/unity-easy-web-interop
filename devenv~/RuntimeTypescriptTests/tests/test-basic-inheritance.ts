import { UnityInstance } from "../UnityInstance.js";
import { assertEquals, SampleValues } from "./test-utilities.js";

export function RunTests(unityInstance: UnityInstance) {

    // Get module
    const module = unityInstance.Module;

    // Test TestBasicInheritance
    const testBasicInheritanceInstance = module.static["Nahoum.UnityJSInterop.Tests"].TestBasicInheritance.GetInstance();
    assertEquals(testBasicInheritanceInstance.GetString().value, SampleValues.TestString, "TestBasicInheritance.GetString works");

    // Test SuperTestBasicInheritance
    const superTestBasicInheritanceInstance = module.static["Nahoum.UnityJSInterop.Tests"].SuperTestBasicInheritance.GetInstanceSuper();
    assertEquals(superTestBasicInheritanceInstance.GetString().value, SampleValues.TestString, "SuperTestBasicInheritance.GetString works");
    assertEquals(superTestBasicInheritanceInstance.GetAsBaseClass().GetString().value, SampleValues.TestString, "SuperTestBasicInheritance.GetAsBaseClass works");
}