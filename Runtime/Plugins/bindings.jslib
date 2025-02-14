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
      //console.log("Collected object with ptr: " + ptrToGcHandle);
    });

    Module.internalJS.delegateFinalizationRegistry = new FinalizationRegistry((delegateID) => {
      Module.internal.FreeDelegate(delegateID);
      //console.log("Collected delegate with key: " + delegateID);
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
    };
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
        if (targetArgs[i] instanceof Module.internalJS.PointerToNativeObject) targetArgs[i] = targetArgs[i].targetGcHandleObjectPtr;
      }
      return Module.internalJS.HandleResPtr(dynCall(signatureAsString, functionPtr, targetArgs), false);
    };
  },
  RegisterMethodInRegistry: function (
    targetId,
    functionPtr,
    functionKeyPtr,
    pathToFunctionArrPtr,
    pathToFunctionArrLength,
    functionParamSignaturePtr,
    returnsTask
  ) {
    const signatureAsString = UTF8ToString(functionParamSignaturePtr);

    // Given that pathToFunctionArrPtr is a string[] on the C# side, we need to convert it to a JS array
    const pathToFunctionArr = Module.internalJS.stringArrayPtrToJSArray(pathToFunctionArrPtr, pathToFunctionArrLength);
    const methodReturnsTask = returnsTask === 1;

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
      } catch (e) {
        // Manually marks this as an exception pointer as it's undefined otherwise
        resultOfCall = -3;

        console.error(
          "An uncaught error occurred when calling the C# method: " +
            functionKeyPtr +
            " with signature " +
            signatureAsString +
            ". This could break the player. If you wish to catch this error, consider wrapping the call in a try-catch block in C# and/or enable exceptions in build publishing settings.",
          e
        );
      }

      // Measure time it took to perform the dyncall
      const resultingManagedObjectPtr = Module.internalJS.HandleResPtr(resultOfCall, true);

      // Handle tasks if needed
      if (methodReturnsTask) {
        return new Promise((resolve, reject) => {
          const callBackPtr = Module.internalJS.createCallback((i) => {
            // When this piece of code is called, it means the task is completed
            try {
              const res = Module.internalJS.HandleResPtr(i, true);
              resolve(res);
            } catch (e) {
              reject(e);
            }
          }, "vi");
          Module.internal.WaitForTaskToComplete(resultingManagedObjectPtr, callBackPtr);
        });
      }

      // If the function is not an task, return the resulting managed object immediately
      else {
        return resultingManagedObjectPtr;
      }
    };

    // Assign the function to the path in the module
    // Static method case
    if (targetId === -1) Module.internalJS.assignValueToPath(Module, pathToFunctionArr, moduleInjectedFunction);
    // Instance case
    else {
      const targetObject = Module.internalJS.tempReferences;
      Module.internalJS.assignValueToPath(targetObject, pathToFunctionArr, moduleInjectedFunction);
    }

    // Add the method to finalization registry so that if it's dereference in JS, it's also dereferenced in C#
    Module.internalJS.delegateFinalizationRegistry.register(moduleInjectedFunction, functionKeyPtr);
  },
    /**
   * Register a constructor that allows to create a managed delegate on the C# side from the JS side
   * Typically, if in c# a method expose an Action<string> in of of the argments of the exposed methods, a constructor for this type will be made here
   * In the case of Action<string>, it will be accessible via Module.utilities.DelegateConstructors["System"]["Action<string>"]
   * And will be used like this: Module.utilities.DelegateConstructors["System"]["Action<string>"](function(arg){console.log(arg);}), where arg is a pointer to a string
   */
    RegisterDelegateConstructor: function(namespaceNamePtr, delegateNamePtr, stringArrayWithAllDelegateArgsTypesPtr, arrayManagedTypesLengthPtr) {
      // Get name
      const namespace = UTF8ToString(namespaceNamePtr);
      const name = UTF8ToString(delegateNamePtr);
  
      // Define namespace and based paths if they don't exist
      if (!Module.extras) Module.extras = {};
      if(!Module.extras[namespace]) Module.extras[namespace] = {};
      if(!Module.extras[namespace][name]) Module.extras[namespace][name] = {};
      
      // Get array of managed types
      const managedTypesArray = Module.internalJS.stringArrayPtrToJSArray(stringArrayWithAllDelegateArgsTypesPtr, arrayManagedTypesLengthPtr);
  
      // Create a function that will allow to create a function that will be called from the C# side
      Module.extras[namespace][name].createDelegate = (callback) => {
  
        // Check callback is a function
        if (typeof callback !== "function") throw new Error("The callback must be a function.");
  
        // Check callback has the same number of args as managed types
        if (callback.length !== managedTypesArray.length) throw new Error("The number of parameters of the callback must match the number of managed types. Found " + callback.length + " parameters for " + managedTypesArray.length + " managed types.");
  
        return Module.utilities.GetManagedAction(callback, managedTypesArray)
      };
    },
};

autoAddDeps(easyWebInteropLib, "$dependencies");
mergeInto(LibraryManager.library, easyWebInteropLib);
