import { assertEquals, assertArrayContentsEquals } from "./testing-utilities.js";
import { SampleValues } from "./testing-utilities.js";

/**
 * Few tests to handling delegates
 */
export function RunTests(module) {
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
    const managedActionInt = module.utilities.GetManagedAction((ptr) => {
        intValueToSet = ptr.value;
    }, ["System.Int32"]);
    instance.TestInvokeCallbackInt(managedActionInt);
    assertEquals(intValueToSet, SampleValues.TestInt, "TestInvokeCallbackInt");

    // Test callback with string as argument
    let stringValueToSet = null;
    const managedActionString = module.utilities.GetManagedAction((ptr) => {
        stringValueToSet = ptr.value;
    }, ["System.String"]);
    instance.TestInvokeCallbackString(managedActionString);
    assertEquals(stringValueToSet, SampleValues.TestString, "TestInvokeCallbackString");

    // Test callback with float as argument
    let floatValueToSet = null;
    const managedActionFloat = module.utilities.GetManagedAction((ptr) => {
        floatValueToSet = ptr.value;
    }, ["System.Single"]);
    instance.TestInvokeCallbackFloat(managedActionFloat);
    assertEquals(floatValueToSet, SampleValues.TestFloat, "TestInvokeCallbackFloat");

    // Test callback with double and string as arguments
    let doubleValueToSet = null;
    let stringDoubleValueToSet = null;
    const managedActionDoubleString = module.utilities.GetManagedAction((doublePtr, stringPtr) => {
        doubleValueToSet = doublePtr.value;
        stringDoubleValueToSet = stringPtr.value;
    }, ["System.Double", "System.String"]);
    instance.TestInvokeDoubleStringCallback(managedActionDoubleString);
    assertEquals(doubleValueToSet, SampleValues.TestDouble, "TestInvokeDoubleStringCallback - Double");
    assertEquals(stringDoubleValueToSet, SampleValues.TestString, "TestInvokeDoubleStringCallback - String");

    // Test callback with class outside namespace as argument
    let classInstanceToSet = null;
    const managedActionClass = module.utilities.GetManagedAction((ptr) => {
        classInstanceToSet = ptr;
    }, ["AnotherNamespace.SomeClass"]);
    instance.TestInvokeActionWithClassOutsideNamespace(managedActionClass);
    assertEquals(classInstanceToSet.managedType.value, "AnotherNamespace.SomeClass", "TestInvokeActionWithClassOutsideNamespace");
    const simpleValue = classInstanceToSet.TestGetSimpleValue();
    assertEquals(simpleValue.value, SampleValues.TestInt, "TestInvokeActionWithClassOutsideNamespace");
}
