# Unity Easy WebGL & Javascript Interop

**Unity Easy Web Interop** is a library for Unity WebGL users that allows to expose **C# methods and class instances to JavaScript** using simple C# decorators. It also generates easy-to-use, strongly typed signatures for TypeScript.

It's build using direct bindings to the WASM side of Unity WebGL and **does NOT rely on messaging or serialization systems** (such as Unity’s `SendMessage`). 

It’s perfect for developing Unity applications that require a modern web UI overlay built with frameworks like **React, Vue, or Angular**.

## Summary

- [Getting started](#getting-started)
- [Installation & Supported Unity Versions](#installation--supported-unity-versions)
 - [Documentation](#documentation)
    - [Accessing Primitive Types](#accessing-primitive-types)
    - [Using Callbacks (Action & Event)](#using-callbacks)
    - [Asynchronous logic (Task / Promises)](#asynchronous-logic-task--promises)
    - [Using Get/Set](#using-getset)
    - [Exception handling](#exception-handling)
    - [Handling lists & arrays](#handling-lists--arrays)
    - [Inheritance, abstract classes and interfaces](#inheritance-abstract-classes-and-interfaces)
    - [Custom TypeScript generation](#custom-typescript-generation)
    - [Custom serialization](#custom-serialization)

## Getting started

Consider you want the following C# code directly to Javascript:

```csharp
namespace Nahoum.UnityJSInterop.Tests
{
    public class TestInstanceMethods
    {
        [ExposeWeb]
        public static TestInstanceMethods GetNewInstance() => new TestInstanceMethods();

        [ExposeWeb]
        public int TestGetInt() => 25;

        [ExposeWeb]
        public int TestAdditionInt(int a, int b) => a + b;
    }
}
```

By applying the `[ExposeWeb]` attribute to these methods, they become accessible in your JavaScript WebGL build. Static methods are accessed via the path `unityInstance.Module.static[namespace][className][methodName]`. Static methods allow you to expose instances, meaning both static and instance methods are available to JavaScript.

### Using the Exposed Methods in JavaScript

```javascript
// Create an instance using the exposed static constructor.
// 'myInstance' now acts as a pointer to the C# memory instance.
const myInstance = unityInstance.Module.static["Nahoum.UnityJSInterop.Tests"].TestInstanceMethods.GetNewInstance();

// Since the methods TestGetInt and TestAdditionInt were marked with [ExposeWeb], they are available to call from the returned instance
const testInt = myInstance.TestGetInt();

// Retrieve the **serialized value** of the integer.
console.log(testInt.value); // Prints: 25

// Use the pointer to call another method.
const addedInt = myInstance.TestAdditionInt(testInt, testInt);
console.log(addedInt.value); // Prints: 50
```

> **Warning:** Calling `console.log(testInt)` will not display the actual value because the object `testInt` represents a pointer to a segment of C# memory, not the actual value. To retrieve the underlying value, use the `value` property (e.g., `testInt.value`). It important to understand that calling `.value` will only return a serialized value (or a copy if you wish) at the instant you call it, it's not a reference to C# memory that you can modify.

### Generated TypeScript Definitions

For the above C# code, the following TypeScript definitions are generated:

```typescript
export type TestInstanceMethods_static = {
    GetNewInstance(): TestInstanceMethods;
};

export type TestInstanceMethods = {
    value: unknown;
    managedType: System.Type;
    TestGetInt(): System.Int32;
    TestAdditionInt(a: System.Int32, b: System.Int32): System.Int32;
} & TestInstanceMethods_static;

// Type you can bind to index.html's createUnityInstance(...) => {}).then((unityInstance) => {
export type UnityInstance = {
    Module: {
        static: {
            "Nahoum.UnityJSInterop.Tests": {
                TestInstanceMethods: Nahoum.UnityJSInterop.Tests.TestInstanceMethods_static;
            }
        }
    }
}
```
*In the example above, other typescript types (such as Int32) are auto-generated but not shown here for visibility*

> **Note:** Typescript .d.ts file can be generated via the menu `UnityJsInterop > Generate Typescript`
> **Warning:** Note that due to typescript limitations, the static part of classes is splitted in two different types.
These definitions provide type safety and autocompletion when interacting with the exposed methods in TypeScript.

## Installation & supported Unity versions

To install **Unity Easy Web Interop**, use Unity's Package Manager. You can choose one of the following methods:

### Using the Git URL

1. In Unity **Window > Package Manager**.
3. Click the **+** button in the top left corner and select **"Add package from git URL..."**.
4. Enter the following URL and click **Add**:

   ```text
   https://github.com/ponahoum/unity-easy-web-interop.git?path=/Packages/org.nahoum.easy-web-interop
   ```

### Manual Installation

Alternatively, you can clone the repository and copy the package directly into your project's package directory:

1. Clone the repository:
   ```bash
   git clone https://github.com/ponahoum/unity-easy-web-interop.git
   ```
2. Copy the folder located at `Packages/org.nahoum.easy-web-interop` from the cloned repository into your project's package directory.

### Supported Unity version
The package has been tested in both Unity 2022 and Unity 6.

## Documentation

### Accessing Primitive Types

When building interop between two languages, everything starts with handling primitive types. You may need to convert JavaScript data—such as strings, arrays of strings, numbers, etc.—into managed types that can be passed as arguments to your exposed web methods. For most of these types, this package provides utility constructors accessible via the `unityInstance.Module.utilities` path.

#### Examples

Here are some examples demonstrating how to create managed types:

```javascript

const utilities = unityInstance.Module.utilities;

// Convert a JavaScript string to a string on the C# side (string)
const aString = utilities.GetManagedString("hello world");

// Convert a JavaScript array of strings to a string array on the C# side (string[])
const aStringArray = utilities.GetManagedStringArray(["hello", "world"]);

// Convert a JavaScript number to a float the C# side (float)
const aFloat = utilities.GetManagedFloat(1234);

// Convert a JavaScript array of numbers to a float array on the C# side (float[])
const aFloatArray = utilities.GetManagedFloatArray([1, 2, 3, 4]);

// Efficiently pass binary data using a Uint8Array to the C# side (byte[])
const aByteArray = utilities.GetManagedByteArray(new Uint8Array([/* your binary data */]));
```

For the best development experience, generate the corresponding TypeScript definitions to know more about the available constructors for primitive types.

---

### Using Callbacks

#### Using Actions with javascript callbacks
You can leverage C# actions to bind them to JavaScript callbacks. This means that when an action is invoked on the C# side, a JavaScript callback can be executed.
For example, consider a C# class that accepts a callback of type `Action<string>`:

```csharp
public class TestActionCallbacks
{
    [ExposeWeb]
    public static TestActionCallbacks GetInstance()
    {
        return new TestActionCallbacks();
    }

    [ExposeWeb]
    public void TestInvokeCallbackString(Action<string> action)
    {
        action("Hello world");
    }
}
```

Since you added the `[ExposeWeb]` attribute to the `TestInvokeCallbackString` method, a utility method for creating an `Action<string>` bound to a JavaScript callback is automatically added to `Module.extras` under the name of the action (`System.Action<String>`).

Here's how you can create and use the callback in JavaScript / Typescript:

```typescript
const myAction = unityInstance.Module.extras["System"]["Action<String>"].createDelegate(
  (myString: System.String) => {
    console.log(myString.value);
  }
);

// Now pass the action to C#. When C# invokes it, the JavaScript callback will be executed, and you can access the string's value via the `.value` property.
const instance = unityInstance.Module.static["Nahoum.UnityJSInterop.Tests"].TestActionCallbacks.GetInstance();
instance.TestInvokeCallbackString(myAction); // Should print "Hello world" in the JS console.
```

This example demonstrates how seamlessly you can connect C# delegate with JavaScript functions, enabling smooth interoperation between your Unity WebGL application and web interfaces.

#### C# Events

A powerful feature of C# is events. Let's assume you have the following event defined in C#:

```csharp
public class TestEvents
{
    // Using the "property style" to define an event (this could also work with add/remove style)
    [ExposeWeb]
    public event Action<string> TestEventString = delegate { };

    // Triggers the event for testing purposes.
    [ExposeWeb]
    public void InvokeEvent(string value)
    {
        TestEventString.Invoke(value);
    }

    [ExposeWeb]
    public static TestEvents GetNewInstance() => new TestEvents();
}
```

When you expose an event using `[ExposeWeb]`, the library automatically generates helper methods on the JavaScript side to manage event subscriptions. Specifically, for the `TestEventString` event, the following javascript methods are created:

- **`add_TestEventString`**: Equivalent to the `+=` operator in C#. Use this method to subscribe a delegate (callback) to the event.
- **`remove_TestEventString`**: Equivalent to the `-=` operator in C#. Use this method to unsubscribe the delegate from the event.

Below is an example demonstrating this process:

```typescript
// Create a delegate for Action<string> using the extras API.
const myDelegate = unityInstance.Module.extras["System"]["Action<String>"].createDelegate(
  (message: System.String) => {
    console.log("Event received: ", message.value);
  }
);

//Get the instance of the C# class exposing the event.
const instance = unityInstance.Module.static["YourNamespace"].TestEvents.GetInstance();

// Subscribe to the event using the auto-generated add method.
instance.add_TestEventString(myDelegate);

// Trigger the event from C#.
instance.InvokeEvent("Hello from C#!");

// Later, to unsubscribe from the event, call the remove method:
instance.remove_TestEventString(myDelegate);
```

This approach allows you to seamlessly subscribe and unsubscribe to C# events from JavaScript, just as you would use `+=` and `-=` in C#. The delegate created in JavaScript acts as a bridge, ensuring that when the event is raised on the C# side, your callback is executed on the JS side.

### Asynchronous logic (Task / Promises)

Unity Easy Web Interop seamlessly integrates C# asynchronous methods with JavaScript by converting C# `Task` and `Task<T>` objects into JavaScript `Promise`. This integration lets you call asynchronous C# methods from JavaScript and handle the results using familiar async/await or Promise chaining patterns.

#### How It Works
- You can use `ExposeWeb` on `async methods`, on methods returning `Task`, or on `Task<T>`, or on a combination of the three

- **Task returning a value (`Task<T>`):**  
  The asynchronous method resolves to a Promise<T> that returns a managed value.

- **Task returning void (`Task`) or async void methods:**  
  Since there is no return value, the corresponding Promise resolves to `undefined`.

#### Example

Consider the following C# example:

```csharp
public class TestTasks
{
    // Task returning a string
    [ExposeWeb]
    public async Task<string> TestTaskString()
    {
        await Task.Yield();
        return "Hello from C#";
    }

    // Task returning nothing
    [ExposeWeb]
    public async Task TestTaskVoid()
    {
        await Task.Yield();
    }

    // Async method that returns void
    [ExposeWeb]
    public async void AsyncVoidMethod() => await Task.Yield();

    // Constructor
    [ExposeWeb]
    public static TestTasks GetInstance() => new TestTasks();
}
```

On the JavaScript side, you can invoke these methods and work with their results as follows:

```javascript
// Retrieve the instance of the C# class.
const instance = unityInstance.Module.static["YourNamespace"].TestTasks.GetInstance();

// Handling a Task that returns a value:
instance.TestTaskString().then(result => {
    // Access the actual string using the .value property.
    console.log(result.value); // Should output: "Hello from C#"
});

// Alternatively, using async/await:
async function callTestTaskString() {
    const result = await instance.TestTaskString();
    console.log(result.value);
}
callTestTaskString();

// You may also need to handle a Task that returns void:
instance.TestTaskVoid().then(voidResult => {
    console.log(voidResult); // Outputs: undefined
});

// Handling an async void method (similar to a Task returning void):
instance.AsyncVoidMethod().then(voidResult => {
    console.log(voidResult); // Outputs: undefined
});
```

This design allows you to write asynchronous C# code that can be easily managed and integrated within your JavaScript environment, leveraging the full power of Promises for smooth, non-blocking interoperation between Unity and web applications.

### Using get/set

You can apply the `[ExposeWeb]` attribute either directly to the property or individually on the getter and/or setter. Consider the following C# code, where two static properties are exposed:

```csharp
/// <summary>
/// Test exposing getters and setters.
/// </summary>
public class TestGetSets
{
    /// <summary>
    /// Exposes the getter and setter by applying [ExposeWeb] individually.
    /// </summary>
    public static string TestString { [ExposeWeb] get; [ExposeWeb] set; } = "Hello 12345";

    /// <summary>
    /// Exposes the property as a whole.
    /// </summary>
    [ExposeWeb]
    public static string TestString2 { get; set; } = "Hello 123456";
}
```

- **`TestString`**: The `[ExposeWeb]` attribute is applied to each accessor.  
- **`TestString2`**: The attribute is applied to the entire property.  

Both approaches result in generating two JavaScript methods per property:
- **Getter:** `get_PropertyName()`
- **Setter:** `set_PropertyName(managedValue)`

#### JavaScript Usage

On the JavaScript side, you can use these generated methods to interact with the C# properties. For example:

```javascript
// Access the TestGetSets class.
const testGetSets = unityInstance.Module.static["YourNamespace"].TestGetSets;

// Get the current value of TestString.
const currentTestString = testGetSets.get_TestString();
console.log("Current TestString:", currentTestString.value); // prints "Hello 12345"

// Create a new managed string.
const newManagedString = unityInstance.Module.utilities.GetManagedString("NewValue");

// Set a new value for TestString.
testGetSets.set_TestString(newManagedString);

// Get the updated value of TestString.
const updatedTestString = testGetSets.get_TestString();
console.log("Updated TestString:", updatedTestString.value); // prints "NewValue"

// Get the current value of TestString2.
const currentTestString2 = testGetSets.get_TestString2();
console.log("Current TestString2:", currentTestString2.value); // prints "Hello 123456"

// Set a new value for TestString2.
testGetSets.set_TestString2(newManagedString);

// Get the updated value of TestString2.
const updatedTestString2 = testGetSets.get_TestString2();
console.log("Updated TestString2:", updatedTestString2.value); // prints "NewValue"
```

###  Exception handling
#### Limitations with unity configuration 
- Documentation coming soon

### Handling lists & arrays
- Documentation coming soon

### Inheritance, abstract classes and interfaces
- Documentation coming soon

### Custom typescript generation
- Documentation coming soon

### Custom serialization 
- Documentation coming soon

### Performance considerations
#### Garbage collection
- Documentation coming soon

### Limitations
#### Cannot directly use constructors
- Documentation coming soon
#### Cannot use default interface implementations (C# feature)
- Documentation coming soon
#### Must use ExposeWeb on class AND interfaces, when class implement an interface
- Documentation coming soon

### TODO
- [Repo] CI & versions
- [Repo] Full documentation
- Added tests to ensure callbacks are correctly freed in the garbage collector. Confirm and fix any issues.
- Enhanceed support for arrays on the JavaScript side (e.g., direct call in TypeScript for GetElementAt(i)).
- Better support for enums
- Simplify the creation primitives types to allow easy access from almost anywhere, particularly for floats and strings.
- Find a way to generate missing TS types for immutable collections like IReadonlyCollection<Vector3> with examples such as IReadOnlyList<Vector3>.