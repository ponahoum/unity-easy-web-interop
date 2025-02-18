export namespace AnotherNamespace {
  export type SomeClass = {
    fullTypeName_71BF8987A338C4DC4B658B027493459F2251552D58D81FDE3553739A2D21AA29: "AnotherNamespace.SomeClass";
    assembly_71BF8987A338C4DC4B658B027493459F2251552D58D81FDE3553739A2D21AA29: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    TestGetSimpleValue(): System.Int32;
  };
}
export namespace System {
  export type Int32 = {
    fullTypeName_1A4C5B3F3707B26139928EEC75D4FAF76764837AD07C75D4E6A90E8A01EF7889: "System.Int32";
    assembly_1A4C5B3F3707B26139928EEC75D4FAF76764837AD07C75D4E6A90E8A01EF7889: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number;
    get managedType(): Type;
  };

  export type String = {
    fullTypeName_7BE15EEA31A4F99C8B95EC61E38A8FD3E5D739B015836086C4AADACE3B2F8D51: "System.String";
    assembly_7BE15EEA31A4F99C8B95EC61E38A8FD3E5D739B015836086C4AADACE3B2F8D51: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): string | null;
    get managedType(): Type;
  } & System.Collections.IEnumerable;

  export type Action = {
    fullTypeName_F4B6DD8B3E8E06AE6753665C6F85AEC59D351F83B0D4E77775E0C0A3ECECD3B2: "System.Action";
    assembly_F4B6DD8B3E8E06AE6753665C6F85AEC59D351F83B0D4E77775E0C0A3ECECD3B2: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): Type;
  };

  export type Action1$Int32 = {
    fullTypeName_CA470E7D9BF41124EB70B26CA12E1D848B34F9CCE89CA3D20B8D87DE9425B0F5: "System.Action`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
    assembly_CA470E7D9BF41124EB70B26CA12E1D848B34F9CCE89CA3D20B8D87DE9425B0F5: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): Type;
  };

  export type Action1$String = {
    fullTypeName_31480652899900BFEEC0D7ABA8CB669E6FCEC6D81BB5582D51F85B36E92881F6: "System.Action`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
    assembly_31480652899900BFEEC0D7ABA8CB669E6FCEC6D81BB5582D51F85B36E92881F6: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): Type;
  };

  export type Single = {
    fullTypeName_E730956F5BB0C4808E8103222463441E929747F209558E35B4BBEC499BB5BF87: "System.Single";
    assembly_E730956F5BB0C4808E8103222463441E929747F209558E35B4BBEC499BB5BF87: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number;
    get managedType(): Type;
  };

  export type Action1$Single = {
    fullTypeName_B2AF6431FBE2BB67C507907708A697C2C2BC8F1AC6C063A7BB1460B704638149: "System.Action`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
    assembly_B2AF6431FBE2BB67C507907708A697C2C2BC8F1AC6C063A7BB1460B704638149: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): Type;
  };

  export type Double = {
    fullTypeName_1783640C653535228BACC4755D3982C1554DE9A4E804D673862B95330D8B0E2D: "System.Double";
    assembly_1783640C653535228BACC4755D3982C1554DE9A4E804D673862B95330D8B0E2D: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number;
    get managedType(): Type;
  };

  export type Action2$Double$String = {
    fullTypeName_D346F5D34218E398F683F7AB20318E40359711A581143736E8284B150FA567A7: "System.Action`2[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
    assembly_D346F5D34218E398F683F7AB20318E40359711A581143736E8284B150FA567A7: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): Type;
  };

  export type Action1$AnotherNamespace_SomeClass = {
    fullTypeName_84F7FB51B17C143ECBC426B5A47328798B9CA5609EFBCC31EDDD562B942C6CFD: "System.Action`1[[AnotherNamespace.SomeClass, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]";
    assembly_84F7FB51B17C143ECBC426B5A47328798B9CA5609EFBCC31EDDD562B942C6CFD: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): Type;
  };

  export type Byte_CSharpArray = {
    fullTypeName_F731B51747BB7D72E94AA746D30E597ECDB9B4B7100B3F57BE27A03AE8EB5623: "System.Byte[]";
    assembly_F731B51747BB7D72E94AA746D30E597ECDB9B4B7100B3F57BE27A03AE8EB5623: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number[];
    get managedType(): Type;
  } & System.Collections.ICollection &
    System.Collections.IEnumerable &
    System.Collections.IList;

  export type Boolean = {
    fullTypeName_5A0C2631BF09F7B05CFEC50008C8A444FFB8400D35CE416697905239202C2C8C: "System.Boolean";
    assembly_5A0C2631BF09F7B05CFEC50008C8A444FFB8400D35CE416697905239202C2C8C: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): boolean;
    get managedType(): Type;
  };

  export type Single_CSharpArray = {
    fullTypeName_8EB6F50137B9346FFBF2C9C331BF1FC521986B895BFB3FC10A7FA6E26843F68C: "System.Single[]";
    assembly_8EB6F50137B9346FFBF2C9C331BF1FC521986B895BFB3FC10A7FA6E26843F68C: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number[];
    get managedType(): Type;
  } & System.Collections.ICollection &
    System.Collections.IEnumerable &
    System.Collections.IList;

  export type Double_CSharpArray = {
    fullTypeName_C09C8EEEFE5FFEAC2A25267D447D9167739A879C1B7B3863A64F523080EECA4D: "System.Double[]";
    assembly_C09C8EEEFE5FFEAC2A25267D447D9167739A879C1B7B3863A64F523080EECA4D: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number[];
    get managedType(): Type;
  } & System.Collections.ICollection &
    System.Collections.IEnumerable &
    System.Collections.IList;

  export type Byte = {
    fullTypeName_4AD3B2A48D1EF7EACF91204DF53E60BAD740058CADA1243650CA2D2443ED2BDA: "System.Byte";
    assembly_4AD3B2A48D1EF7EACF91204DF53E60BAD740058CADA1243650CA2D2443ED2BDA: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number;
    get managedType(): Type;
  };

  export type Int64 = {
    fullTypeName_0AD0717715C52D6D5810FA9E519C27301C17E161102E8AB16EA6146D1DC83490: "System.Int64";
    assembly_0AD0717715C52D6D5810FA9E519C27301C17E161102E8AB16EA6146D1DC83490: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number;
    get managedType(): Type;
  };

  export type Type = {
    fullTypeName_28EFCA72142AE647F1622B9BBF17C18C9D1AFC67EFD6489E0BA5781715E8F224: "System.Type";
    assembly_28EFCA72142AE647F1622B9BBF17C18C9D1AFC67EFD6489E0BA5781715E8F224: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): string;
    get managedType(): Type;
  };

  export type String_CSharpArray = {
    fullTypeName_D9B03C5908E5F69C8D15F26A6595B198EE1438F2401575D0E9FFF1F32CE6A387: "System.String[]";
    assembly_D9B03C5908E5F69C8D15F26A6595B198EE1438F2401575D0E9FFF1F32CE6A387: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): (string | null)[];
    get managedType(): Type;
  } & System.Collections.ICollection &
    System.Collections.IEnumerable &
    System.Collections.IList;

  export type Int32_CSharpArray = {
    fullTypeName_3D2652DD050837302D168A719C23EEF8DB0057C12981BAC52677167754DF7A96: "System.Int32[]";
    assembly_3D2652DD050837302D168A719C23EEF8DB0057C12981BAC52677167754DF7A96: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number[];
    get managedType(): Type;
  } & System.Collections.ICollection &
    System.Collections.IEnumerable &
    System.Collections.IList;

  export type Boolean_CSharpArray = {
    fullTypeName_A803FF90FCEFA9EF3CB901099863B9BEF8412FD0663C498142316FE9EE2C89C4: "System.Boolean[]";
    assembly_A803FF90FCEFA9EF3CB901099863B9BEF8412FD0663C498142316FE9EE2C89C4: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): boolean[];
    get managedType(): Type;
  } & System.Collections.ICollection &
    System.Collections.IEnumerable &
    System.Collections.IList;
}
export namespace Nahoum.UnityJSInterop.Tests {
  export type TestAbstractClass_static = {
    fullTypeName_FCC6C7871F4910A296CB2448940DA8F5D3A2F074814852111931019D88489038: "Nahoum.UnityJSInterop.Tests.TestAbstractClass";
    assembly_FCC6C7871F4910A296CB2448940DA8F5D3A2F074814852111931019D88489038: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetInstance(): TestAbstractClassAbstract;
  };

