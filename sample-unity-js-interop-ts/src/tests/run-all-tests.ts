import { UnityInstance } from "../UnityJSInterop";
import { RunEmbeddedUtilitiesTests } from "./test-embedded-utilities";
import { RunTestExceptions } from "./test-exceptions";
import { RunTestGetBasicValues } from "./test-get-basic-values";
import { RunTestInstanceMethods } from "./test-instance-methods";
import { RunTestTasks } from "./test-tasks";

export async function RunAllTests(unityInstance: UnityInstance) {

    try {
        RunEmbeddedUtilitiesTests(unityInstance);
        RunTestGetBasicValues(unityInstance);
        RunTestInstanceMethods(unityInstance);
        await RunTestExceptions(unityInstance);
        await RunTestTasks(unityInstance);

        // Notice tests done with a green colored text
        console.log("%c All tests passed !", "color: green");
    }
    catch(error) {
        // Notice tests done with a red colored text
        console.log("%c Some tests failed !", "color: red");
        console.log(error);
    }
}