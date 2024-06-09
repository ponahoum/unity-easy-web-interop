export type RuntimeWebTests_static = {
	key: 'RuntimeWebTests';
	GetNewTestInstance(): RuntimeWebTests;
	TestStaticMethodReturnsString(): System.String;
}

export type CSharpArray<T> = {
	GetElementAt(index: System.Int32): T;
}

export type RuntimeWebTests = {
	key: 'RuntimeWebTests';
	MyMethodReturningString(): System.String;
	MyMethodReturningDouble(): System.Double;
	MyMethodReturningInt(): System.Int32;
	TestReturnNullValue(): System.String;
	AddOneToDouble(a: System.Double): System.Double;
	MethodReturningDoubleArray(): CSharpArray<System.Double>;
	ConcatenateStrings(a: System.String, b: System.String): System.String;
	AsyncTaskReturnString(): Promise<System.String>;
	AsyncTaskStringExplicitelyFail(): Promise<System.String>;
	AsyncTaskUnraisedException(): Promise<void>;
	AsyncTaskVoidMethod(): Promise<void>;
	AsyncVoidMethod(): void;
	GetImageInformation(imageBytes: CSharpArray<System.Byte>): System.String;
	InvokeCallbackForTest(action: System.Action): void;
	InvokeCallbackWithActionString(action: System.MulticastDelegate1$String): void;
	InvokeCallbackWithActionStringDouble(action: System.MulticastDelegate2$String$Double): void;
	TestInvokedException(): void;
	TestUnraisedException(): void;
}
	& RuntimeWebTests_static; export namespace System {
		export type String = {
			key: 'System.String';
		}
		export type Double = {
			key: 'System.Double';
		}
		export type Int32 = {
			key: 'System.Int32';
		}
		export type Byte = {
			key: 'System.Byte';
		}
		export type Action = {
			key: 'System.Action';
		}
		export type MulticastDelegate1$String = {
			key: 'System.Action`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]';
		}
		export type MulticastDelegate2$String$Double = {
			key: 'System.Action`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]';
		}
	}
export namespace Nahoum.UnityJSInterop {
	export type ACoolObject_static = {
		key: 'Nahoum.UnityJSInterop.ACoolObject';
		GetNewInstance(): ACoolObject;
	}
	export type ACoolObject = {
		key: 'Nahoum.UnityJSInterop.ACoolObject';
		GetName(): System.String;
		Age(): System.Int32;
	}
		& ACoolObject_static;
}
export type UnityInstance = {
	Module: {
		static: {
			RuntimeWebTests: RuntimeWebTests_static;
			System: {
			};
			Nahoum: {
				UnityJSInterop: {
					ACoolObject: Nahoum.UnityJSInterop.ACoolObject_static;
				};
			};
		}
	}
}
