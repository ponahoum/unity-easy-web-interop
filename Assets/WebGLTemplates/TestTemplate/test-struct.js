import { assertEquals } from "./testing-utilities.js";
import { SampleValues } from "./testing-utilities.js";

/**
 * Test playing with structs - Remember that structs are passed by reference (boxed) to JS
 */
export async function RunTests(module) {
  const structBoxed = module.static.Nahoum.UnityJSInterop.Tests.TestStructExpose.GetExposeInstance();

  assertEquals(structBoxed.value.A, SampleValues.TestInt, "Passed getting int value in struct");
  assertEquals(structBoxed.value.F, SampleValues.TestFloat, "Passed getting float value in struct");
  assertEquals(structBoxed.value.Name, SampleValues.TestString, "Passed getting string value in struct");
}
