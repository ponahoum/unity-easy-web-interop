import { UnityInstance } from "../UnityInstance.js";
import { assertArrayContentsEquals, assertEquals } from "./test-utilities.js";

/**
 * Test large arrays which tend to make OOM when wrongly handled on the serialization sides
 */
export function RunTests(unityInstance: UnityInstance) {
  const utilities = unityInstance.Module.utilities;

  const arraySize = 20000000;
  // Make a 20 000 000 sized byte array
  const largeArray: number[] = [];
  for (let i = 0; i < arraySize; i++) {
    // Random integer between 0 and 255
    largeArray[i] = Math.floor(Math.random() * 256);
  }
  const managedByteArray = utilities.GetManagedByteArray(new Uint8Array(largeArray));

    // Serialize it
  const serialized = managedByteArray.value;

  // Asserts the values in it are equal individually to the original array
  assertArrayContentsEquals(serialized, largeArray, "Ensure large serialized managed byte array matches original");
}
