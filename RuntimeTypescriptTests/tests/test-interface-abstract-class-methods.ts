
import { UnityInstance } from "../UnityInstance.js";
import { assertEquals, SampleValues } from "./test-utilities.js";

/**
 * Test that inheritance works well for both abstract and interface classes
 */
export function RunTests(unityInstance: UnityInstance) {

    RunTestAbstractClass(unityInstance);
    RunTestInterfaceClass(unityInstance);
}

function RunTestAbstractClass(unityInstance: UnityInstance) {
    const module = unityInstance.Module;
    // Get instance of a method that returns an abstract class
    const instance = module.static["Nahoum.UnityJSInterop.Tests"].TestAbstractClass.GetInstance();

    // Ensure a method is well exposed in the final class (inheriting from abstract class) even though not exposed in the inheriting class
    assertEquals(instance.GetString().value, SampleValues.TestString, "TestAbstractMethod.GetString() to get a string from an instance works");
}

function RunTestInterfaceClass(unityInstance: UnityInstance) {
    const module = unityInstance.Module;
    const instance = module.static["Nahoum.UnityJSInterop.Tests"].TestClassImplementingInterface.GetNewInstanceOfInterface();

    // Test calling interface declared methods (method is declared in interface - aka explicit default interface implementation)
    // This default implementation is not supported if not in the instance
    console.log(instance);
    assertEquals(instance.TestGetStringFromInterface().value, SampleValues.TestString2, "TestClassImplementingInterface.TestGetStringFromClass() to get a string from an implemented interface method in implementing class works");

    // Test getting string from interface method
    assertEquals(instance.TestGetStringFromInterfaceDeclaration().value, SampleValues.TestString, "TestClassImplementingInterface.TestGetStringFromInterfaceDeclaration() to get a string from an interface method in implementing class works");

    // Test calling static method from interface
    assertEquals(module.static["Nahoum.UnityJSInterop.Tests"].ITestInterface.TestGetStringFromInterfaceStatic().value, SampleValues.TestString, "ITestInterface.TestGetStringFromInterfaceStatic() to get a string from a static method in interface works");

}