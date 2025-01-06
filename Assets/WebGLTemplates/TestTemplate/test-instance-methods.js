import { assertEquals, assertArrayContentsEquals, assertObjectEqualsByValues  } from "./testing-utilities.js";
import { SampleValues } from "./testing-utilities.js";

export function RunTests(module) {

    // Get instance
    const instance = module.static["Nahoum.UnityJSInterop.Tests"].TestInstanceMethods.GetNewInstance();
    
    // Test methods of the instance
    // Regular values
    assertEquals(instance.TestGetString().value, SampleValues.TestString, "TestGetString to get a string from an instance works");
    assertEquals(instance.TestGetDouble().value, SampleValues.TestDouble, "TestGetDouble to get a double from an instance works");
    assertEquals(instance.TestGetInt().value, SampleValues.TestInt, "TestGetInt to get an int from an instance works");
    assertEquals(instance.TestGetFloat().value, SampleValues.TestFloat, "TestGetFloat to get a float from an instance works");
    assertArrayContentsEquals(instance.TestGetFloatArray().value, SampleValues.TestFloatArray, "TestGetFloatArray to get a float array from an instance works");
    assertArrayContentsEquals(instance.TestGetDoubleArray().value, SampleValues.TestDoubleArray, "TestGetDoubleArray to get a double array from an instance works");
    assertEquals(instance.TestGetBoolTrue().value, SampleValues.TestBoolTrue, "TestGetBoolTrue to get a true boolean from an instance works");
    assertEquals(instance.TestGetBoolFalse().value, SampleValues.TestBoolFalse, "TestGetBoolFalse to get a false boolean from an instance works");
    assertEquals(instance.TestGetNullString().value, null, "TestGetNullString to get a null string from an instance works");

    // Operations on the instance
    // Addition int
    var testInt1 = module.utilities.GetManagedInt(1);
    var testInt2 = module.utilities.GetManagedInt(2);
    assertEquals(instance.TestAdditionInt(testInt1, testInt2).value, 3, "TestAdditionInt to add two integers works");

    // Addition float
    var testFloat1 = module.utilities.GetManagedFloat(1.1);
    var testFloat2 = module.utilities.GetManagedFloat(2.2);
    assertEquals(instance.TestAdditionFloat(testFloat1, testFloat2).value, 3.3, "TestAdditionFloat to add two floats works");

    // Addition double
    var testDouble1 = module.utilities.GetManagedDouble(1.1);
    var testDouble2 = module.utilities.GetManagedDouble(2.2);
    assertEquals(instance.TestAdditionDouble(testDouble1, testDouble2).value, 3.3, "TestAdditionDouble to add two doubles works");

    // Concatenation of string
    var part1 = module.utilities.GetManagedString("Hello, ")
    var part2 = module.utilities.GetManagedString("World!")
    assertEquals(instance.TestAdditionString(part1, part2).value, "Hello, World!", "TestAdditionString to concatenate two strings works");

    // Simple unity types
    console.log(instance.TestGetVector2().value);
    assertObjectEqualsByValues(instance.TestGetVector2().value, SampleValues.TestVector2, "TestGetVector2 to get a Vector2 from an instance works");
    assertObjectEqualsByValues(instance.TestGetVector3().value, SampleValues.TestVector3, "TestGetVector3 to get a Vector3 from an instance works");
    assertObjectEqualsByValues(instance.TestGetVector4().value, SampleValues.TestVector4, "TestGetVector4 to get a Vector4 from an instance works");
    assertObjectEqualsByValues(instance.TestGetQuaternion().value, SampleValues.TestQuaternion, "TestGetQuaternion to get a Quaternion from an instance works");
    assertObjectEqualsByValues(instance.TestGetColor().value, SampleValues.TestColor, "TestGetColor to get a Color from an instance works");
    assertObjectEqualsByValues(instance.TestGetColor32().value, SampleValues.TestColor32, "TestGetColor32 to get a Color32 from an instance works");
}