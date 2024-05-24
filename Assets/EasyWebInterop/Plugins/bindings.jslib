var easyWebInteropLib = {
    $dependencies: {},

    Setup: function (getIntPtrValueMethodPtr) {
        // Expose possibility to allocate a pointer from a string
        Module.allocateString = (str) => allocateUTF8(str);
        Module.convertStringResult = (strPtr) => UTF8ToString(strPtr);

        // Setup callback registerer, This allows to create javascript callbacks that can be called from C#
        Module.createCallback = (callback, signature) => {
            var ptr = addFunction(callback, signature);
            return ptr;
        };

        // Given a managed C# object pointer, return a serialized JSON object
        Module.getJsonValueFromGCHandlePtr = (ptrToGcHandle) => {
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
    RegisterMethodInRegistry: function (functionPtr, functionNamePtr, functionParamSignaturePtr, parametersNamesPtr, parametersNamesPtrSize) {
        var functionNameAsString = UTF8ToString(functionNamePtr);
        var signatureAsString = UTF8ToString(functionParamSignaturePtr);

        // Get each parameter in parametersNamesPtr string array
        var parametersNames = [];
        for (var i = 0; i < parametersNamesPtrSize; i++) {
            var ptr = parametersNamesPtr + i * 4;
            var str = UTF8ToString(Module.HEAPU32[ptr >> 2]);
            parametersNames.push(str);
        }
        console.log("ParametersNames: " + parametersNames);
        
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
            return Module.internalJs.HandleResPtr(dynCall(signatureAsString, functionPtr, targetArgs));
        };
    },
};

autoAddDeps(easyWebInteropLib, '$dependencies');
mergeInto(LibraryManager.library, easyWebInteropLib);
