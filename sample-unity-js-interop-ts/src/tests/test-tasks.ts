import { UnityInstance } from "../UnityJSInterop";
import { SampleValues } from "../test-utilities/sample-values";
import { assertEqual } from "../test-utilities/test-asserts";
export async function RunTestTasks(unityInstance: UnityInstance): Promise<void> {
    const instance = unityInstance.Module.static.Nahoum.UnityJSInterop.Tests.TestTasks.GetInstance();

    var testFloat = await instance.TestTaskFloat()
    assertEqual(testFloat.value, SampleValues.TestFloat, "TestTaskFloat");

    var testInt = await instance.TestTaskInt();
    assertEqual(testInt.value, SampleValues.TestInt, "TestTaskInt");

    var testString = await instance.TestTaskString();
    assertEqual(testString.value, SampleValues.TestString, "TestTaskString");

    var testBool = await instance.TestTaskVoid();

}