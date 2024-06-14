import { UnityInstance } from "../UnityJSInterop";
export async function RunTestCallbackActions(unityInstance: UnityInstance): Promise<void> {
    const instance = unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestActionCallbacks.GetInstance();

    // Test simple callback
    // TO DO: Generate actions methods


}