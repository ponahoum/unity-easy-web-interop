import { assertThrowsAsync, assertThrows } from "./testing-utilities.js";
import { DefaultExceptionMessage } from "./testing-utilities.js";

/**
 * Few tests to check exception handling, in both sync and async methods
 */
export async function RunTests(module) {
  // Get static class
  const staticClass = module.static["Nahoum.UnityJSInterop.Tests"].TestExceptions;

  // Test regular exception
  assertThrows(staticClass.ThrowSimpleException, DefaultExceptionMessage, "ThrowSimpleException");
  assertThrows(staticClass.TestUnraisedException, DefaultExceptionMessage, "TestUnraisedException");
  await assertThrowsAsync(staticClass.ThrowSimpleExceptionAsync, DefaultExceptionMessage, "ThrowSimpleExceptionAsync");

  // Not supported yet
  //await assertThrowsAsync(staticClass.TestUnraisedExceptionAsync, DefaultExceptionMessage, "TestUnraisedExceptionAsync");
}