  export type TestAbstractClass = {
    fullTypeName_FCC6C7871F4910A296CB2448940DA8F5D3A2F074814852111931019D88489038: "Nahoum.UnityJSInterop.Tests.TestAbstractClass";
    assembly_FCC6C7871F4910A296CB2448940DA8F5D3A2F074814852111931019D88489038: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    GetString(): System.String;
  } & TestAbstractClassAbstract &
    TestAbstractClass_static;

  export type TestAbstractClassAbstract = {
    fullTypeName_1C60D0CCA05C43674C8824D1D531C9E98F560142F77B1FC79C47BCC1D8E55603: "Nahoum.UnityJSInterop.Tests.TestAbstractClassAbstract";
    assembly_1C60D0CCA05C43674C8824D1D531C9E98F560142F77B1FC79C47BCC1D8E55603: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    GetString(): System.String;
  };

  export type TestActionCallbacks_static = {
    fullTypeName_4D003D21C580B46619180C8A29C1F73A04C37A5C57AB58EBAD96EBEA6BA4639E: "Nahoum.UnityJSInterop.Tests.TestActionCallbacks";
    assembly_4D003D21C580B46619180C8A29C1F73A04C37A5C57AB58EBAD96EBEA6BA4639E: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetInstance(): TestActionCallbacks;
  };

