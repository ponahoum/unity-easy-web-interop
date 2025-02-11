import { UnityInstance } from "./UnityInstance";
import { RunTests as RunTestsPrimitiveGetters } from "./tests/test-primitive-getters.js";
import { RunTests as RunTestsGetBasicValues } from "./tests/test-get-basic-values-static.js";


const waitForUnityInstance = async function (): Promise<UnityInstance> {
    while (!(window as any).UnityInstance) {
      await new Promise((resolve) => setTimeout(resolve, 100));
    }
    return (window as any).UnityInstance;
  };
  
const runTests = async function(): Promise<void>{
    const unityInstance = await waitForUnityInstance();
    //console.log(unityModule.Module.static["Nahoum.UnityJSInterop.Tests"].TestGetBasicValues.GetTestDouble().value)
    // Run tests
    RunTestsPrimitiveGetters(unityInstance);
    RunTestsGetBasicValues(unityInstance);
}

runTests();