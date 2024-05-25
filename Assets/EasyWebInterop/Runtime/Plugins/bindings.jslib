var easyWebInteropLib = {
    $dependencies: {},

    Setup: function (getIntPtrValueMethodPtr) {
        // Setup callback registerer, This allows to create javascript callbacks that can be called from C#
        Module.internal.createCallback = (callback, signature) => {
            var ptr = addFunction(callback, signature);
            return ptr;
        };

        // Given a managed C# object pointer, return a serialized JSON object
        Module.internal.getJsonValueFromGCHandlePtr = (ptrToGcHandle) => {
            const resPtr = dynCall("ii", getIntPtrValueMethodPtr, [ptrToGcHandle]);
            const asJsonObject = JSON.parse(UTF8ToString(resPtr)).value;
            return asJsonObject;
        };
    },
    RegisterStaticMethodInternalRegistry: function (functionPtr, functionNamePtr, functionParamSignaturePtr) {
        var functionNameAsString = UTF8ToString(functionNamePtr);
        var signatureAsString = UTF8ToString(functionParamSignaturePtr);


        Module.internal[functionNameAsString] = (...args) => {
            var targetArgs = [...args];
            // For simplificy, if element is Module.PointerToNativeObject, just replace the element with the targetGcHandleObjectPtr directly
            for (var i = 0; i < targetArgs.length; i++) {
                if (targetArgs[i] instanceof Module.PointerToNativeObject)
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
                if (targetArgs[i] instanceof Module.PointerToNativeObject)
                    targetArgs[i] = targetArgs[i].targetGcHandleObjectPtr;
                else
                    throw new Error("All arguments must be instances of PointerToNativeObject. Argument at index " + i + " is not.");
            }

            // Add function name pointer string as the first element
            targetArgs.unshift(allocateUTF8(functionNameAsString));

            // Call the function
            const resultingManagedObjectPtr = Module.internalJs.HandleResPtr(dynCall(signatureAsString, functionPtr, targetArgs));

            // Handle async task if needed
            if (isAsyncTask) {
                return new Promise((resolve, reject) => {
                    const callBackPtr = Module.internal.createCallback((i) => {
                        console.log("Task completed with result: " + i);
                        // Case exception on the C# side
                        if (i == -3) {
                            reject("Task failed on the C# side.");
                        }
                        // Case void task
                        else if(i == -2) {
                            resolve(undefined);
                        }
                        // Case task with a result (Task<T>)
                        else {
                            resolve(Module.internalJs.HandleResPtr(i));
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
