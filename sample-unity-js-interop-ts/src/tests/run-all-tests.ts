import { UnityInstance } from "../UnityJSInterop";
import { RunEmbeddedUtilitiesTests } from "./test-embedded-utilities";
import { RunTestGetBasicValues } from "./test-get-basic-values";

export function RunAllTests(unityInstance: UnityInstance){
    RunEmbeddedUtilitiesTests(unityInstance);
    RunTestGetBasicValues(unityInstance);
}