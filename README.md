# Unity Easy Web Interop

> **This repository is a work in progress. Please do not use it in production.**  
> Full documentation will be provided soon.

## TODO List

- Ensure callbacks are correctly freed in the garbage collector. Confirm and fix any issues.
- Improve TypeScript type generation (currently contains ugly code and lacks some types, especially for delegates).
- Organize all static types under their appropriate namespace.
- Add utility JavaScript methods to TypeScript.
- Enhance support for arrays on the JavaScript side (e.g., direct call in TypeScript for `GetElementAt(i)`).

## Installation

To install, use Unity's Package Manager. Add the following URL to your project dependencies:
```https://github.com/ponahoum/unity-easy-web-interop.git?path=/Packages/org.nahoum.easy-web-interop```
