import { AnotherNamespace, System, UnityInstance } from "../UnityInstance.js";
import { assertEquals, SampleValues } from "./test-utilities.js";

/**
 * Few tests to handling delegates
 */

export function RunTests(unityInstance: UnityInstance) {
  RunTestWithManualDelegateConstructor(unityInstance);
  RunTestWithAutoDelegateConstructor(unityInstance);
}

function RunTestWithManualDelegateConstructor(unityInstance: UnityInstance) {
  const module = unityInstance.Module;
  const instance = module.static["Nahoum.UnityJSInterop.Tests"].TestActionCallbacks.GetInstance();

  // Test callback with no arguments
  let valueToSet = null;
  const managedActionVoid = module.utilities.GetManagedAction(() => {
    valueToSet = "Callback invoked";
  }, []);
  instance.TestInvokeCallbackVoid(managedActionVoid);
  assertEquals(valueToSet, "Callback invoked", "TestInvokeCallbackVoid");

  // Test callback with int as argument
  let intValueToSet = null;
  const managedActionInt = module.utilities.GetManagedAction(
    (ptr: System.Int32) => {
      intValueToSet = ptr.value;
    },
    ["System.Int32"]
  );
  instance.TestInvokeCallbackInt(managedActionInt);
  assertEquals(intValueToSet, SampleValues.TestInt, "TestInvokeCallbackInt");

  // Test callback with string as argument
  let stringValueToSet = null;
  const managedActionString = module.utilities.GetManagedAction(
    (ptr: System.String) => {
      stringValueToSet = ptr.value;
    },
    ["System.String"]
  );
  instance.TestInvokeCallbackString(managedActionString);
  assertEquals(stringValueToSet, SampleValues.TestString, "TestInvokeCallbackString");

  // Test callback with float as argument
  let floatValueToSet = null;
  const managedActionFloat = module.utilities.GetManagedAction(
    (ptr: System.Single) => {
      floatValueToSet = ptr.value;
    },
    ["System.Single"]
  );
  instance.TestInvokeCallbackFloat(managedActionFloat);
  assertEquals(floatValueToSet, SampleValues.TestFloat, "TestInvokeCallbackFloat");

  // Test callback with double and string as arguments
  let doubleValueToSet: null | System.Double = null;
  let stringDoubleValueToSet: null | System.String = null;
  const managedActionDoubleString = module.utilities.GetManagedAction(
    (doublePtr: System.Double, stringPtr: System.String) => {
      doubleValueToSet = doublePtr;
      stringDoubleValueToSet = stringPtr;
    },
    ["System.Double", "System.String"]
  );

  instance.TestInvokeDoubleStringCallback(managedActionDoubleString);
  // Right after the callback is invoked, the values should be set
  if (doubleValueToSet == null || stringDoubleValueToSet == null) {
    throw new Error("Double or String value is null");
  }

  const asDouble: System.Double = doubleValueToSet;
  const asString: System.String = stringDoubleValueToSet;
  assertEquals(asDouble.value, SampleValues.TestDouble, "TestInvokeDoubleStringCallback - Double");
  assertEquals(asString.value, SampleValues.TestString, "TestInvokeDoubleStringCallback - String");

  // Test callback with class outside namespace as argument
  let classInstanceToSet: AnotherNamespace.SomeClass | null = null;
  const managedActionClass = module.utilities.GetManagedAction(
    (ptr: AnotherNamespace.SomeClass) => {
      classInstanceToSet = ptr;
    },
    ["AnotherNamespace.SomeClass"]
  );
  instance.TestInvokeActionWithClassOutsideNamespace(managedActionClass);
  // Right after the callback is invoked, the values should be set
  if (classInstanceToSet == null) {
    throw new Error("Class instance is null");
  }
  const asSomeClass: AnotherNamespace.SomeClass = classInstanceToSet;
  assertEquals(asSomeClass.managedType.value, "AnotherNamespace.SomeClass", "TestInvokeActionWithClassOutsideNamespace");
  const simpleValue = asSomeClass.TestGetSimpleValue();
  assertEquals(simpleValue.value, SampleValues.TestInt, "TestInvokeActionWithClassOutsideNamespace");
}