  export type TestActionCallbacks = {
    fullTypeName_4D003D21C580B46619180C8A29C1F73A04C37A5C57AB58EBAD96EBEA6BA4639E: "Nahoum.UnityJSInterop.Tests.TestActionCallbacks";
    assembly_4D003D21C580B46619180C8A29C1F73A04C37A5C57AB58EBAD96EBEA6BA4639E: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    TestInvokeCallbackVoid(action: System.Action): void;
    TestInvokeCallbackInt(action: System.Action1$Int32): void;
    TestInvokeCallbackString(action: System.Action1$String): void;
    TestInvokeCallbackFloat(action: System.Action1$Single): void;
    TestInvokeDoubleStringCallback(action: System.Action2$Double$String): void;
    TestInvokeActionWithClassOutsideNamespace(
      action: System.Action1$AnotherNamespace_SomeClass
    ): void;
  } & TestActionCallbacks_static;

  export type TestTasks_static = {
    fullTypeName_B839AB15D3AC2BC63D5BBB5835FE5F73E10D073E978155746C413FBCFC136A56: "Nahoum.UnityJSInterop.Tests.TestTasks";
    assembly_B839AB15D3AC2BC63D5BBB5835FE5F73E10D073E978155746C413FBCFC136A56: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetInstance(): TestTasks;
  };

  export type TestTasks = {
    fullTypeName_B839AB15D3AC2BC63D5BBB5835FE5F73E10D073E978155746C413FBCFC136A56: "Nahoum.UnityJSInterop.Tests.TestTasks";
    assembly_B839AB15D3AC2BC63D5BBB5835FE5F73E10D073E978155746C413FBCFC136A56: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    TestTaskVoid(): Promise<void>;
    TestTaskString(): Promise<System.String>;
    TestTaskInt(): Promise<System.Int32>;
    TestTaskFloat(): Promise<System.Single>;
    AsyncVoidMethod(): void;
  } & TestTasks_static;

  export type ITestInterfaceForBaseClass = {
    fullTypeName_F751B6F1E1257F5533C1AEB8AEC0D56A41325E5FD5350331F2A0B484947E8E71: "Nahoum.UnityJSInterop.Tests.ITestInterfaceForBaseClass";
    assembly_F751B6F1E1257F5533C1AEB8AEC0D56A41325E5FD5350331F2A0B484947E8E71: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    GetString(): System.String;
  };

  export type TestBasicInheritance_static = {
    fullTypeName_721458168E68304EEF3617CB2803169ED2A26D2EEE7BD3C9B9EC730198DBE80D: "Nahoum.UnityJSInterop.Tests.TestBasicInheritance";
    assembly_721458168E68304EEF3617CB2803169ED2A26D2EEE7BD3C9B9EC730198DBE80D: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetInstance(): TestBasicInheritance;
  };

  export type TestBasicInheritance = {
    fullTypeName_721458168E68304EEF3617CB2803169ED2A26D2EEE7BD3C9B9EC730198DBE80D: "Nahoum.UnityJSInterop.Tests.TestBasicInheritance";
    assembly_721458168E68304EEF3617CB2803169ED2A26D2EEE7BD3C9B9EC730198DBE80D: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    GetString(): System.String;
  } & ITestInterfaceForBaseClass &
    TestBasicInheritance_static;

  export type SuperTestBasicInheritance_static = {
    fullTypeName_A9176F0B48ED7931F97A5C4A4069D0B6380CE23E3EC6B175624ADF9E8FA2B5F1: "Nahoum.UnityJSInterop.Tests.SuperTestBasicInheritance";
    assembly_A9176F0B48ED7931F97A5C4A4069D0B6380CE23E3EC6B175624ADF9E8FA2B5F1: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetInstanceSuper(): SuperTestBasicInheritance;
  };

