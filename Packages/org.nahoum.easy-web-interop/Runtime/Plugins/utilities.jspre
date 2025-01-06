// Check if Module["internalJs"] exists, if not, create it
if (!Module.utilities) Module.utilities = {};

/**
 * Get a managed string on the C# side from a JavaScript string
 */
Module.utilities.GetManagedString = (jsStr) => {
  const ptr = stringToNewUTF8(jsStr);
  const managed = Module.internal.GetManagedString(ptr);
  _free(ptr);
  return managed;
};
Module.utilities.GetManagedBool = (targetBool) => {
  // A 1-byte signed integer. You can use this member to transform a Boolean value into a 1-byte, C-style bool (true = 1, false = 0).
  const asL1 = targetBool ? 1 : 0;
  return Module.internal.GetManagedBool(asL1);
};

Module.utilities.GetManagedFloat = (targetNumber) => {
  return Module.internal.GetManagedFloat(targetNumber);
};

Module.utilities.GetManagedInt = (targetNumber) => {
  return Module.internal.GetManagedInt(targetNumber);
};

Module.utilities.GetManagedDouble = (targetNumber) => {
  return Module.internal.GetManagedDouble(targetNumber);
};

Module.utilities.GetManagedLong = (targetNumber) => {
  return Module.internal.GetManagedLong(targetNumber);
};

Module.utilities.GetManagedDoubleArray = (array) => {
  if (!isNumberArray(array)) throw new Error("All elements of the array must be numbers.");
  const float64Array = new Float64Array(array);
  const dataPtr = Module.internalJS.allocateMemoryForArray(float64Array);
  const ptrToManagedData = Module.internal.GetManagedDoubleArray(dataPtr, float64Array.length);
  _free(dataPtr);
  return ptrToManagedData;
};

Module.utilities.GetManagedIntArray = (array) => {
  if (!isNumberArray(array)) throw new Error("All elements of the array must be numbers.");
  const int32Array = new Int32Array(array);
  const dataPtr = Module.internalJS.allocateMemoryForArray(int32Array);
  const ptrToManagedData = Module.internal.GetManagedIntArray(dataPtr, int32Array.length);
  _free(dataPtr);
  return ptrToManagedData;
};

Module.utilities.GetManagedFloatArray = (array) => {
  if (!isNumberArray(array)) throw new Error("All elements of the array must be numbers.");
  const float32Array = new Float32Array(array);
  const dataPtr = Module.internalJS.allocateMemoryForArray(float32Array);
  const ptrToManagedData = Module.internal.GetManagedFloatArray(dataPtr, float32Array.length);
  _free(dataPtr);
  return ptrToManagedData;
};

Module.utilities.GetManagedStringArray = (array) => {
  if (!isStringArray(array)) throw new Error("All elements of the array must be strings.");
  const stringPtrArray = array.map((str) => stringToNewUTF8(str));

  // Allocate memory for the array of pointers
  let ptrArray = _malloc(stringPtrArray.length * 4); // 4-bytes per pointer
  for (let i = 0; i < stringPtrArray.length; i++) {
    // Since HEAP32 is an array of 32-bit integers, each element of HEAP32 represents 4 bytes in memory.
    // Therefore, to convert a byte address to an index in HEAP32, you need to divide the byte address by 4.
    // This is efficiently done using a bitwise right shift by 2 (>> 2).
    HEAP32[(ptrArray + i * 4) >> 2] = stringPtrArray[i];
  }

  const result = Module.internal.GetManagedStringArray(ptrArray, stringPtrArray.length);
  _free(ptrArray);
  stringPtrArray.forEach((ptr) => _free(ptr));
  return result;
};

Module.utilities.GetManagedByteArray = (uint8array) => {
  // Ensure it' a Uint8Array
  if (!(uint8array instanceof Uint8Array)) throw new Error("The input must be a Uint8Array.");
  const dataPtr = Module.internalJS.allocateMemoryForArray(uint8array);
  const ptrToManagedData = Module.internal.GetManagedByteArray(dataPtr, uint8array.length);
  _free(dataPtr);
  return ptrToManagedData;
};

Module.utilities.GetManagedBoolArray = (array) => {
  // Ensure it's a boolean array
  if (!isBooleanArray(array)) throw new Error("The input must be a boolean array.");
  const int8Array = new Int8Array(array.map((b) => (b ? 1 : 0)));
  const dataPtr = Module.internalJS.allocateMemoryForArray(int8Array);
  const ptrToManagedData = Module.internal.GetManagedBoolArray(dataPtr, int8Array.length);
  _free(dataPtr);
  return ptrToManagedData;
};

Module.utilities.GetManagedAction = (callback, managedTypesArray) => {
  // If array is null, just replace it with empty array (optionnal arg)
  const inputTypeStringArray = managedTypesArray == null || managedTypesArray == undefined ? [] : managedTypesArray;

  // Check managed types array is a string array
  if (!isStringArray(inputTypeStringArray)) throw new Error("The second parameter must be an array of strings.");

  // Get callback signature depending on the length of the managed types array
  let callbackSignature = "v";
  for (let i = 0; i < inputTypeStringArray.length; i++) callbackSignature += "i";

  // Ensure the the callback is a function
  if (typeof callback !== "function") throw new Error("The first parameter must be a function.");

  // Make sure the function number of parameters matches the number of managed types
  if (callback.length !== inputTypeStringArray.length) throw new Error("The number of parameters of the callback must match the number of managed types.");

  // Now, create a callback that wraps this callback so that its returns GC collect ptrs
  const newCallback = (...args) => {
    // Create a PointerToNativeObject for each argument
    const newArgs = [];
    for (let i = 0; i < args.length; i++) {
      newArgs.push(new Module.internalJS.HandleResPtr(args[i]));
    }
    // Invoke the original callback with the wrapped arguments
    callback(...newArgs);
  };

  // Create the callback
  const jsPtrCallback = Module.internalJS.createCallback(newCallback, callbackSignature);

  // Convert the string array
  const ptrToManagedStringArray = Module.utilities.GetManagedStringArray(managedTypesArray);

  // Get the action pointer on the C# side
  const managedAction = Module.internal.GetManagedWrappedAction(ptrToManagedStringArray, jsPtrCallback);
  return managedAction;
};

isArrayOfType = (array, type) => {
  for (let i = 0; i < array.length; i++) {
    if (typeof array[i] !== type) return false;
  }
  return true;
};

isNumberArray = (array) => {
  return isArrayOfType(array, "number");
};

isStringArray = (array) => {
  return isArrayOfType(array, "string");
};

isBooleanArray = (array) => {
  return isArrayOfType(array, "boolean");
};
