var easyWebInteropLib = {
    $dependencies: {},

    Setup: function (getIntPtrValueMethodPtr, collectManagedPtrMethodPtr) {
        // Setup callback registerer, This allows to create javascript callbacks that can be called from C#
        Module.internal.createCallback = (callback, signature) => {
            var ptr = addFunction(callback, signature);
            return ptr;
        };

        // Given a managed C# object pointer, return a serialized JSON object
        Module.internal.getJsonValueFromGCHandlePtr = (ptrToGcHandle) => {
            const resPtr = dynCall("ii", getIntPtrValueMethodPtr, [ptrToGcHandle]);
            const asJsonObject = JSON.parse(UTF8ToString(resPtr)).value;
            // Free the memory allocated by the C# side
            _free(resPtr);
            return asJsonObject;
        };

        Module.internal.finalizationRegistry = new FinalizationRegistry((ptrToGcHandle) => {
            dynCall("vi", collectManagedPtrMethodPtr, [ptrToGcHandle]);
            console.log("Collected object with ptr: " + ptrToGcHandle);
        });
    },
    RegisterStaticMethodInternalRegistry: function (functionPtr, functionNamePtr, functionParamSignaturePtr) {
        var functionNameAsString = UTF8ToString(functionNamePtr);
        var signatureAsString = UTF8ToString(functionParamSignaturePtr);


        Module.internal[functionNameAsString] = (...args) => {
            var targetArgs = [...args];
            // For simplificy, if element is Module.internal.PointerToNativeObject, just replace the element with the targetGcHandleObjectPtr directly
            // This way we can pass either the PointerToNativeObject or the targetGcHandleObjectPtr directly
            // Only for internal use, not exposed to the user
            for (var i = 0; i < targetArgs.length; i++) {
                if (targetArgs[i] instanceof Module.internal.PointerToNativeObject)
                    targetArgs[i] = targetArgs[i].targetGcHandleObjectPtr;
            }
            return Module.internalJs.HandleResPtr(dynCall(signatureAsString, functionPtr, targetArgs));
        }
    },
    RegisterMethodInRegistry: function (functionPtr, functionNamePtr, functionParamSignaturePtr, isAsyncTaskPtr) {
        var functionNameAsString = UTF8ToString(functionNamePtr);
        var signatureAsString = UTF8ToString(functionParamSignaturePtr);

        // Detect if the method is an async task on the C# side. Therefore we'll wrap it in a promise
        const isAsyncTask = isAsyncTaskPtr === 1;
        Module[functionNameAsString] = (...args) => {
            // Assign params of the fuction to a variable that's we'll play with afterwards
            var targetArgs = [...args];

            // Ensure all args are PointerToNativeObject, if not, throw an error
            for (var i = 0; i < targetArgs.length; i++) {
                if (targetArgs[i] instanceof Module.internal.PointerToNativeObject)
                    targetArgs[i] = targetArgs[i].targetGcHandleObjectPtr;
                else
                    throw new Error("All arguments must be instances of PointerToNativeObject. Argument at index " + i + " is not.");
            }

            // Add function name pointer string as the first element
            var functionNamePtr = allocateUTF8(functionNameAsString);
            targetArgs.unshift(functionNamePtr);

            // Call the function
            const resultingManagedObjectPtr = Module.internalJs.HandleResPtr(dynCall(signatureAsString, functionPtr, targetArgs));
            
            // Free the memory allocated by the allocateUTF8
            _free(functionNamePtr);

            // Handle async task if needed
            if (isAsyncTask) {
                return new Promise((resolve, reject) => {
                    const callBackPtr = Module.internal.createCallback((i) => {
                        console.log("Task completed with result: " + i);
                        try{
                            resolve(Module.internalJs.HandleResPtr(i));
                        }
                        catch(e){
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
    },
};

autoAddDeps(easyWebInteropLib, '$dependencies');
mergeInto(LibraryManager.library, easyWebInteropLib);
