import { UnityInstance } from "../UnityInstance.js";
import { assertEquals, SampleValues } from "./test-utilities.js";

export async function RunTests(unityInstance: UnityInstance) {

  // Get module
  const module = unityInstance.Module;

  // Get instance
  const instance = module.static["Nahoum.UnityJSInterop.Tests"].TestTasks.GetInstance();

  // Tasks returning void should return undefined in javascript
  assertEquals(await instance.TestTaskVoid(), undefined, "Asynchonous void Task");

  // Test Task returning string
  assertEquals((await instance.TestTaskString()).value, SampleValues.TestString, "Asynchronous Task<string>");

  // Test Task returning int
  assertEquals((await instance.TestTaskInt()).value, SampleValues.TestInt, "Asynchronous Task<int>");

  // Test Task returning float
  assertEquals((await instance.TestTaskFloat()).value, SampleValues.TestFloat, "Asynchronous Task<float>");

  // Test async void method
  assertEquals(await instance.AsyncVoidMethod(), undefined, "Asynchronous 'async void' method");
}
