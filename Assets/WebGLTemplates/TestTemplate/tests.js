const assert = function (condition, testName) {
    if (!condition)
        throw Error('The following test failed: ' + testName);
    else
        console.log('The following test passed: ' + testName);
};

const assertEquals = function (actual, expected, testName) {
    if (expected !== actual)
        throw Error('The following test failed: ' + testName + '. Expected: ' + expected + ' but actual is: ' + actual);
    else
        console.log('The following test passed: ' + testName);
}

/**
 * Check two array have the same values inside
 */
const arrayContentsEquals = function (array1, array2) {
    if(array1.length !== array2.length)
        return false;

    for (let i = 0; i < array1.length; i++) {
        if (array1[i] !== array2[i]) {
            return false;
        }
    }
    return true;

}

/**
 *  Download sample image
 */

async function downloadJPGImageToUint8Array(resX, resY) {
    try {
        // Fetch the image
        const url= "https://picsum.photos/" + resX + '/' + resY;
        const response = await fetch(url);

        // Check if the response is ok
        if (!response.ok) {
            throw new Error(`Network response was not ok: ${response.statusText}`);
        }

        // Get the image as a blob
        const blob = await response.blob();

        // Convert the blob to an ArrayBuffer
        const arrayBuffer = await blob.arrayBuffer();

        // Create a Uint8Array from the ArrayBuffer
        const uint8Array = new Uint8Array(arrayBuffer);

        // Return the Uint8Array
        return uint8Array;
    } catch (error) {
        console.error('Error downloading or converting the image:', error);
    }
}


// Wait for window.Module to be defined
const waitForModule = async function () {
    while (!window.Module) {
        await new Promise(resolve => setTimeout(resolve, 100));
    }
    return window.Module;
};

const runTests = async function () {
    const module = await waitForModule();
    console.log('Module loaded');

    // Regular method and async methods
    assertEquals(module.MyMethodReturningString().value, "returning a random string", "Method returning string works");
    assertEquals(module.MyMethodReturningDouble().value, 2147483647125, "Method returning double works");
    assertEquals(module.MyMethodReturningInt().value, 450050554, "Method returning int works");
    assertEquals(module.TestReturnNullValue().value, null, "Method returning null works");
    assertEquals((await module.AsyncTaskReturnString()).value, "A cool result from an async task", "AsyncTaskReturnString (Async Task<string>) returns correct string value");
    assert(module.AsyncTaskReturnString().then, "AsyncTaskReturnString (Async Task<string>) is a promise");
    assertEquals(await module.AsyncTaskVoidMethod(), undefined, "Awaiting AsyncTaskVoidMethod returns undefined");
    assert(module.AsyncTaskVoidMethod().then, "AsyncTaskVoidMethod is a promise");
    assertEquals(module.AsyncVoidMethod(), undefined, "AsyncVoidMethod is not a promise and is undefined");
    assert(arrayContentsEquals(module.MethodReturningDoubleArray().value, [123, 789, 101112]), "MethodReturningDoubleArray");

    // Exceptions catching
    try {
        await module.AsyncTaskStringFail();
        assert(false, "Async task failed as expected");
    } catch (e) { 
        assert(true, "Async task failed as expected");
    }

    // Primitives getters
    assertEquals(module.GetManagedInt(123456).value, 123456, "GetInt works");
    assertEquals(module.GetManagedLong(123456).value, 123456, "GetBool works");
    assertEquals(module.GetManagedFloat(123456.3).value, 123456.3, "GetFloat works (around 7 decimal digits)");
    assertEquals(module.GetManagedDouble(123456.3339944).value, 123456.3339944, "GetDouble works (around 13 decimal digits");
    assertEquals(module.GetManagedBool(true).value, true, "GetBool true works");
    assertEquals(module.GetManagedBool(false).value, false, "GetBool true works");
    assertEquals(module.GetManagedString("A string").value, "A string", "GetString works");

    // Primitive arrays getter
    assert(arrayContentsEquals(module.GetManagedIntArray([1, 2, 3]).value, [1, 2, 3]), "GetIntArray works");
    assert(arrayContentsEquals(module.GetManagedDoubleArray([1.1, 2.2, 3.3]).value, [1.1, 2.2, 3.3]), "GetDoubleArray works");
    assert(arrayContentsEquals(module.GetManagedFloatArray([1.1, 2.2, 3.3]).value, [1.1, 2.2, 3.3]), "GetFloatArray works");
    assert(arrayContentsEquals(module.GetManagedBoolArray([true, false, true]).value, [true, false, true]), "GetBoolArray works");
    assert(arrayContentsEquals(module.GetManagedByteArray(new Uint8Array([1, 2, 3])).value, [1, 2, 3]), "GetByteArray works");

    // Methods with parameters
    // Download image as byte array and pass it to C#
    const imageByteArray = await downloadJPGImageToUint8Array(200, 300);
    const getManagedByteArray = module.GetManagedByteArray(imageByteArray);
    const imageDebug = module.GetImageInformation(getManagedByteArray);
    assert(imageDebug.value.includes("200x300"), "GetManagedByteArray works")

    // Methods with callbacks
    let valueToSet = "";
    const managedActionVoid = module.GetManagedActionVoid(()=> {
        valueToSet = "Callback invoked";
    });
    module.InvokeCallbackForTest(managedActionVoid);
    assertEquals(valueToSet, "Callback invoked", "Action created an invoked");
};

runTests();