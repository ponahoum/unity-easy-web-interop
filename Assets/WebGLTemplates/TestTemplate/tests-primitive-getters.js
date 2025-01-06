import { assertEquals, assertArrayContentsEquals } from "./testing-utilities.js";

/**
 * Test all default embedded utilities
 */
export function RunTests(module) {
  // Primitives getters
  assertEquals(module.utilities.GetManagedInt(123456).value, 123456, "GetInt works");
  assertEquals(module.utilities.GetManagedLong(123456).value, 123456, "GetLong works");
  assertEquals(module.utilities.GetManagedFloat(123456.3).value, 123456.3, "GetFloat works (around 7 decimal digits)");
  assertEquals(module.utilities.GetManagedDouble(123456.3339944).value, 123456.3339944, "GetDouble works (around 13 decimal digits");
  assertEquals(module.utilities.GetManagedBool(true).value, true, "GetBool (true) works");
  assertEquals(module.utilities.GetManagedBool(false).value, false, "GetBool (false) works");
  assertEquals(module.utilities.GetManagedString("A string").value, "A string", "GetString works");

  // Primitive arrays getter
  assertArrayContentsEquals(module.utilities.GetManagedIntArray([1, 2, 3]).value, [1, 2, 3], "GetIntArray works");
  assertArrayContentsEquals(module.utilities.GetManagedDoubleArray([1.1, 2.2, 3.3]).value, [1.1, 2.2, 3.3], "GetDoubleArray works");
  assertArrayContentsEquals(module.utilities.GetManagedFloatArray([1.1, 2.2, 3.3]).value, [1.1, 2.2, 3.3], "GetFloatArray works");
  assertArrayContentsEquals(module.utilities.GetManagedBoolArray([true, false, true]).value, [true, false, true], "GetBoolArray works");
  assertArrayContentsEquals(module.utilities.GetManagedByteArray(new Uint8Array([1, 2, 3])).value, [1, 2, 3], "GetByteArray works");
}
