var easyWebInteropLib = {
    $dependencies: {},

    Setup: function (getIntPtrValueMethodPtr, collectManagedPtrMethodPtr) {
        /**
         * Setup callback registerer, This allows to create javascript callbacks that can be called from C#
         * @param callback The javascript callback (arg)=>{...}
         */
        Module.internal.createCallback = (callback, signature) => {
            var ptr = addFunction(callback, signature);
            return ptr;
        };

        /**
         * Given a managed C# object pointer, return a serialized JSON object
         */
        Module.internal.getJsonValueFromGCHandlePtr = (ptrToGcHandle) => {
            const resPtr = dynCall("ii", getIntPtrValueMethodPtr, [ptrToGcHandle]);
            const asJsonObject = JSON.parse(UTF8ToString(resPtr)).value;
            // Free the memory allocated by the C# side
            _free(resPtr);
            return asJsonObject;
        };

        /**
         * Setup callback registerer, This allows to create javascript callbacks that can be called from C#
         */
        Module.internal.finalizationRegistry = new FinalizationRegistry((ptrToGcHandle) => {
            dynCall("vi", collectManagedPtrMethodPtr, [ptrToGcHandle]);
            console.log("Collected object with ptr: " + ptrToGcHandle);
        });

        /**
         * Convert a string array pointer to a JS array
         * Doesn't free the input pointer, this has to be done manually somewhere else
         */
        Module.internal.stringArrayPtrToJSArray = (stringArrayPtr, stringArrayLength) => {
            var stringArray = new Array(stringArrayLength);
            for (var i = 0; i < stringArrayLength; i++) {
                var currentPtr = getValue(stringArrayPtr + i * 4, "i32");
                stringArray[i] = UTF8ToString(currentPtr);
            }
            return stringArray;
        }
    },
    /**
     * Register a static method in the internal registry
     * Only for internal use, not exposed to the user
     * This is used to register static methods that are called from the C# side without handling marshalling automatically
     */
    RegisterStaticMethodInternalRegistry: function (functionPtr, functionNamePtr, functionParamSignaturePtr) {
        const functionNameAsString = UTF8ToString(functionNamePtr);
        const signatureAsString = UTF8ToString(functionParamSignaturePtr);

        Module.internal[functionNameAsString] = (...args) => {
            const targetArgs = [...args];
            // For simplificy, if element is Module.internalJS.PointerToNativeObject, just replace the element with the targetGcHandleObjectPtr directly
            // This way we can pass either the PointerToNativeObject or the targetGcHandleObjectPtr directly
            // Only for internal use, not exposed to the user
            for (var i = 0; i < targetArgs.length; i++) {
                if (targetArgs[i] instanceof Module.internalJS.PointerToNativeObject)
                    targetArgs[i] = targetArgs[i].targetGcHandleObjectPtr;
            }
            return Module.internalJS.HandleResPtr(dynCall(signatureAsString, functionPtr, targetArgs));
        }
    },
    RegisterMethodInRegistry: function (functionPtr, functionKeyPtr, pathToFunctionArrPtr, pathToFunctionArrLength, functionParamSignaturePtr, isAsyncTaskPtr) {
        const functionNameAsString = UTF8ToString(functionKeyPtr);
        const signatureAsString = UTF8ToString(functionParamSignaturePtr);

        // Given that pathToFunctionArrPtr is a string[] on the C# side, we need to convert it to a JS array
        const pathToFunctionArr = Module.internal.stringArrayPtrToJSArray(pathToFunctionArrPtr, pathToFunctionArrLength);

        // Detect if the method is an async task on the C# side. Therefore we'll wrap it in a promise
        const isAsyncTask = isAsyncTaskPtr === 1;
        const moduleInjectedFunction = (...args) => {
            // Assign params of the fuction to a variable that's we'll play with afterwards
            const targetArgs = [...args];

            // Ensure all args are PointerToNativeObject, if not, throw an error
            for (var i = 0; i < targetArgs.length; i++) {
                if (targetArgs[i] instanceof Module.internalJS.PointerToNativeObject)
                    targetArgs[i] = targetArgs[i].targetGcHandleObjectPtr;
                else
                    throw new Error("All arguments must be instances of PointerToNativeObject. Argument at index " + i + " is not.");
            }

            // Add function name pointer string as the first element
            const methodSignaturePtr = allocateUTF8(functionNameAsString);
            targetArgs.unshift(methodSignaturePtr);

            // Call the function. Handle any uncaught errors (typically exceptions not thrown by the user, which is an option in the Unity build);
            let resultOfCall;
            try {
                resultOfCall = dynCall(signatureAsString, functionPtr, targetArgs)
            }
            catch {
                resultOfCall = undefined;
                console.error("An uncaught error occurred when calling the C# method: " + functionNameAsString + ". This could break the player. If you wish to catch this error, consider wrapping the call in a try-catch block in C# and/or enable exceptions in build publishing settings.");
            }
            const resultingManagedObjectPtr = Module.internalJS.HandleResPtr(resultOfCall);

            // Free the memory allocated by the allocateUTF8
            _free(methodSignaturePtr);

            // Handle async task if needed
            if (isAsyncTask) {
                return new Promise((resolve, reject) => {
                    const callBackPtr = Module.internal.createCallback((i) => {
                        // When this piece of code is called, it means the task is completed
                        try {
                            resolve(Module.internalJS.HandleResPtr(i));
                        }
                        catch (e) {
                            reject(e);
                        }
                    }, "vi");
                    Module.internal.WaitForTaskToComplete(resultingManagedObjectPtr, callBackPtr);
                });
            }

            // If the function is not an async task, return the resulting managed object immediately
            else {
                return resultingManagedObjectPtr;
            }
        };

        // Assign the function to the path in the module
        Module.internal.assignValueToPath(Module, pathToFunctionArr, moduleInjectedFunction);
    },
};

autoAddDeps(easyWebInteropLib, '$dependencies');
mergeInto(LibraryManager.library, easyWebInteropLib);
