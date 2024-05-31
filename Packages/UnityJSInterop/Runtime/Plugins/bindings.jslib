var easyWebInteropLib = {
    $dependencies: {},

    Setup: function (getIntPtrValueMethodPtr) {
        /**
         * Setup callback registerer, This allows to create javascript callbacks that can be called from C#
         * @param callback The javascript callback (arg)=>{...}
         */
        Module.internalJS.createCallback = (callback, signature) => {
            var ptr = addFunction(callback, signature);
            return ptr;
        };

        /**
         * Given a managed C# object pointer, return a serialized JSON object
         */
        Module.internalJS.getJsonValueFromGCHandlePtr = (ptrToGcHandle) => {
            const resPtr = dynCall("ii", getIntPtrValueMethodPtr, [ptrToGcHandle]);
            const asJsonObject = JSON.parse(UTF8ToString(resPtr)).value;
            // Free the memory allocated by the C# side
            _free(resPtr);
            return asJsonObject;
        };

        /**
         * Setup callback registerer, This allows to create javascript callbacks that can be called from C#
         */
        Module.internalJS.managedObjectsFinalizationRegistry = new FinalizationRegistry((ptrToGcHandle) => {
            Module.internal.CollectManagedPtr(ptrToGcHandle);
            console.log("Collected object with ptr: " + ptrToGcHandle);
        });

        Module.internalJS.delegateFinalizationRegistry = new FinalizationRegistry((delegateID) => {
            Module.internal.FreeDelegate(delegateID);
            console.log("Collected delegate with key: " + delegateID);
        });

        /**
         * Convert a string array pointer to a JS array
         * Doesn't free the input pointer, this has to be done manually somewhere else
         */
        Module.internalJS.stringArrayPtrToJSArray = (stringArrayPtr, stringArrayLength) => {
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
            // TO DO USE SAME FUNCTION AS IN RegisterMethodInRegistry
            for (var i = 0; i < targetArgs.length; i++) {
                if (targetArgs[i] instanceof Module.internalJS.PointerToNativeObject)
                    targetArgs[i] = targetArgs[i].targetGcHandleObjectPtr;
            }
            return Module.internalJS.HandleResPtr(dynCall(signatureAsString, functionPtr, targetArgs), false);
        }
    },
    RegisterMethodInRegistry: function (targetId, functionPtr, functionKeyPtr, pathToFunctionArrPtr, pathToFunctionArrLength, functionParamSignaturePtr, isAsyncTaskPtr) {
        const signatureAsString = UTF8ToString(functionParamSignaturePtr);

        // Given that pathToFunctionArrPtr is a string[] on the C# side, we need to convert it to a JS array
        const pathToFunctionArr = Module.internalJS.stringArrayPtrToJSArray(pathToFunctionArrPtr, pathToFunctionArrLength);
        const isAsyncTask = isAsyncTaskPtr === 1;

        // Create the function that will be injected in the module (if static) / target object (if instance)
        const moduleInjectedFunction = (...args) => {
            // Assign params of the fuction to a variable that's we'll play with afterwards
            const targetArgs = Module.internalJS.autoTransformArgs([...args]);

            // Add function name pointer string as the first element
            targetArgs.unshift(functionKeyPtr);

            // Call the function. Handle any uncaught errors (typically exceptions not thrown by the user, which is an option in the Unity build);
            let resultOfCall = undefined;
            try {
                resultOfCall = dynCall(signatureAsString, functionPtr, targetArgs);
            }
            catch (e) {
                console.error("An uncaught error occurred when calling the C# method: " + functionKeyPtr + ". This could break the player. If you wish to catch this error, consider wrapping the call in a try-catch block in C# and/or enable exceptions in build publishing settings.", e);
            }

            // Measure time it took to perform the dyncall
            const resultingManagedObjectPtr = Module.internalJS.HandleResPtr(resultOfCall, true);

            // Handle async task if needed
            if (isAsyncTask) {
                return new Promise((resolve, reject) => {
                    const callBackPtr = Module.internalJS.createCallback((i) => {
                        // When this piece of code is called, it means the task is completed
                        try {
                            const res = Module.internalJS.HandleResPtr(i, true);
                            resolve(res);
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
        // Static method case
        if (targetId === -1)
            Module.internalJS.assignValueToPath(Module, pathToFunctionArr, moduleInjectedFunction);

        // Instance case
        else {
            const targetObject = Module.internalJS.tempReferences;
            Module.internalJS.assignValueToPath(targetObject, pathToFunctionArr, moduleInjectedFunction);
        }

        // Add the method to finalization registry so that if it's dereference in JS, it's also dereferenced in C#
        Module.internalJS.delegateFinalizationRegistry.register(moduleInjectedFunction, functionKeyPtr);
    },
};

autoAddDeps(easyWebInteropLib, '$dependencies');
mergeInto(LibraryManager.library, easyWebInteropLib);
