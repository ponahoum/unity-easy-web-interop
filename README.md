# Unity Easy WebGL & Javascript Interop

**Unity Easy Web Interop** is a library designed targetting Unity WebGL users that allows to expose **C# methods and class instances from Unity to JavaScript** using simple C# decorators. It also generates easy-to-use, strongly typed signatures for TypeScript.

This library is built on a direct binding to the WASM side of Unity WebGL and does not rely on messaging or serialization systems (such as Unity’s `SendMessage`). 

It’s perfect for developing Unity applications that require a modern web UI overlay built with frameworks like React, Vue, or Angular.

## Basic Example

Consider the following C# code:

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

By applying the `[ExposeWeb]` attribute to these methods, they become accessible in your JavaScript WebGL build. Static methods are accessed via the path `static[namespace][className][methodName]`. Static methods allow you to expose instances, meaning both static and instance methods are available to JavaScript.

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

## How to use

### Accessing Primitive Types

When building interop between two languages, everything starts with handling primitive types. You may need to convert JavaScript data—such as strings, arrays of strings, numbers, etc.—into managed types that can be passed as arguments to your exposed web methods. For most of these types, this package provides utility constructors accessible via the `unityInstance.Module.utilities` path.

#### Examples

Here are some examples demonstrating how to create managed types:

```javascript
// Convert a JavaScript string to a string on the C# side (string)
const aString = unityInstance.Module.utilities.GetManagedString("hello world");

// Convert a JavaScript array of strings to a string array on the C# side (string[])
const aStringArray = unityInstance.Module.utilities.GetManagedStringArray(["hello", "world"]);

// Convert a JavaScript number to a float the C# side (float)
const aFloat = unityInstance.Module.utilities.GetManagedFloat(1234);

// Convert a JavaScript array of numbers to a float array on the C# side (float[])
const aFloatArray = unityInstance.Module.utilities.GetManagedFloatArray([1, 2, 3, 4]);

// Efficiently pass binary data using a Uint8Array to the C# side (byte[])
const aByteArray = unityInstance.Module.utilities.GetManagedByteArray(new Uint8Array([/* your binary data */]));
```

For the best development experience, generate the corresponding TypeScript definitions to review the utility signatures and ensure type safety.

---

### Using callbacks (Action, event)
- Documentation coming soon

### Asynchronous logic (Task vs Promises)
- Documentation coming soon

###  Exception handling
#### Limitations with unity configuration 
- Documentation coming soon

### Handling lists & arrays
- Documentation coming soon

### Using get/set on properties
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