const waitForModule = async function () {
  while (!window.Module) {
    await new Promise((resolve) => setTimeout(resolve, 100));
  }
  return window.Module;
};

var unityModule = await waitForModule();

import { RunTests as RunTestsGetBasicValuesStatic } from "./test-get-basic-values-static.js";
import { RunTests as RunTestsPrimitiveGetters} from "./tests-primitive-getters.js";
import { RunTests as RunTestsByteArrayTests } from "./test-byte-array.js";
import { RunTests as RunTestsInstanceMethods } from "./test-instance-methods.js";
import { RunTests as RunTestsAsyncMethods } from "./test-async-methods.js";
import { RunTests as RunTestsExceptionsHandling } from "./test-exceptions-handling.js";
import { RunTests as RunTestsDelegates } from "./test-delegates.js";
import { RunTests as RunTestsAbstractAndInterfaces } from "./test-interface-abstract-class-methods.js";
import { RunTests as RunTestsGettersAndSetters } from "./test-getters-setters.js";


// Run all tests sequentially
RunTestsPrimitiveGetters(unityModule);
RunTestsGetBasicValuesStatic(unityModule);
RunTestsInstanceMethods(unityModule);
RunTestsDelegates(unityModule);
RunTestsAbstractAndInterfaces(unityModule);
RunTestsGettersAndSetters(unityModule);

// Some tests are async, hence the await
await RunTestsByteArrayTests(unityModule);
await RunTestsAsyncMethods(unityModule);
await RunTestsExceptionsHandling(unityModule);
