
// Check if Module["internal"] exists, if not, create it
if (!Module["internal"])
    Module["internal"] = {};

// Check if Module["internalJs"] exists, if not, create it
if (!Module["internalJs"])
    Module["internalJs"] = {};

// Declare class that holds references
Module.internal.PointerToNativeObject = class PointerToNativeObject {
    // Constructor to initialize the private integer field
    constructor(targetGcHandleObjectPtr) {
        this.targetGcHandleObjectPtr = targetGcHandleObjectPtr;
    }

    get value() {
        return Module.internal.getJsonValueFromGCHandlePtr(this.targetGcHandleObjectPtr);
    }

    get managedType() {
        return Module.internal.GetManagedTypeFromManagedPtr(this.targetGcHandleObjectPtr);
    }
}

// Declare a wrapper around a C# object pointer that will be collected by the garbage collector
Module.internalJs.HandleResPtr = function (resPtr) {
    // Check for undefined
    if (resPtr === undefined || resPtr === -2)
        return undefined;

    // Check for exceptions
    if (resPtr == -3) {
        throw new Error("An exception occured on the C# side.");
    }

    let resultingPtr = new Module.internal.PointerToNativeObject(resPtr);
    Module.internal.finalizationRegistry.register(resultingPtr, resPtr);
    return resultingPtr;
}

// Allocate memory for an array of type Float64Array, Float32Array, Int32Array, etc.
// After allocating memory, one must make sure to free it using Module._free
Module.internalJs.allocateMemoryForArray = (data) => {
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
    }
    else if (data instanceof Int8Array) {
        HEAP8.set(data, dataPtr / data.BYTES_PER_ELEMENT);
    }
    else if (data instanceof Int32Array) {
        HEAP32.set(data, dataPtr / data.BYTES_PER_ELEMENT);
    }
    else if (data instanceof Uint8Array) {
        HEAPU8.set(data, dataPtr / data.BYTES_PER_ELEMENT);
    }
    else
        throw "Unsupported array type. Supported types are Float64Array, Float32Array, Int32Array, BigInt64Array, Uint8Array."
    return dataPtr;
};

/**
 * Get a managed string on the C# side from a JavaScript string
 */
Module.GetManagedString = (jsStr) => {
    const ptr = stringToNewUTF8(jsStr)
    const managed = Module.internal.GetManagedString(ptr);
    _free(ptr);
    return managed;
}
Module.GetManagedBool = (targetBool) => {
    // A 1-byte signed integer. You can use this member to transform a Boolean value into a 1-byte, C-style bool (true = 1, false = 0).
    const asL1 = targetBool ? 1 : 0;
    return Module.internal.GetManagedBool(asL1);
}

Module.GetManagedFloat = (targetNumber) => {
    return Module.internal.GetManagedFloat(targetNumber);
}

Module.GetManagedInt = (targetNumber) => {
    return Module.internal.GetManagedInt(targetNumber);
}

Module.GetManagedDouble = (targetNumber) => {
    return Module.internal.GetManagedDouble(targetNumber);
}

Module.GetManagedLong = (targetNumber) => {
    return Module.internal.GetManagedLong(targetNumber);
}


Module.GetManagedDoubleArray = (array) => {
    if (!isNumberArray(array))
        throw new Error("All elements of the array must be numbers.");
    const float64Array = new Float64Array(array);
    const dataPtr = Module.internalJs.allocateMemoryForArray(float64Array);
    const ptrToManagedData = Module.internal.GetManagedDoubleArray(dataPtr, float64Array.length);
    _free(dataPtr);
    return ptrToManagedData;
}

Module.GetManagedIntArray = (array) => {
    if (!isNumberArray(array))
        throw new Error("All elements of the array must be numbers.");
    const int32Array = new Int32Array(array);
    const dataPtr = Module.internalJs.allocateMemoryForArray(int32Array);
    const ptrToManagedData = Module.internal.GetManagedIntArray(dataPtr, int32Array.length);
    _free(dataPtr);
    return ptrToManagedData;
}

