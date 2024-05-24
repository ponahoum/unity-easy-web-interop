
// Check if Module["internal"] exists, if not, create it
if (!Module["internal"])
    Module["internal"] = {};

// Check if Module["internalJs"] exists, if not, create it
if (!Module["internalJs"])
    Module["internalJs"] = {};

// Declare class that holds references
Module.PointerToNativeObject = class PointerToNativeObject {
    // Constructor to initialize the private integer field
    constructor(targetGcHandleObjectPtr, managedType) {
        this.targetGcHandleObjectPtr = targetGcHandleObjectPtr;
        this.managedType = managedType;
    }

    get value() {
        return Module.getJsonValueFromGCHandlePtr(this.targetGcHandleObjectPtr);
    }
}

// Declare a wrapper around a C# object pointer that will be collected by the garbage collector
Module.internalJs.HandleResPtr = function (resPtr, resManagedType) {
    if (resPtr === undefined)
        return undefined;

    const registry = new FinalizationRegistry((heldValue) => {
        // TO DO: remove the object from the C# side
        console.log("Object collected");
        console.log(heldValue);
    });
    console.log("Registering object - TO DO CHECK COLLECTIOn");

    let resultingPtr = new Module.PointerToNativeObject(resPtr, resManagedType);
    registry.register(resultingPtr, 'here put id of collected ??');
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
    HEAPF64.set(data, dataPtr / data.BYTES_PER_ELEMENT);
    return dataPtr;
};

Module.GetManagedDoubleArray = (array) => {
    // Check if the array contains only numbers
    for (var i = 0; i < array.length; i++) {
        if (typeof array[i] !== 'number')
            throw new Error("All elements of the array must be numbers.");
    }

    // Create float 64 array from array of number
    var float64Array = new Float64Array(array);
    
    // Allocate memory for the array
    var dataPtr = Module.internalJs.allocateMemoryForArray(float64Array);

    // Call internal function that will return a pointer to the array
    var ptrToManagedData = Module.internal.GetManagedDoubleArrayFromPtr(dataPtr, float64Array.length);

    // Free ptr
    _free(dataPtr);

    // Return a PointerToNativeObject that will be collected by the garbage collector
    return ptrToManagedData;
}