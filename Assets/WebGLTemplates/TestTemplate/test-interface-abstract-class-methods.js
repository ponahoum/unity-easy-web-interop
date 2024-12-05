import { assertEquals, assertArrayContentsEquals } from "./testing-utilities.js";
import { SampleValues } from "./testing-utilities.js";

/**
 * Test that inheritance works well for both abstract and interface classes
 */
export function RunTests(module) {
    RunTestAbstractClass(module);
    RunTestInterfaceClass(module);
}

function RunTestAbstractClass(module){
    // Get instance of a method that returns an abstract class
    const instance = module.static.Nahoum.UnityJSInterop.Tests.TestAbstractClass.GetInstance();

    // Ensure a method is well exposed in the final class (inheriting from abstract class) even though not exposed in the inheriting class
    assertEquals(instance.GetString().value, SampleValues.TestString, "TestAbstractMethod.GetString() to get a string from an instance works");
}

function RunTestInterfaceClass(module){
    const instance = module.static.Nahoum.UnityJSInterop.Tests.TestClassImplementingInterface.GetNewInstanceOfInterface();

    // Test calling interface declared methods (method is delcared in interface - aka explicit interface implementation)
    assertEquals(instance.TestGetStringFromClass().value, SampleValues.TestString, "TestClassImplementingInterface.TestGetStringFromClass() to get a string from an implemented interface method in implementing class works");

    // Test getting string from interface method
    assertEquals(instance.TestGetStringFromInterfaceDeclaration().value, SampleValues.TestString, "TestClassImplementingInterface.TestGetStringFromInterfaceDeclaration() to get a string from an interface method in implementing class works");

    // Test calling static method from interface
    assertEquals(module.static.Nahoum.UnityJSInterop.Tests.ITestInterface.TestGetStringFromInterfaceStatic().value, SampleValues.TestString, "ITestInterface.TestGetStringFromInterfaceStatic() to get a string from a static method in interface works");

}