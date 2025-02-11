import { UnityInstance } from "../UnityInstance.js";
import { assertArrayContentsEquals, assertEquals } from "./test-utilities.js";

/**
 * Test all default embedded utilities
 */
export function RunTests(module: UnityInstance) {
  const utilities = module.Module.utilities;

  // Primitives getters
  assertEquals(utilities.GetManagedInt(123456).value, 123456, "GetInt works");
  assertEquals(utilities.GetManagedLong(123456).value, 123456, "GetLong works");
  assertEquals(utilities.GetManagedFloat(123456.3).value, 123456.3, "GetFloat works (around 7 decimal digits)");
  assertEquals(utilities.GetManagedDouble(123456.3339944).value, 123456.3339944, "GetDouble works (around 13 decimal digits");
  assertEquals(utilities.GetManagedBool(true).value, true, "GetBool (true) works");
  assertEquals(utilities.GetManagedBool(false).value, false, "GetBool (false) works");
  assertEquals(utilities.GetManagedString("A string").value, "A string", "GetString works");

  // Primitive arrays getter
  assertArrayContentsEquals(utilities.GetManagedIntArray([1, 2, 3]).value, [1, 2, 3], "GetIntArray works");
  assertArrayContentsEquals(utilities.GetManagedDoubleArray([1.1, 2.2, 3.3]).value, [1.1, 2.2, 3.3], "GetDoubleArray works");
  assertArrayContentsEquals(utilities.GetManagedFloatArray([1.1, 2.2, 3.3]).value, [1.1, 2.2, 3.3], "GetFloatArray works");
  assertArrayContentsEquals(utilities.GetManagedBoolArray([true, false, true]).value, [true, false, true], "GetBoolArray works");
  assertArrayContentsEquals(utilities.GetManagedByteArray(new Uint8Array([1, 2, 3])).value, [1, 2, 3], "GetByteArray works");
}