  export type SuperTestBasicInheritance = {
    fullTypeName_A9176F0B48ED7931F97A5C4A4069D0B6380CE23E3EC6B175624ADF9E8FA2B5F1: "Nahoum.UnityJSInterop.Tests.SuperTestBasicInheritance";
    assembly_A9176F0B48ED7931F97A5C4A4069D0B6380CE23E3EC6B175624ADF9E8FA2B5F1: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    GetAsBaseClass(): TestBasicInheritance;
    GetString(): System.String;
  } & ITestInterfaceForBaseClass &
    TestBasicInheritance &
    SuperTestBasicInheritance_static;

  export type TestByteArray_static = {
    fullTypeName_561ACD64EBC54DCB272D409F44805D8661504025B58A06F50C0D733DF68E0575: "Nahoum.UnityJSInterop.Tests.TestByteArray";
    assembly_561ACD64EBC54DCB272D409F44805D8661504025B58A06F50C0D733DF68E0575: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    LoadTexture(imageBytes: System.Byte_CSharpArray): UnityEngine.Texture2D;
    GetTextureResolution(tex: UnityEngine.Texture2D): System.String;
    DestroyTexture(tex: UnityEngine.Texture2D): void;
    GetByteArrayLength(byteArray: System.Byte_CSharpArray): System.Int32;
  };

  export type TestEnums_static = {
    fullTypeName_9F3457E50A35122DA915D46D16E1DEA0F4D05733A576C36FAB6C475866491943: "Nahoum.UnityJSInterop.Tests.TestEnums";
    assembly_9F3457E50A35122DA915D46D16E1DEA0F4D05733A576C36FAB6C475866491943: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetInstance(): TestEnums;
  };

  export type TestEnums = {
    fullTypeName_9F3457E50A35122DA915D46D16E1DEA0F4D05733A576C36FAB6C475866491943: "Nahoum.UnityJSInterop.Tests.TestEnums";
    assembly_9F3457E50A35122DA915D46D16E1DEA0F4D05733A576C36FAB6C475866491943: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    GetEnumFirstValue(): MyEnum;
    GetEnumSecondValue(): MyEnum;
    GetEnumThirdValue(): MyEnum;
    GetEnumValueAsString(value: MyEnum): System.String;
  } & TestEnums_static;

