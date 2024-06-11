import { UnityInstance } from "../UnityJSInterop";
import { RunEmbeddedUtilitiesTests } from "./test-embedded-utilities";

export function RunAllTests(unityInstance: UnityInstance){
    RunEmbeddedUtilitiesTests(unityInstance);
}