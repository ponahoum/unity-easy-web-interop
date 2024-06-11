import { UnityInstance } from "../UnityJSInterop";
import { assertEqual } from "../test-utilities/test-asserts";

export function RunEmbeddedUtilitiesTests(unityInstance: UnityInstance){
    // New tests
    assertEqual(unityInstance.Module.utilities.GetManagedInt(123456).value, 123456, "GetManagedInt");
    assertEqual(unityInstance.Module.utilities.GetManagedLong(123456).value, 123456, "GetManagedLong");
    assertEqual(unityInstance.Module.utilities.GetManagedFloat(123.3).value, 123.3, "GetManagedFloat");
    assertEqual(unityInstance.Module.utilities.GetManagedDouble(123.3339944).value, 123.3339944, "GetManagedDouble");
    assertEqual(unityInstance.Module.utilities.GetManagedBool(true).value, true, "GetManagedBool true");
    assertEqual(unityInstance.Module.utilities.GetManagedBool(false).value, false, "GetManagedBool false");
    assertEqual(unityInstance.Module.utilities.GetManagedString("A string").value, "A string", "GetManagedString");

}