import { UnityInstance } from "../UnityInstance.js";
import { assertEquals, SampleValues } from "./test-utilities.js";


/**
 * Test ensuring that methods can be called up to the limit of 10 arguments of the library
 */
export function RunTests(unityInstance: UnityInstance) {

  // Get module
  const module = unityInstance.Module;
  const testStaticClass = module.static["Nahoum.UnityJSInterop.Tests"].TestMethodWithALotOfArguments;
  const testInt = module.utilities.GetManagedInt(1);

  // Test 0 arg
  assertEquals(testStaticClass.OneArgument(testInt).value, 1, "Test0Args");
  assertEquals(testStaticClass.TwoArguments(testInt, testInt).value, 2, "Test1Arg");
  assertEquals(testStaticClass.ThreeArguments(testInt, testInt, testInt).value, 3, "Test2Args");
  assertEquals(testStaticClass.FourArguments(testInt, testInt, testInt, testInt).value, 4, "Test3Args");
  assertEquals(testStaticClass.FiveArguments(testInt, testInt, testInt, testInt, testInt).value, 5, "Test4Args");
  assertEquals(testStaticClass.SixArguments(testInt, testInt, testInt, testInt, testInt, testInt).value, 6, "Test5Args");
  assertEquals(testStaticClass.SevenArguments(testInt, testInt, testInt, testInt, testInt, testInt, testInt).value, 7, "Test6Args");
  assertEquals(testStaticClass.EightArguments(testInt, testInt, testInt, testInt, testInt, testInt, testInt, testInt).value, 8, "Test7Args");
  assertEquals(testStaticClass.NineArguments(testInt, testInt, testInt, testInt, testInt, testInt, testInt, testInt, testInt).value, 9, "Test8Args");
  assertEquals(testStaticClass.TenArguments(testInt, testInt, testInt, testInt, testInt, testInt, testInt, testInt, testInt, testInt).value, 10, "Test9Args");
}
