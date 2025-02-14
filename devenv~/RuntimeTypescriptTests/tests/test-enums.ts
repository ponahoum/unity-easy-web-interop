import { UnityInstance } from "../UnityInstance.js";
import {
  assertEquals,
  SampleValues,
  assertArrayContentsEquals,
  assertObjectEqualsByValues,
} from "./test-utilities.js";

export function RunTests(unityInstance: UnityInstance) {
  // Get module
  const module = unityInstance.Module;

  // Get instance
  const instance =
    module.static["Nahoum.UnityJSInterop.Tests"].TestEnums.GetInstance();

  // Ensure values are properly serialized
  assertEquals(
    instance.GetEnumFirstValue().value,
    SampleValues.TestEnum.red,
    "GetEnumFirstValue"
  );

  assertEquals(
    instance.GetEnumSecondValue().value,
    SampleValues.TestEnum.green,
    "GetEnumSecondValue"
  );

  assertEquals(
    instance.GetEnumThirdValue().value,
    SampleValues.TestEnum.blue,
    "GetEnumThirdValue"
  );

  assertEquals(instance.GetEnumValueAsString(instance.GetEnumThirdValue()).value, SampleValues.TestEnum.blue, "GetEnumValueAsString");
}
