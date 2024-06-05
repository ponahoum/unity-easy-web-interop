

// Wait for window.Module to be defined
const waitForModule = async function () {
    while (!window.Module) {
        await new Promise(resolve => setTimeout(resolve, 100));
    }
    return window.Module;
};

const runTests = async function () {
    const module = await waitForModule();
    console.log('Module loaded. Test running...');

    // Primitives getters
    assertEquals(module.utilities.GetManagedInt(123456).value, 123456, "GetInt works");
    assertEquals(module.utilities.GetManagedLong(123456).value, 123456, "GetBool works");
    assertEquals(module.utilities.GetManagedFloat(123456.3).value, 123456.3, "GetFloat works (around 7 decimal digits)");
    assertEquals(module.utilities.GetManagedDouble(123456.3339944).value, 123456.3339944, "GetDouble works (around 13 decimal digits");
    assertEquals(module.utilities.GetManagedBool(true).value, true, "GetBool true works");
    assertEquals(module.utilities.GetManagedBool(false).value, false, "GetBool true works");
    assertEquals(module.utilities.GetManagedString("A string").value, "A string", "GetString works");

    // Primitive arrays getter
    assert(arrayContentsEquals(module.utilities.GetManagedIntArray([1, 2, 3]).value, [1, 2, 3]), "GetIntArray works");
    assert(arrayContentsEquals(module.utilities.GetManagedDoubleArray([1.1, 2.2, 3.3]).value, [1.1, 2.2, 3.3]), "GetDoubleArray works");
    assert(arrayContentsEquals(module.utilities.GetManagedFloatArray([1.1, 2.2, 3.3]).value, [1.1, 2.2, 3.3]), "GetFloatArray works");
    assert(arrayContentsEquals(module.utilities.GetManagedBoolArray([true, false, true]).value, [true, false, true]), "GetBoolArray works");
    assert(arrayContentsEquals(module.utilities.GetManagedByteArray(new Uint8Array([1, 2, 3])).value, [1, 2, 3]), "GetByteArray works");

    // Instance methods and async methods
    const instance = module.static.RuntimeWebTests.GetNewTestInstance()
    assertEquals(instance.MyMethodReturningString().value, "returning a random string", "Method returning string works");
    assertEquals(instance.MyMethodReturningDouble().value, 2147483647125, "Method returning double works");
    assertEquals(instance.MyMethodReturningInt().value, 450050554, "Method returning int works");
    assertEquals(instance.TestReturnNullValue().value, null, "Method returning null works");
    assertEquals((await instance.AsyncTaskReturnString()).value, "A cool result from an async task", "AsyncTaskReturnString (Async Task<string>) returns correct string value");
    assert(instance.AsyncTaskReturnString().then, "AsyncTaskReturnString (Async Task<string>) is a promise");
    assertEquals(await instance.AsyncTaskVoidMethod(), undefined, "Awaiting AsyncTaskVoidMethod returns undefined");
    assert(instance.AsyncTaskVoidMethod().then, "AsyncTaskVoidMethod is a promise");
    assertEquals(instance.AsyncVoidMethod(), undefined, "AsyncVoidMethod is not a promise and is undefined");
    assert(arrayContentsEquals(instance.MethodReturningDoubleArray().value, [123, 789, 101112]), "MethodReturningDoubleArray");
    // Method with parameter - Download image as byte array and pass it to C#
    const imageByteArray = await downloadJPGImageToUint8Array(200, 300);
    const getManagedByteArray = module.utilities.GetManagedByteArray(imageByteArray);
    const imageDebug = instance.GetImageInformation(getManagedByteArray);
    assert(imageDebug.value.includes("200x300"), "GetManagedByteArray works")

    // Methods with callbacks
    // Test callback Action
    runCallbackTests(module, instance);

    // Exception catching
    await runExceptionCatchingTests(instance);

    // green test in console telling all tests passed
    console.log("%cAll tests passed", "color: green; font-size: 20px");
};

runTests();

const runExceptionCatchingTests = async function (instance) {

    // Explicitely raised exception
    try {
        instance.TestInvokedException();
        assert.false("Exception not raised");
    } catch (e) {
        assert(true, "Exception raised as expected");
    }

    // Exception in Task
    try {
        await instance.AsyncTaskStringExplicitelyFail();
        assert(false, "Async task failed as expected");
    } catch (e) {
        assert(true, "Async task failed as expected");
    }

    // Unraised exception
    try {
        instance.TestUnraisedException();
        assert(false, "Exception not raised");
    } catch (e) {
        assert(true, "Exception raised as expected");
    }

    // Unraised exception in task
    // try {
    //     await instance.AsyncTaskUnraisedException();
    //     assert(false, "Async task unraised exception not raised");
    // } catch (e) {
    //     assert(true, "Async task natural exception raised as expected");
    // }

}
const runCallbackTests = function (module, instance) {
    let valueToSet = null;
    const managedActionVoid = module.utilities.GetManagedAction(() => {
        valueToSet = "Callback invoked";
    }, []);
    instance.InvokeCallbackForTest(managedActionVoid);
    assertEquals(valueToSet, "Callback invoked", "Action created an invoked");

    // Test callback Action<string>
    let valueToSet2 = null;
    const managedActionString = module.utilities.GetManagedAction((str) => {
        valueToSet2 = str;
    }, ["System.String"]);
    instance.InvokeCallbackWithActionString(managedActionString);
    assertEquals(valueToSet2.value, "Some callback string");

    // Test callback Action<string, double>
    let targetString = null;
    let targetDouble = null;
    const managedActionStringDouble = module.utilities.GetManagedAction((str, dbl) => {
        targetString = str;
        targetDouble = dbl;
    }, ["System.String", "System.Double"]);
    instance.InvokeCallbackWithActionStringDouble(managedActionStringDouble);
    assertEquals(targetString.value, "Some callback string");
    assertEquals(targetDouble.value, 12345);
}

const assert = function (condition, testName) {
    if (!condition)
        throw Error('%cThe following test failed: ' + testName, 'color: red');
    else
        console.log('%cThe following test passed: ' + testName, 'color: green');
};

const assertEquals = function (actual, expected, testName) {
    if (expected !== actual)
        throw Error("%cThe following test failed: " + testName + ". Expected: " + expected + " but got: " + actual, 'color: red')
    else
        console.log("%cThe following test passed: " + testName, 'color: green');
}

/**
 * Check two array have the same values inside
 */
const arrayContentsEquals = function (array1, array2) {
    if (array1.length !== array2.length)
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
        const url = "https://picsum.photos/" + resX + '/' + resY;
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