  export type MyEnum = {
    fullTypeName_CDA0929EB8DDEA02F7C8D606EBAFC0979C7BBFF1563B2D2BC1D7CE74F147AD0B: "Nahoum.UnityJSInterop.Tests.MyEnum";
    assembly_CDA0929EB8DDEA02F7C8D606EBAFC0979C7BBFF1563B2D2BC1D7CE74F147AD0B: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): "Red" | "Green" | "Blue";
    get managedType(): System.Type;
  };

  export type TestEvents_static = {
    fullTypeName_D6FD91B2082087F081F0F75528CD3D58372B985F8C2F5DD779BFEEBA93DFB91D: "Nahoum.UnityJSInterop.Tests.TestEvents";
    assembly_D6FD91B2082087F081F0F75528CD3D58372B985F8C2F5DD779BFEEBA93DFB91D: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetNewInstance(): TestEvents;
  };

  export type TestEvents = {
    fullTypeName_D6FD91B2082087F081F0F75528CD3D58372B985F8C2F5DD779BFEEBA93DFB91D: "Nahoum.UnityJSInterop.Tests.TestEvents";
    assembly_D6FD91B2082087F081F0F75528CD3D58372B985F8C2F5DD779BFEEBA93DFB91D: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    add_TestEventStringAuto(value: System.Action1$String): void;
    remove_TestEventStringAuto(value: System.Action1$String): void;
    add_TestEventStringManual(value: System.Action1$String): void;
    remove_TestEventStringManual(value: System.Action1$String): void;
    InvokeEvent(value: System.String): void;
  } & TestEvents_static;

  export type TestExceptions_static = {
    fullTypeName_A1735008FA7264DEB323796B88783003C9089F88341F8417CCAC871F2B32E67A: "Nahoum.UnityJSInterop.Tests.TestExceptions";
    assembly_A1735008FA7264DEB323796B88783003C9089F88341F8417CCAC871F2B32E67A: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    ThrowSimpleException(): void;
    TestUnraisedException(): void;
    ThrowSimpleExceptionAsync(): Promise<void>;
    TestUnraisedExceptionAsync(): Promise<void>;
  };

  export type TestExceptions = {
    fullTypeName_A1735008FA7264DEB323796B88783003C9089F88341F8417CCAC871F2B32E67A: "Nahoum.UnityJSInterop.Tests.TestExceptions";
    assembly_A1735008FA7264DEB323796B88783003C9089F88341F8417CCAC871F2B32E67A: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
  } & TestExceptions_static;

  export type TestGenericClass_static = {
    fullTypeName_6E09F0E1ED8AEF700828861257EEC58317E53F244F82336E0C9B6136B706F0DB: "Nahoum.UnityJSInterop.Tests.TestGenericClass";
    assembly_6E09F0E1ED8AEF700828861257EEC58317E53F244F82336E0C9B6136B706F0DB: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    CreateInstance(): TestGenericClass;
  };

  export type TestGenericClass = {
    fullTypeName_6E09F0E1ED8AEF700828861257EEC58317E53F244F82336E0C9B6136B706F0DB: "Nahoum.UnityJSInterop.Tests.TestGenericClass";
    assembly_6E09F0E1ED8AEF700828861257EEC58317E53F244F82336E0C9B6136B706F0DB: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    GetTypeName(): System.String;
    GetTypeNameFromExternal(input: ITestInterfaceInGenericClass): System.String;
  } & ITestInterfaceInGenericClass &
    TestGenericClass_static;

  export type ITestInterfaceInGenericClass = {
    fullTypeName_017E506FD4692089C84D6F8110B02773E001FC2ED139B13AAC583CA934A09A03: "Nahoum.UnityJSInterop.Tests.ITestInterfaceInGenericClass";
    assembly_017E506FD4692089C84D6F8110B02773E001FC2ED139B13AAC583CA934A09A03: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
  };

  export type TestAnotherGenericClass_static = {
    fullTypeName_22BF149C1205E39FC3FE1B2F950C8032D0A2684BE4B385E86C77569737862566: "Nahoum.UnityJSInterop.Tests.TestAnotherGenericClass";
    assembly_22BF149C1205E39FC3FE1B2F950C8032D0A2684BE4B385E86C77569737862566: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    CreateInstance(): TestAnotherGenericClass;
  };

  export type TestAnotherGenericClass = {
    fullTypeName_22BF149C1205E39FC3FE1B2F950C8032D0A2684BE4B385E86C77569737862566: "Nahoum.UnityJSInterop.Tests.TestAnotherGenericClass";
    assembly_22BF149C1205E39FC3FE1B2F950C8032D0A2684BE4B385E86C77569737862566: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    GetTypeName(): System.String;
    GetTypeNameFromExternal(input: ITestInterfaceInGenericClass): System.String;
  } & ITestInterfaceInGenericClass &
    TestAnotherGenericClass_static;

  export type TestGetBasicValues_static = {
    fullTypeName_41B2D7FC3E0E1CD9D3982F854A7792947F77A00BBC4CAC5B43AF6FED949B1D9B: "Nahoum.UnityJSInterop.Tests.TestGetBasicValues";
    assembly_41B2D7FC3E0E1CD9D3982F854A7792947F77A00BBC4CAC5B43AF6FED949B1D9B: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetTestString(): System.String;
    GetTestDouble(): System.Double;
    GetTestInt(): System.Int32;
    GetTestBoolTrue(): System.Boolean;
    GetTestBoolFalse(): System.Boolean;
    GetNullString(): System.String;
  };

  export type TestGetBasicValues = {
    fullTypeName_41B2D7FC3E0E1CD9D3982F854A7792947F77A00BBC4CAC5B43AF6FED949B1D9B: "Nahoum.UnityJSInterop.Tests.TestGetBasicValues";
    assembly_41B2D7FC3E0E1CD9D3982F854A7792947F77A00BBC4CAC5B43AF6FED949B1D9B: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
  } & TestGetBasicValues_static;

  export type TestGetSets_static = {
    fullTypeName_7336151FCAB055BA4E5A69E66EC6819228C9A998D94D10806CA867706B678675: "Nahoum.UnityJSInterop.Tests.TestGetSets";
    assembly_7336151FCAB055BA4E5A69E66EC6819228C9A998D94D10806CA867706B678675: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get_TestString(): System.String;
    set_TestString(value: System.String): void;
    get_TestString2(): System.String;
    set_TestString2(value: System.String): void;
  };

  export type TestGetSets = {
    fullTypeName_7336151FCAB055BA4E5A69E66EC6819228C9A998D94D10806CA867706B678675: "Nahoum.UnityJSInterop.Tests.TestGetSets";
    assembly_7336151FCAB055BA4E5A69E66EC6819228C9A998D94D10806CA867706B678675: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
  } & TestGetSets_static;

  export type TestInstanceMethods_static = {
    fullTypeName_47220AC5AD3CDDE831DF1421B56F1E94FBA0AC907ADE78FE9D9D42697D6BB63F: "Nahoum.UnityJSInterop.Tests.TestInstanceMethods";
    assembly_47220AC5AD3CDDE831DF1421B56F1E94FBA0AC907ADE78FE9D9D42697D6BB63F: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetNewInstance(): TestInstanceMethods;
  };

  export type TestInstanceMethods = {
    fullTypeName_47220AC5AD3CDDE831DF1421B56F1E94FBA0AC907ADE78FE9D9D42697D6BB63F: "Nahoum.UnityJSInterop.Tests.TestInstanceMethods";
    assembly_47220AC5AD3CDDE831DF1421B56F1E94FBA0AC907ADE78FE9D9D42697D6BB63F: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    TestGetString(): System.String;
    TestGetDouble(): System.Double;
    TestGetInt(): System.Int32;
    TestGetFloat(): System.Single;
    TestGetFloatArray(): System.Single_CSharpArray;
    TestGetFloatList(): System.Collections.Generic.List1$System_Single;
    TestGetDoubleArray(): System.Double_CSharpArray;
    TestGetBoolTrue(): System.Boolean;
    TestGetBoolFalse(): System.Boolean;
    TestGetNullString(): System.String;
    TestAdditionInt(a: System.Int32, b: System.Int32): System.Int32;
    TestAdditionFloat(a: System.Single, b: System.Single): System.Single;
    TestAdditionDouble(a: System.Double, b: System.Double): System.Double;
    TestAdditionString(a: System.String, b: System.String): System.String;
    TestGetVector2(): UnityEngine.Vector2;
    TestGetVector3(): UnityEngine.Vector3;
    TestGetVector4(): UnityEngine.Vector4;
    TestGetQuaternion(): UnityEngine.Quaternion;
    TestGetColor(): UnityEngine.Color;
    TestGetColor32(): UnityEngine.Color32;
  } & TestInstanceMethods_static;

  export type TestClassImplementingInterface_static = {
    fullTypeName_54CCDDBE4FD6F8CC590C262925B21D9FBD4468B989068554887F8E271314B25B: "Nahoum.UnityJSInterop.Tests.TestClassImplementingInterface";
    assembly_54CCDDBE4FD6F8CC590C262925B21D9FBD4468B989068554887F8E271314B25B: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetNewInstanceOfInterface(): ITestInterface;
  };

  export type TestClassImplementingInterface = {
    fullTypeName_54CCDDBE4FD6F8CC590C262925B21D9FBD4468B989068554887F8E271314B25B: "Nahoum.UnityJSInterop.Tests.TestClassImplementingInterface";
    assembly_54CCDDBE4FD6F8CC590C262925B21D9FBD4468B989068554887F8E271314B25B: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    TestGetStringFromClass(): System.String;
    TestGetStringFromInterfaceDeclaration(): System.String;
    GetSampleFloat(): System.Single;
    TestGetStringFromInterface(): System.String;
  } & ITestInterface &
    TestClassImplementingInterface_static;

  export type ITestInterface_static = {
    fullTypeName_7785F7D66212B156A32F754BA29C35A64B0817A5F66FB5A2B7A11B846354BAFB: "Nahoum.UnityJSInterop.Tests.ITestInterface";
    assembly_7785F7D66212B156A32F754BA29C35A64B0817A5F66FB5A2B7A11B846354BAFB: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    TestGetStringFromInterfaceStatic(): System.String;
  };

  export type ITestInterface = {
    fullTypeName_7785F7D66212B156A32F754BA29C35A64B0817A5F66FB5A2B7A11B846354BAFB: "Nahoum.UnityJSInterop.Tests.ITestInterface";
    assembly_7785F7D66212B156A32F754BA29C35A64B0817A5F66FB5A2B7A11B846354BAFB: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
    GetSampleFloat(): System.Single;
    TestGetStringFromInterface(): System.String;
    TestGetStringFromInterfaceDeclaration(): System.String;
  } & ITestInterface_static;

  export type TestStructExpose_static = {
    fullTypeName_FA34B0BB0A662BAF058B325D67DA35C3D17329E0BA12250B83910B480F793691: "Nahoum.UnityJSInterop.Tests.TestStructExpose";
    assembly_FA34B0BB0A662BAF058B325D67DA35C3D17329E0BA12250B83910B480F793691: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    GetExposeInstance(): TestStructExpose;
  };

  export type TestStructExpose = {
    fullTypeName_FA34B0BB0A662BAF058B325D67DA35C3D17329E0BA12250B83910B480F793691: "Nahoum.UnityJSInterop.Tests.TestStructExpose";
    assembly_FA34B0BB0A662BAF058B325D67DA35C3D17329E0BA12250B83910B480F793691: "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): { A: number; F: number; Name: string };
    get managedType(): System.Type;
  } & TestStructExpose_static;
}
export namespace UnityEngine {
  export type Texture2D = {
    fullTypeName_7762A4A3A4088C5B99D2DB41578DD6E726D5FBB727C9C4E41524E08C1EA7B25A: "UnityEngine.Texture2D";
    assembly_7762A4A3A4088C5B99D2DB41578DD6E726D5FBB727C9C4E41524E08C1EA7B25A: "UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): unknown;
    get managedType(): System.Type;
  };

