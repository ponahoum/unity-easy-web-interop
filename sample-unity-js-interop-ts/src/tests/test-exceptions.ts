import { UnityInstance } from "../UnityJSInterop";
import { assertThrows, assertThrowsAsync } from "../test-utilities/test-asserts";

export async function RunTestExceptions(unityInstance: UnityInstance){
    const instance = unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestExceptions;
    assertThrows(() => instance.ThrowSimpleException(), "Simple exception");
    assertThrows(() => instance.TestUnraisedException(), "Complex exception");
    await assertThrowsAsync(() => instance.ThrowSimpleExceptionAsync(), "Simple exception async");
    
    // Not supported yet
    //await assertThrowsAsync(() => instance.TestUnraisedExceptionAsync(), "Complex exception async");

}