const assert = function (condition, testName) {
    if (!condition)
        throw Error('The following test failed: ' + testName);
    else
        console.log('The following test passed: ' + testName);
};

const assertEquals = function (expected, actual, testName) {
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

    // Methods with parameters

    // Methods with callbacks
};

runTests();