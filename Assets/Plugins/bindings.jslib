var myLib = {
    $dependencies: {},

    RegisterMethodInRegistry: function(functionPtr, functionNamePtr, functionParamSignaturePtr) {

        window.Module = Module;
        Module.allocateString = (str)=>allocateUTF8(str);
        Module.convertStringResult = (strPtr)=>UTF8ToString(strPtr);
        
        // Convert function name to string
        var functionNameAsString = UTF8ToString(functionNamePtr);

        // Convert signature to UTF8ToString
        var signatureAsString = UTF8ToString(functionParamSignaturePtr);

         Module.createCallback = (callback, signature)=>{
            var ptr=addFunction(callback,signature);
            return ptr;
        }

        Module[functionNameAsString] = (...diverseValuesArray) => {

            const targetArgs = diverseValuesArray;

            // Check if the diverseValuesArray is a single argument and not an array
            if (!Array.isArray(targetArgs)) {
                targetArgs = [targetArgs];
            }
            // Add functio name pointer string as the first element
            targetArgs.unshift(allocateUTF8(functionNameAsString));
        console.log("Sent target array was",targetArgs);
            console.log("intiialize call to "+functionNameAsString);
            var resPtr = dynCall(signatureAsString, functionPtr, targetArgs);
            return resPtr;
        };
    },
    JSCallbackExample: function(){

        return ptr;
    }
};

autoAddDeps(myLib, '$dependencies');
mergeInto(LibraryManager.library, myLib);
