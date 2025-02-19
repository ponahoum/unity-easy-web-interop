Release 1.0.5 - 18 feb 2025

- Fixed an issue in the code analyzer where inheritance was not properly taken into account, as it used the declared type instead of the reflected type. Reflected type is now correctly used since inheritance is enabled when retrieving attributes.  
- Removed all references to the "declaring type" when fetching the exposed web attribute, ensuring greater stability.  
- Improved default serialization, addressing culture settings, malformed strings, and adding unit tests.  
- Enabled the ability to run package unit tests in the development environment project.  
- Added protections against bad serialization on the JavaScript side.  
- Added getters for "value" and "managed" types in TypeScript generation to better reflect their true nature.  
- Upgraded to version 1.0.5 for release. 
