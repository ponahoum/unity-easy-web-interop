// Check if Module["internal"] exists, if not, create it
if (!Module.internal) Module.internal = {};

// Check if Module["internalJs"] exists, if not, create it
if (!Module.internalJS) Module.internalJS = {};

/**
 * A dictionnary of int, pointer to native object to temporary store the objects
 */
Module.internalJS.tempReferences = {};

/**
 * Given an object and a string array, assign a value to the path specified by the string array
 * For example if the array is ["a", "b", "c"] and the value is 10, the object will be modified to be {a: {b: {c: 10}}}
 */
Module.internalJS.assignValueToPath = (targetObject, pathArray, value) => {
  let currentLevel = targetObject;
  for (let i = 0; i < pathArray.length; i++) {
    const key = pathArray[i];

    // If it's the last key in the path, assign the value
    if (i === pathArray.length - 1) {
      currentLevel[key] = value;
    } else {
      // If the key does not exist or is not an object, create an empty object
      if (!currentLevel[key] || typeof currentLevel[key] !== "object") {
        currentLevel[key] = {};
      }
      // Move to the next level
      currentLevel = currentLevel[key];
    }
  }
};

// Declare class that holds references
Module.internalJS.PointerToNativeObject = class PointerToNativeObject {
  // Constructor to initialize the private integer field
  constructor(targetGcHandleObjectPtr) {
    this.targetGcHandleObjectPtr = targetGcHandleObjectPtr;
  }

  get value() {
    return Module.internalJS.getJsonValueFromGCHandlePtr(this.targetGcHandleObjectPtr);
  }

  get managedType() {
    return Module.internal.GetManagedTypeFromManagedPtr(this.targetGcHandleObjectPtr);
  }
};

// Declare a wrapper around a C# object pointer that will be collected by the garbage collector
Module.internalJS.HandleResPtr = function (resPtr, injectDelegateInObject = true) {
  // Check for undefined (-2 is void, which is undefined)
  if (resPtr === undefined || resPtr === -2) return undefined;

  // Check for exceptions (-3 is the encoding for an exception. See C# code for more details)
  if (resPtr == -3) throw new Error("An exception occured on the C# side.");

  // Wrap the pointer in a PointerToNativeObject for ease of use
  const resultingPtr = new Module.internalJS.PointerToNativeObject(resPtr);

  // Register the object in the finalization registry so that when it's deferenced, it's collected
  Module.internalJS.managedObjectsFinalizationRegistry.register(resultingPtr, resPtr);

  // If this object is an instance, we inject the methods in the object
  if (injectDelegateInObject) {
    // Timer to measure the time it takes
    var timeStart = performance.now();

    // Set the object to the temporary reference so that the method coming back from the C# side can be called
    // This way, methods will be populated in the object tempReferences
    // Once this is done, the object will be removed from tempReferences
    Module.internalJS.tempReferences = resultingPtr;
    Module.internal.RequestCompleteReturnedObject(resultingPtr);
    Module.internalJS.tempReferences = undefined;

    // Remove from tempReferences
    var timeEnd = performance.now();
    console.log("Tool took " + (timeEnd - timeStart) + " milliseconds to populate pointer to native object of id " + resultingPtr.targetGcHandleObjectPtr);
  }

  return resultingPtr;
};

// Allocate memory for an array of type Float64Array, Float32Array, Int32Array, etc.
// After allocating memory, one must make sure to free it using Module._free
Module.internalJS.allocateMemoryForArray = (data) => {
  // Ensure data is an array like Float64Array, Float32Array, Int32Array, etc.
  if (!data.BYTES_PER_ELEMENT) {
    throw new Error("Data must be an array like Float64Array, Float32Array, Int32Array, etc.");
  }

  var nDataBytes = data.length * data.BYTES_PER_ELEMENT;
  var dataPtr = _malloc(nDataBytes);

  // Depending on the kind of array, allocate the memory on different emscripten views
  if (data instanceof Float64Array) {
    HEAPF64.set(data, dataPtr / data.BYTES_PER_ELEMENT);
  } else if (data instanceof Float32Array) {
    HEAPF32.set(data, dataPtr / data.BYTES_PER_ELEMENT);
  } else if (data instanceof Int8Array) {
    HEAP8.set(data, dataPtr / data.BYTES_PER_ELEMENT);
  } else if (data instanceof Int32Array) {
    HEAP32.set(data, dataPtr / data.BYTES_PER_ELEMENT);
  } else if (data instanceof Uint8Array) {
    HEAPU8.set(data, dataPtr / data.BYTES_PER_ELEMENT);
  } else throw "Unsupported array type. Supported types are Float64Array, Float32Array, Int32Array, BigInt64Array, Uint8Array.";
  return dataPtr;
};

/**
 * Givens args, transform them to the expected types if they are of native types
 */
Module.internalJS.autoTransformArgs = (argsToTransformArray) => {
  const newArgs = [];
  for (let i = 0; i < argsToTransformArray.length; i++) {
    const arg = argsToTransformArray[i];

    // Just transform the argument if it's a native type
    if (arg instanceof Module.internalJS.PointerToNativeObject) newArgs.push(arg.targetGcHandleObjectPtr);
    // TO DO LATER: Add if here to allow transformation of certain js types directly to c# types using internal static GetManagedX methods
    // Not obvious because for example, a number could be a double, int, float, etc.
    else throw new Error("Unsupported argument type.", arg);
  }

  return newArgs;
};
