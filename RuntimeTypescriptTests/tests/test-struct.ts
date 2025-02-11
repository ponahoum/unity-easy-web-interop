import { UnityInstance } from "../UnityInstance.js";
import { assertEquals, SampleValues } from "./test-utilities.js";


/**
 * Test playing with structs - Remember that structs are passed by reference (boxed) to JS
 */
export function RunTests(unityInstance: UnityInstance) {

  // Get module
  const module = unityInstance.Module;

  const structBoxed = module.static["Nahoum.UnityJSInterop.Tests"].TestStructExpose.GetExposeInstance();

  assertEquals(structBoxed.value.A, SampleValues.TestInt, "Passed getting int value in struct");
  assertEquals(structBoxed.value.F, SampleValues.TestFloat, "Passed getting float value in struct");
  assertEquals(structBoxed.value.Name, SampleValues.TestString, "Passed getting string value in struct");
}
