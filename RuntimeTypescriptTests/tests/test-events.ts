import { UnityInstance } from "../UnityInstance.js";
import { assertArrayContentsEquals, assertEquals } from "./test-utilities.js";

/**
 * Test events binding
 */
export function RunTests(unityInstance: UnityInstance) {
    const utilities = unityInstance.Module.utilities;
    const extras = unityInstance.Module.extras;
    const testString = "Hello 12345";

    // Test events the manual way
    const instanceForTestEvents = unityInstance.Module.static["Nahoum.UnityJSInterop.Tests"].TestEvents.GetNewInstance();

    // Create the callback
    let resultAuto: string | null = null;
    let resultManual: string | null = null;

    // Add events with the auto way
    const delegateAuto = extras.System["Action<String>"].createDelegate((str) => {
        resultAuto = str.value;
    });
    instanceForTestEvents.add_TestEventStringAuto(delegateAuto);

    // Add events with the manual way
    const delegateManual = extras.System["Action<String>"].createDelegate((str) => {
        resultManual = str.value;
    });
    instanceForTestEvents.add_TestEventStringManual(delegateManual);

    // Ensure result is null
    assertEquals(resultAuto, null, "The auto registered event has not been triggered yet");
    assertEquals(resultManual, null, "The manual registered event has not been triggered yet");

    // Now trigger event
    instanceForTestEvents.InvokeEvent(utilities.GetManagedString(testString));

    // Ensure result is the test string
    assertEquals(resultAuto, testString, "The auto registered event has been triggered");
    assertEquals(resultManual, testString, "The manual registered event has been triggered");

    // Now try to unregister the events
    instanceForTestEvents.remove_TestEventStringManual(delegateManual);
    instanceForTestEvents.remove_TestEventStringAuto(delegateAuto);

    // Trigger the event again
    instanceForTestEvents.InvokeEvent(utilities.GetManagedString(testString + "x"));

    // Ensure result is still the test string and not the new one (+x)
    assertEquals(resultAuto, testString, "The unregister (auto) event has not been triggered again");
    assertEquals(resultManual, testString, "The unregister (manual) event has not been triggered again");
}