  export type Vector2 = {
    fullTypeName_1073C1923544E4B1897AA86D17D73DD78E72B7F113049DE6ED42359C5814F398: "UnityEngine.Vector2";
    assembly_1073C1923544E4B1897AA86D17D73DD78E72B7F113049DE6ED42359C5814F398: "UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): { x: number; y: number };
    get managedType(): System.Type;
  };

  export type Vector3 = {
    fullTypeName_7C3FFFF188C53DAE3CC5B9A843621336618668A20350FFF8F26659556D0B1C8A: "UnityEngine.Vector3";
    assembly_7C3FFFF188C53DAE3CC5B9A843621336618668A20350FFF8F26659556D0B1C8A: "UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): { x: number; y: number; z: number };
    get managedType(): System.Type;
  };

  export type Vector4 = {
    fullTypeName_1CF0FAC87540967A4A340914D5AC49E982F15541B0AFDC965A6FE76A36FB31F9: "UnityEngine.Vector4";
    assembly_1CF0FAC87540967A4A340914D5AC49E982F15541B0AFDC965A6FE76A36FB31F9: "UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): { x: number; y: number; z: number; w: number };
    get managedType(): System.Type;
  };

  export type Quaternion = {
    fullTypeName_30498154C951D387EEFC678967EB600BD4E738C8379AA9B0FD2D7D9405E3E911: "UnityEngine.Quaternion";
    assembly_30498154C951D387EEFC678967EB600BD4E738C8379AA9B0FD2D7D9405E3E911: "UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): { x: number; y: number; z: number; w: number };
    get managedType(): System.Type;
  };

