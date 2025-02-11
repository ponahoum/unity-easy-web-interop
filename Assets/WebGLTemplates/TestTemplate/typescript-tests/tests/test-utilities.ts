/**
 * Check if a condition is true and log the result
 */
export const assert = (condition: boolean, testName: string): void => {
    if (!condition) {
      // Note: The Error constructor does not officially support style parameters,
      // so we include only the message here.
      throw new Error("%cThe following test failed: " + testName);
    } else {
      console.log("%cThe following test passed: " + testName, "color: green");
    }
  };
  
  /**
   * Check if two values are equal and log the result
   */
  export const assertEquals = <T>(actual: T, expected: T, testName: string): void => {
    if (expected !== actual) {
      throw new Error(
        "%cThe following test failed: " + testName + ". Expected: " + expected + " but got: " + actual
      );
    } else {
      console.log("%cThe following test passed: " + testName, "color: green");
    }
  };
  
  /**
   * Check if two objects are equal by comparing their values and log the result.
   * (The objects are compared via JSON serialization, not by reference.)
   */
  export const assertObjectEqualsByValues = (
    actual: unknown,
    expected: unknown,
    testName: string
  ): void => {
    const expectedSerialized = JSON.stringify(expected);
    const actualSerialized = JSON.stringify(actual);
    if (expectedSerialized !== actualSerialized) {
      throw new Error(
        "%cThe following test failed: " +
          testName +
          ". Expected: " +
          expectedSerialized +
          " but got: " +
          actualSerialized
      );
    }
    console.log("%cThe following test passed: " + testName, "color: green");
  };
  
  /**
   * Check if two arrays have the same contents in the same order.
   */
  export const assertArrayContentsEquals = <T>(
    array1: T[],
    array2: T[],
    testName: string
  ): void => {
    if (array1.length !== array2.length) {
      throw new Error(
        "%cThe following test failed: " + testName + ". Arrays have different lengths"
      );
    }
  
    for (let i = 0; i < array1.length; i++) {
      if (array1[i] !== array2[i]) {
        throw new Error(
          "%cThe following test failed: " +
            testName +
            ". Arrays have different values. Expected: " +
            array1 +
            " but got: " +
            array2
        );
      }
    }
  
    console.log("%cThe following test passed: " + testName, "color: green");
  };
  
  /**
   * Check if a function throws an exception with a specific message.
   */
  export const assertThrows = (
    fn: () => void,
    expectedThrowMessage: string,
    testName: string
  ): void => {
    let thrown = false;
    let thrownMessage = "";
    try {
      fn();
    } catch (error: unknown) {
      if (error instanceof Error) {
        thrownMessage = error.message;
      } else {
        thrownMessage = String(error);
      }
      if (thrownMessage === expectedThrowMessage) {
        thrown = true;
      }
    }
  
    if (!thrown) {
      throw new Error(
        "%cThe following test failed: " +
          testName +
          ". Expected to throw: " +
          expectedThrowMessage +
          " but got: " +
          thrownMessage
      );
    }
  
    console.log("%cThe following test passed: " + testName, "color: green");
  };
  
  /**
   * Check if an async function throws an exception with a specific message.
   */
  export const assertThrowsAsync = async (
    fn: () => Promise<void>,
    expectedThrowMessage: string,
    testName: string
  ): Promise<void> => {
    let thrown = false;
    let thrownMessage = "";
    try {
      await fn();
    } catch (error: unknown) {
      if (error instanceof Error) {
        thrownMessage = error.message;
      } else {
        thrownMessage = String(error);
      }
      if (thrownMessage === expectedThrowMessage) {
        thrown = true;
      }
    }
  
    if (!thrown) {
      throw new Error(
        "%cThe following test failed: " +
          testName +
          ". Expected to throw: " +
          expectedThrowMessage +
          " but got: " +
          thrownMessage
      );
    }
  
    console.log("%cThe following test passed: " + testName, "color: green");
  };
  
  // --- Types for Serialized Values (e.g., from Unity) ---
  
  export interface Vector2 {
    x: number;
    y: number;
  }
  
  export interface Vector3 {
    x: number;
    y: number;
    z: number;
  }
  
  export interface Vector4 {
    x: number;
    y: number;
    z: number;
    w: number;
  }
  
  export type Quaternion = Vector4;
  
  export interface Color {
    r: number;
    g: number;
    b: number;
    a: number;
  }
  
  export type Color32 = Color;
  
  // --- Sample Values ---
  
  export const SampleValues = {
    TestString: "test string",
    TestDouble: 2147483647125,
    TestInt: 450050554,
    TestFloat: 1234567.0,
    TestFloatArray: [123.0, 789.0, 101112.0],
    TestDoubleArray: [123.0, 789.0, 101112.0],
    TestBoolTrue: true,
    TestBoolFalse: false,
    TestByte: 255,
    TestByteArray: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
    TestExceptionValue: "This is a test exception",
  
    // Serialized values similar to those from Unity
    TestVector2: { x: 1, y: 2 } as Vector2,
    TestVector3: { x: 1, y: 2, z: 3 } as Vector3,
    TestVector4: { x: 1, y: 2, z: 3, w: 4 } as Vector4,
    TestQuaternion: { x: 1, y: 2, z: 3, w: 4 } as Quaternion,
    TestColor: { r: 1, g: 2, b: 3, a: 4 } as Color,
    TestColor32: { r: 1, g: 2, b: 3, a: 4 } as Color32,
  };
  
  export const DefaultExceptionMessage: string =
    "An exception occured on the C# side.";
  