Module.GetManagedFloatArray = (array) => {
    if (!isNumberArray(array))
        throw new Error("All elements of the array must be numbers.");
    const float32Array = new Float32Array(array);
    const dataPtr = Module.internalJs.allocateMemoryForArray(float32Array);
    const ptrToManagedData = Module.internal.GetManagedFloatArray(dataPtr, float32Array.length);
    _free(dataPtr);
    return ptrToManagedData;
}

Module.GetManagedStringArray = (array) => {
    if (!isStringArray(array))
        throw new Error("All elements of the array must be strings.");
    const stringPtrArray = array.map((str) => stringToNewUTF8(str));

    // Allocate memory for the array of pointers
    let ptrArray = _malloc(stringPtrArray.length * 4); // 4-bytes per pointer
    for (let i = 0; i < stringPtrArray.length; i++) {
        // Since HEAP32 is an array of 32-bit integers, each element of HEAP32 represents 4 bytes in memory. 
        // Therefore, to convert a byte address to an index in HEAP32, you need to divide the byte address by 4. 
        // This is efficiently done using a bitwise right shift by 2 (>> 2).
        HEAP32[((ptrArray + i * 4) >> 2)] = stringPtrArray[i];
    }

    const result = Module.internal.GetManagedStringArray(ptrArray, stringPtrArray.length);
    _free(ptrArray);
    stringPtrArray.forEach(ptr => _free(ptr));
    return result;
}

Module.GetManagedByteArray = (uint8array) => {
    // Ensure it' a Uint8Array
    if (!(uint8array instanceof Uint8Array))
        throw new Error("The input must be a Uint8Array.");
    const dataPtr = Module.internalJs.allocateMemoryForArray(uint8array);
    const ptrToManagedData = Module.internal.GetManagedByteArray(dataPtr, uint8array.length);
    _free(dataPtr);
    return ptrToManagedData;
}

Module.GetManagedBoolArray = (array) => {
    // Ensure it's a boolean array
    if (!isBooleanArray(array))
        throw new Error("The input must be a boolean array.");
    const int8Array = new Int8Array(array.map((b) => b ? 1 : 0));
    const dataPtr = Module.internalJs.allocateMemoryForArray(int8Array);
    const ptrToManagedData = Module.internal.GetManagedBoolArray(dataPtr, int8Array.length);
    _free(dataPtr);
    return ptrToManagedData;
}

Module.GetManagedAction = (callback, managedTypesArray) => {
    // Check managed types array is a string array
    if (!isStringArray(managedTypesArray))
        throw new Error("The second parameter must be an array of strings.");

    // Get callback signature depending on the length of the managed types array
    let callbackSignature = "v";
    for (let i = 0; i < managedTypesArray.length; i++)
        callbackSignature += "i";

    // Ensure the the callback is a function
    if (typeof callback !== "function")
        throw new Error("The first parameter must be a function.");

    // Make sure the function number of parameters matches the number of managed types
    if (callback.length !== managedTypesArray.length)
        throw new Error("The number of parameters of the callback must match the number of managed types.");

    // Now, create a callback that wraps this callback so that its returns GC collect ptrs
    const newCallback = (...args) => {
        // Create a PointerToNativeObject for each argument
        const newArgs = [];
        for (let i = 0; i < args.length; i++) {
            newArgs.push(new Module.internalJs.HandleResPtr(args[i]));
        }
        // Invoke the original callback with the wrapped arguments
        callback(...newArgs);
    }

    // Create the callback
    const jsPtrCallback = Module.internal.createCallback(newCallback, callbackSignature);

    // Convert the string array 
    const ptrToManagedStringArray = Module.GetManagedStringArray(managedTypesArray);

    // Get the action pointer on the C# side
    const managedAction = Module.internal.GetManagedWrappedAction(ptrToManagedStringArray, jsPtrCallback);
    return managedAction;
}

isArrayOfType = (array, type) => {
    for (let i = 0; i < array.length; i++) {
        if (typeof array[i] !== type)
            return false;
    }
    return true;
}

isNumberArray = (array) => {
    return isArrayOfType(array, "number");
}

isStringArray = (array) => {
    return isArrayOfType(array, "string");
}

isBooleanArray = (array) => {
    return isArrayOfType(array, "boolean");
}