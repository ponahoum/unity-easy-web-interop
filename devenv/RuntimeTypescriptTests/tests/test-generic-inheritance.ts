import { UnityInstance } from "../UnityInstance.js";
import { assertEquals } from "./test-utilities.js";

/**
 * Few tests to check that we can call class inheriting from generic classes, and use the interfaces with it too
 */
export function RunTests(unityInstance: UnityInstance) {

  // Get module
  const module = unityInstance.Module;
  
  // Get instance
  const instance = module.static["Nahoum.UnityJSInterop.Tests"].TestGenericClass.CreateInstance();

  // Check "GetTypeName" return the name of TestGenericClass
  assertEquals(instance.GetTypeName().value, "Int32", "Can inherit methods from generic class");

  // Create instance of TestAnotherGenericClass, another generic class
    const anotherInstance = module.static["Nahoum.UnityJSInterop.Tests"].TestAnotherGenericClass.CreateInstance();
    assertEquals(anotherInstance.GetTypeName().value, "Double", "Can inherit methods from another generic class");

    // Test that we can use the first instance as an interface and inject it in the second instance
    assertEquals(anotherInstance.GetTypeNameFromExternal(instance).value, "Int32", "Can use the first instance as an interface in the second instance as argument from a method");


    // Test that the second instance can be used as an interface in the first instance
    assertEquals(instance.GetTypeNameFromExternal(anotherInstance).value, "Double", "Can use the second instance as an interface in the first instance as argument from a method");
}
