import { UnityInstance } from "./UnityInstance";
import { RunTests as RunTestsPrimitiveGetters } from "./tests/test-primitive-getters.js";
import { RunTests as RunTestsGetBasicValues } from "./tests/test-get-basic-values-static.js";
import { RunTests as RunTestsInstanceMethods } from "./tests/test-instance-methods.js";
import { RunTests as RunTestsDelegates } from "./tests/test-delegates.js";
import { RunTests as RunTestsAbstractAndInterfaces } from "./tests/test-interface-abstract-class-methods.js";
import { RunTests as RunTestsGettersAndSetters } from "./tests/test-getters-setters.js";
import { RunTests as RunTestsStructs } from "./tests/test-struct.js";
import { RunTests as RunTestsGenericInheritance } from "./tests/test-generic-inheritance.js";
import { RunTests as RunTestsByteArrayTests } from "./tests/test-byte-array.js";
import { RunTests as RunTestsAsyncMethods } from "./tests/test-async-methods.js";
import { RunTests as RunTestsExceptionsHandling } from "./tests/test-exceptions-handling.js";
import { RunTests as RunTestsEvents } from "./tests/test-events.js";
import { RunTests as RunTestsEnums } from "./tests/test-enums.js";
import { RunTests as RunTestsBasicInheritance } from "./tests/test-basic-inheritance.js";

const waitForUnityInstance = async function (): Promise<UnityInstance> {
  while (!(window as any).UnityInstance) {
    await new Promise((resolve) => setTimeout(resolve, 100));
  }
  return (window as any).UnityInstance;
};

const runTests = async function (): Promise<void> {
  const unityInstance = await waitForUnityInstance();

  // Run all tests sequentially
  RunTestsPrimitiveGetters(unityInstance);
  RunTestsGetBasicValues(unityInstance);
  RunTestsInstanceMethods(unityInstance);
  RunTestsDelegates(unityInstance);
  RunTestsAbstractAndInterfaces(unityInstance);
  RunTestsGettersAndSetters(unityInstance);
  RunTestsStructs(unityInstance);
  RunTestsGenericInheritance(unityInstance);
  RunTestsEvents(unityInstance);
  RunTestsEnums(unityInstance);
  RunTestsBasicInheritance(unityInstance);

  // Some tests are async, hence the await
  await RunTestsByteArrayTests(unityInstance);
  await RunTestsAsyncMethods(unityInstance);
  await RunTestsExceptionsHandling(unityInstance);

  // Notice all tests passed in green and bold
  console.log("%cAll tests passed", "color: green; font-weight: bold;");
};

runTests();