  export type Color = {
    fullTypeName_219494579F6DC7815DE9F25C2BB8044EB8C76EBDBF9CB7A60D79E0AE4F2491AA: "UnityEngine.Color";
    assembly_219494579F6DC7815DE9F25C2BB8044EB8C76EBDBF9CB7A60D79E0AE4F2491AA: "UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): { r: number; g: number; b: number; a: number };
    get managedType(): System.Type;
  };

  export type Color32 = {
    fullTypeName_30CFDD6A0B0068CB896367EFCCC6BE2653FC5F8DBD7F65EA84C1815847E9A7DE: "UnityEngine.Color32";
    assembly_30CFDD6A0B0068CB896367EFCCC6BE2653FC5F8DBD7F65EA84C1815847E9A7DE: "UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    get value(): { r: number; g: number; b: number; a: number };
    get managedType(): System.Type;
  };
}
export namespace System.Collections.Generic {
  export type List1$System_Single = {
    fullTypeName_6E9D7A8B0275D90AAEDF253871594BF8033BFC62D5BEBF3726248FF1C5D256A4: "System.Collections.Generic.List`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
    assembly_6E9D7A8B0275D90AAEDF253871594BF8033BFC62D5BEBF3726248FF1C5D256A4: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): number[];
    get managedType(): System.Type;
  } & System.Collections.ICollection &
    System.Collections.IEnumerable &
    System.Collections.IList;
}
export namespace System.Collections {
  export type IList = {
    fullTypeName_8DCFD2C8BA207E862FBCECF81423971CD71682BD2E99E84D6479BFFC596D749A: "System.Collections.IList";
    assembly_8DCFD2C8BA207E862FBCECF81423971CD71682BD2E99E84D6479BFFC596D749A: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): System.Type;
  } & ICollection &
    IEnumerable;

  export type ICollection = {
    fullTypeName_711A53EF0F58166FF23D8CA3FCA55B585DBC5EF5403BFD2A1B972CBC8CC6613A: "System.Collections.ICollection";
    assembly_711A53EF0F58166FF23D8CA3FCA55B585DBC5EF5403BFD2A1B972CBC8CC6613A: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): System.Type;
  } & IEnumerable;

  export type IEnumerable = {
    fullTypeName_AC4D3005F7591CD3E71CFFA64FE3408BA36352369B8CF6F49599272E4B17EAA9: "System.Collections.IEnumerable";
    assembly_AC4D3005F7591CD3E71CFFA64FE3408BA36352369B8CF6F49599272E4B17EAA9: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): System.Type;
  };

  export type IEnumerator = {
    fullTypeName_6D788DFC0E92CEE235FDCE483C1E40EB7F29C72AF3D6CE2F2C5A6D32926E6EE2: "System.Collections.IEnumerator";
    assembly_6D788DFC0E92CEE235FDCE483C1E40EB7F29C72AF3D6CE2F2C5A6D32926E6EE2: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    get value(): unknown;
    get managedType(): System.Type;
  };
}
// The following is generated from the main template
// Contains all hardcoded utility types and functions for the generated TypeScript file to work correctly

