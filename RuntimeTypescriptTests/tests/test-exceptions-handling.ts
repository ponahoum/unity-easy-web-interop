import { UnityInstance } from "../UnityInstance.js";
import { assertThrows, assertThrowsAsync, DefaultExceptionMessage } from "./test-utilities.js";
/**
 * Few tests to check exception handling, in both sync and async methods
 */
export async function RunTests(unityInstance: UnityInstance) {

  // Get module
  const module = unityInstance.Module;
  
  // Get static class
  const staticClass = module.static["Nahoum.UnityJSInterop.Tests"].TestExceptions;

  // Test regular exception
  assertThrows(staticClass.ThrowSimpleException, DefaultExceptionMessage, "ThrowSimpleException");
  assertThrows(staticClass.TestUnraisedException, DefaultExceptionMessage, "TestUnraisedException");
  await assertThrowsAsync(staticClass.ThrowSimpleExceptionAsync, DefaultExceptionMessage, "ThrowSimpleExceptionAsync");

  // Not supported yet
  //await assertThrowsAsync(staticClass.TestUnraisedExceptionAsync, DefaultExceptionMessage, "TestUnraisedExceptionAsync");
}
