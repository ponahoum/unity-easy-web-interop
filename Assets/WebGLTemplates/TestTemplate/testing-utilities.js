/**
 * Check if a condition is true and log the result
 */
export const assert = function (condition, testName) {
  if (!condition) throw Error("%cThe following test failed: " + testName, "color: red");
  else console.log("%cThe following test passed: " + testName, "color: green");
};

/**
 * Check if two values are equal and log the result
 */
export const assertEquals = function (actual, expected, testName) {
  if (expected !== actual) throw Error("%cThe following test failed: " + testName + ". Expected: " + expected + " but got: " + actual, "color: red");
  else console.log("%cThe following test passed: " + testName, "color: green");
};


/**
 * Check if two objects are equal by comparing their values and log the result
 * Don't compare by reference
 */
export const assertObjectEqualsByValues = function (actual, expected, testName) {
  const expectedSerialized = JSON.stringify(expected);
  const actualSerialized = JSON.stringify(actual);
  if (expectedSerialized !== actualSerialized) {
    throw Error("%cThe following test failed: " + testName + ". Expected: " + expectedSerialized + " but got: " + actualSerialized, "color: red");
  }
  console.log("%cThe following test passed: " + testName, "color: green");
}

/**
 * Check two array have the same values inside
 */
export const assertArrayContentsEquals = function (array1, array2, testName) {
  if (array1.length !== array2.length){
    throw Error("%cThe following test failed: " + testName + ". Arrays have different lengths", "color: red");
  }

  for (let i = 0; i < array1.length; i++) {
    if (array1[i] !== array2[i]) {
      throw Error("%cThe following test failed: " + testName + ". Arrays have different values. Expected: " + array1 + " but got: " + array2, "color: red");
    }
  }
  
  console.log("%cThe following test passed: " + testName, "color: green");
};

/**
 * Check if a function throws an exception with a specific message
 */
export const assertThrows = function (fn, expectedThrowMessage, testName) {
  let thrown = false;
  let thrownMessage = "";
  try {
    fn();
  } catch (error) {
    thrownMessage = error.message;
    if (thrownMessage === expectedThrowMessage) {
      thrown = true;
    }
  }

  if (!thrown) {
    throw Error("%cThe following test failed: " + testName + ". Expected to throw: " + expectedThrowMessage + " but got: " + thrownMessage, "color: red");
  }

  console.log("%cThe following test passed: " + testName, "color: green");
}


/**
 * Check if an async function throws an exception with a specific message
 */
export const assertThrowsAsync = async function (fn, expectedThrowMessage, testName) {
  let thrown = false;
  let thrownMessage = "";
  try {
    await fn();
  } catch (error) {
    thrownMessage = error.message;
    if (thrownMessage === expectedThrowMessage) {
      thrown = true;
    }
  }

  if (!thrown) {
    throw Error("%cThe following test failed: " + testName + ". Expected to throw: " + expectedThrowMessage + " but got: " + thrownMessage, "color: red");
  }

  console.log("%cThe following test passed: " + testName, "color: green");
}

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

  // Add a few serialized values from unity aka 
  TestVector2: { x: 1, y: 2 },
  TestVector3: { x: 1, y: 2, z: 3 },
  TestVector4: { x: 1, y: 2, z: 3, w: 4 },
  TestQuaternion: { x: 1, y: 2, z: 3, w: 4 },
  TestColor: { r: 1, g: 2, b: 3, a: 4 },
  TestColor32: { r: 1, g: 2, b: 3, a: 4 },
};

export const DefaultExceptionMessage = "An exception occured on the C# side.";