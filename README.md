# Unity Easy Web Interop

> **This repository is a work in progress. Please do not use it in production.**  
> Full documentation will be provided soon.

## TODO List

- Ensure callbacks are correctly freed in the garbage collector. Confirm and fix any issues.
- Improve TypeScript type generation (currently contains messy code and lacks some types, especially for delegates).
- Add utility JavaScript methods to TypeScript.
- Enhance support for arrays on the JavaScript side (e.g., direct call in TypeScript for GetElementAt(i)).
- Easy generation of enums
- Ability to create an enum easily and accessibly with createFromString.
- Simplify the creation of strings, arrays, etc., to allow easy access from almost anywhere, particularly for floats and strings.
- Generate missing types for immutable collections like IReadonlyCollection<Vector3> with examples such as IReadonlyList<Vector3>.
- Automatically create constructors for generating strongly-typed actions composed of complex types.
Expose utility methods separately in internal code for creating objects like Vector3, Quaternion, Color, etc.

## Installation

To install, use Unity's Package Manager. Add the following URL to your project dependencies:
```https://github.com/ponahoum/unity-easy-web-interop.git?path=/Packages/org.nahoum.easy-web-interop```
