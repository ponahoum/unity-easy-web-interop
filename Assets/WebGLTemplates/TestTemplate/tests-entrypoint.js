const waitForModule = async function () {
  while (!window.Module) {
    await new Promise((resolve) => setTimeout(resolve, 100));
  }
  return window.Module;
};

var unityModule = await waitForModule();

import { RunTests as RunTestGetBasicValuesStaticTests } from "./test-get-basic-values-static.js";
import { RunTests as RunPrimitiveGettersTests } from "./tests-primitive-getters.js";
import { RunTests as RunTestsByteArrayTests } from "./test-byte-array.js";
import { RunTests as RunInstanceMethodsTests } from "./test-instance-methods.js";
import { RunTests as RunTestsAsyncMethods } from "./test-async-methods.js";
import { RunTests as RunTestsExceptionsHandling } from "./test-exceptions-handling.js";
import { RunTests as RunTestDelegates } from "./test-delegates.js";
import { RunTests as RunTestsAbstractAndInterfaces } from "./test-interface-abstract-class-methods.js";


// Run all tests sequentially
RunPrimitiveGettersTests(unityModule);
RunTestGetBasicValuesStaticTests(unityModule);
RunInstanceMethodsTests(unityModule);
RunTestDelegates(unityModule);
RunTestsAbstractAndInterfaces(unityModule);

// Some tests are async, hence the await
await RunTestsByteArrayTests(unityModule);
await RunTestsAsyncMethods(unityModule);
await RunTestsExceptionsHandling(unityModule);
