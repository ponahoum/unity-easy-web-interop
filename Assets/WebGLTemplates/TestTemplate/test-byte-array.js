import { assertEquals, assertArrayContentsEquals } from "./testing-utilities.js";
import { SampleValues } from "./testing-utilities.js";

/**
 * Few tests to check the byte array conversion from Unity to JS
 * Loading a texture as an example
 */
export async function RunTests(module) {
  // Get static class
  const staticClass = module.static.Nahoum.UnityJSInterop.Tests.TestByteArray;

  // Build a UInt8Array
  const width = 500;
  const height = 600;
  const imageUint8Array = await downloadJPGImageToUint8Array(width, height);

  // Get byte array from Unity given a UInt8Array
  const uint8arrayUnity = module.utilities.GetManagedByteArray(imageUint8Array);

  // Get byte array length
  assertEquals(staticClass.GetByteArrayLength(uint8arrayUnity).value, imageUint8Array.length, "GetByteArrayLength");

  // Load texture from byte array
  const unitySideTexture = staticClass.LoadTexture(uint8arrayUnity);
  assertEquals(unitySideTexture.managedType.value, "UnityEngine.Texture2D", "LoadTexture");

  // Get texture resolution an ensure it is the same as the downloaded image
  assertEquals(staticClass.GetTextureResolution(unitySideTexture).value, `${width}x${height}`, "GetTextureResolution")
}

/**
 *  Download sample image and return it as a Uint8Array
 */

async function downloadJPGImageToUint8Array(resX, resY) {
  try {
    // Fetch the image
    const url = "https://picsum.photos/" + resX + "/" + resY;
    const response = await fetch(url);

    // Check if the response is ok
    if (!response.ok) {
      throw new Error(`Network response was not ok: ${response.statusText}`);
    }

    // Get the image as a blob
    const blob = await response.blob();

    // Convert the blob to an ArrayBuffer
    const arrayBuffer = await blob.arrayBuffer();

    // Create a Uint8Array from the ArrayBuffer
    const uint8Array = new Uint8Array(arrayBuffer);

    // Return the Uint8Array
    return uint8Array;
  } catch (error) {
    console.error("Error downloading or converting the image:", error);
  }
}