export type Utilities = {
  GetManagedAction<T extends any[]>(
    callback: (...actionParameters: T) => void,
    managedTypesArray: { [K in keyof T]: string }
  ): any;
  GetManagedAction(callback: () => void): System.Action;
  GetManagedBool(targetBool: boolean): System.Boolean;
  GetManagedBoolArray(array: boolean[]): System.Boolean_CSharpArray;
  GetManagedByteArray(array: Uint8Array): System.Byte_CSharpArray;
  GetManagedDouble(targetNumber: number): System.Double;
  GetManagedDoubleArray(array: number[]): System.Double_CSharpArray;
  GetManagedFloat(targetNumber: number): System.Single;
  GetManagedFloatArray(array: number[]): System.Single_CSharpArray;
  GetManagedInt(targetNumber: number): System.Int32;
  GetManagedIntArray(array: number[]): System.Int32_CSharpArray;
  GetManagedLong(targetNumber: number): System.Int64;
  GetManagedString(jsStr: string): System.String;
  GetManagedStringArray(array: string[]): System.String_CSharpArray;
};

export type UnityInstance = {
  Module: {
    static: {
      "Nahoum.UnityJSInterop.Tests": {
        TestAbstractClass: Nahoum.UnityJSInterop.Tests.TestAbstractClass_static;
        TestActionCallbacks: Nahoum.UnityJSInterop.Tests.TestActionCallbacks_static;
        TestTasks: Nahoum.UnityJSInterop.Tests.TestTasks_static;
        TestBasicInheritance: Nahoum.UnityJSInterop.Tests.TestBasicInheritance_static;
        SuperTestBasicInheritance: Nahoum.UnityJSInterop.Tests.SuperTestBasicInheritance_static;
        TestByteArray: Nahoum.UnityJSInterop.Tests.TestByteArray_static;
        TestEnums: Nahoum.UnityJSInterop.Tests.TestEnums_static;
        TestEvents: Nahoum.UnityJSInterop.Tests.TestEvents_static;
        TestExceptions: Nahoum.UnityJSInterop.Tests.TestExceptions_static;
        TestGenericClass: Nahoum.UnityJSInterop.Tests.TestGenericClass_static;
        TestAnotherGenericClass: Nahoum.UnityJSInterop.Tests.TestAnotherGenericClass_static;
        TestGetBasicValues: Nahoum.UnityJSInterop.Tests.TestGetBasicValues_static;
        TestGetSets: Nahoum.UnityJSInterop.Tests.TestGetSets_static;
        TestInstanceMethods: Nahoum.UnityJSInterop.Tests.TestInstanceMethods_static;
        TestClassImplementingInterface: Nahoum.UnityJSInterop.Tests.TestClassImplementingInterface_static;
        ITestInterface: Nahoum.UnityJSInterop.Tests.ITestInterface_static;
        TestStructExpose: Nahoum.UnityJSInterop.Tests.TestStructExpose_static;
      };
    };
    utilities: Utilities;
    extras: {
      System: {
        Action: { createDelegate: (callback: () => void) => System.Action };
        "Action<Int32>": {
          createDelegate: (
            callback: (a: System.Int32) => void
          ) => System.Action1$Int32;
        };
        "Action<String>": {
          createDelegate: (
            callback: (a: System.String) => void
          ) => System.Action1$String;
        };
        "Action<Single>": {
          createDelegate: (
            callback: (a: System.Single) => void
          ) => System.Action1$Single;
        };
        "Action<Double,String>": {
          createDelegate: (
            callback: (a: System.Double, b: System.String) => void
          ) => System.Action2$Double$String;
        };
        "Action<AnotherNamespace.SomeClass>": {
          createDelegate: (
            callback: (a: AnotherNamespace.SomeClass) => void
          ) => System.Action1$AnotherNamespace_SomeClass;
        };
      };
    };
  };
};
