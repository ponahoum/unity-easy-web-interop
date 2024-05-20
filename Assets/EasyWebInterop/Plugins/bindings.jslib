var myLib = {
    $dependencies: {},

    Setup: function (getIntPtrValueMethodPtr) {
        window.Module = Module;

        // Expose possibility to allocate a pointer from a string
        Module.allocateString = (str) => allocateUTF8(str);
        Module.convertStringResult = (strPtr) => UTF8ToString(strPtr);

        // Setup callback registerer
        Module.createCallback = (callback, signature) => {
            var ptr = addFunction(callback, signature);
            return ptr;
        };

        // Setup the function that allows to serialize any GCHandle registerer value
        Module.getJsonValueFromGCHandlePtr = (ptrToGcHandle) => {
            const resPtr = dynCall("ii", getIntPtrValueMethodPtr, [ptrToGcHandle]);
            console.log("Json is "+UTF8ToString(resPtr));
            const asJsonObject = JSON.parse(UTF8ToString(resPtr));
            return asJsonObject;
        };

        Module.allocateMemoryForArray = (data) => {
            var nDataBytes = data.length * data.BYTES_PER_ELEMENT;
            var dataPtr = _malloc(nDataBytes);

            HEAPF64.set(data, dataPtr / data.BYTES_PER_ELEMENT);
            console.log("Size is "+nDataBytes + " and size per element"+data.BYTES_PER_ELEMENT);
          
            // Set the values of the double array in the Emscripten heap
            //var dataHeap = new Uint8Array(HEAPU8.buffer, dataPtr, nDataBytes);
            //dataHeap.set(new Uint8Array(data.buffer));
            return dataPtr;
        };

        // Declare class that holds references
        Module.PointerToNativeObject = class PointerToNativeObject {
            // Constructor to initialize the private integer field
            constructor(initialValue) {
                this.targetGcHandleObjectPtr = initialValue;
            }

            get value() {
                return Module.getJsonValueFromGCHandlePtr(this.targetGcHandleObjectPtr);
            }
        }
    },
    RegisterStaticMethodInternalRegistry: function (functionPtr, functionNamePtr, functionParamSignaturePtr) {
        var functionNameAsString = UTF8ToString(functionNamePtr);
        var signatureAsString = UTF8ToString(functionParamSignaturePtr);

        // Check if Module["internal"] exists, if not, create it
        if (!Module["internal"]) {
            Module["internal"] = {};
        }

        Module["internal"][functionNameAsString] = (...args) => {
            // Assign params of the fuction to a variable that's we'll play with afterwards
            var targetArgs = [...args];
            const resPtr = dynCall(signatureAsString, functionPtr, targetArgs);

            // If rest is not undefined, return a pointer to the object
            if (resPtr === undefined)
                return undefined;
            return new Module.PointerToNativeObject(resPtr);
        }
    },
    RegisterMethodInRegistry: function (functionPtr, functionNamePtr, functionParamSignaturePtr) {
        var functionNameAsString = UTF8ToString(functionNamePtr);
        var signatureAsString = UTF8ToString(functionParamSignaturePtr);

        Module[functionNameAsString] = (...args) => {
            // Assign params of the fuction to a variable that's we'll play with afterwards
            var targetArgs = [...args];

            // Ensure all args are PointerToNativeObject, if not, throw an error
            for (var i = 0; i < targetArgs.length; i++) {
                if (targetArgs[i] instanceof Module.PointerToNativeObject) {
                    targetArgs[i] = targetArgs[i].targetGcHandleObjectPtr;
                }
                else{
                    throw new Error("All arguments must be instances of PointerToNativeObject. Argument at index " + i + " is not.");
                }
            }

            // Add function name pointer string as the first element
            targetArgs.unshift(allocateUTF8(functionNameAsString));

            console.log("Sent target array to" + functionNameAsString, targetArgs);

            // Call the function
            const resPtr = dynCall(signatureAsString, functionPtr, targetArgs);
            if (resPtr === undefined)
                return undefined;
            return new Module.PointerToNativeObject(resPtr);
        };
    },
};

autoAddDeps(myLib, '$dependencies');
mergeInto(LibraryManager.library, myLib);