/**
 * Run the same tests but create the delegates via auto generated constructor in extras
 */
export function RunTestWithAutoDelegateConstructor(unityInstance: UnityInstance) {
  const module = unityInstance.Module;

  const instance = module.static["Nahoum.UnityJSInterop.Tests"].TestActionCallbacks.GetInstance();

  // Test callback with no arguments
  let valueToSet = null;
  const managedActionVoid = module.extras["System"]["Action"].createDelegate(() => {
    valueToSet = "Callback invoked";
  });
  instance.TestInvokeCallbackVoid(managedActionVoid);
  assertEquals(valueToSet, "Callback invoked", "TestInvokeCallbackVoid with auto delegate constructor");

  // Test callback with int as argument
  let intValueToSet = null;
  const managedActionInt = module.extras["System"]["Action<Int32>"].createDelegate(
    (ptr) => {
      intValueToSet = ptr.value;
    }
  );
  instance.TestInvokeCallbackInt(managedActionInt);
  assertEquals(intValueToSet, SampleValues.TestInt, "TestInvokeCallbackInt with auto delegate constructor");

  // Test callback with string as argument
  let stringValueToSet = null;
  const managedActionString = module.extras["System"]["Action<String>"].createDelegate(
    (ptr) => {
      stringValueToSet = ptr.value;
    }
  );
  instance.TestInvokeCallbackString(managedActionString);
  assertEquals(stringValueToSet, SampleValues.TestString, "TestInvokeCallbackString with auto delegate constructor");

  // Test callback with float as argument
  let floatValueToSet = null;
  const managedActionFloat = module.extras["System"]["Action<Single>"].createDelegate(
    (ptr) => {
      floatValueToSet = ptr.value;
    }
  );

  instance.TestInvokeCallbackFloat(managedActionFloat);
  assertEquals(floatValueToSet, SampleValues.TestFloat, "TestInvokeCallbackFloat with auto delegate constructor");

  // Test callback with double and string as arguments
  let doubleValueToSet = null;
  let stringDoubleValueToSet = null;
  const managedActionDoubleString = module.extras["System"]["Action<Double,String>"].createDelegate(
    (doublePtr, stringPtr) => {
      doubleValueToSet = doublePtr.value;
      stringDoubleValueToSet = stringPtr.value;
    }
  );
  instance.TestInvokeDoubleStringCallback(managedActionDoubleString);
  assertEquals(doubleValueToSet, SampleValues.TestDouble, "TestInvokeDoubleStringCallback - Double with auto delegate constructor");
  assertEquals(stringDoubleValueToSet, SampleValues.TestString, "TestInvokeDoubleStringCallback - String with auto delegate constructor");

  // Test callback with class outside namespace as argument
  let classInstanceToSet: AnotherNamespace.SomeClass | null = null;
  const managedActionClass = module.extras["System"]["Action<AnotherNamespace.SomeClass>"].createDelegate(
    (ptr) => {
      classInstanceToSet = ptr;
    }
  );
  instance.TestInvokeActionWithClassOutsideNamespace(managedActionClass);

  // Check null for ts compilation
  if (classInstanceToSet == null) {
    throw new Error("Class instance is null");
  }
  // Assign to a well typed variable now that we know it's not null
  const classInstanceWellTyped: AnotherNamespace.SomeClass = classInstanceToSet;

  assertEquals(classInstanceWellTyped.managedType.value, "AnotherNamespace.SomeClass", "TestInvokeActionWithClassOutsideNamespace with auto delegate constructor");
  const simpleValue = classInstanceWellTyped.TestGetSimpleValue();
  assertEquals(simpleValue.value, SampleValues.TestInt, "TestInvokeActionWithClassOutsideNamespace with auto delegate constructor");
}
