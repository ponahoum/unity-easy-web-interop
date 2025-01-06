namespace Nahoum.UnityJSInterop.Tests
{
    public class TestGenericClass : GenericClassBase<int>
    {
        [ExposeWeb]
        public static TestGenericClass CreateInstance() => new TestGenericClass();
    }

    public class TestAnotherGenericClass : GenericClassBase<double>
    {
        [ExposeWeb]
        public static TestAnotherGenericClass CreateInstance() => new TestAnotherGenericClass();
    }

    public class GenericClassBase<T> : TestInterfaceInGenericClass
    {
        [ExposeWeb]
        public string GetTypeName() => typeof(T).Name;

        [ExposeWeb]
        public string GetTypeNameFromExternal(TestInterfaceInGenericClass input) => input.GetTypeName();
    }

    public interface TestInterfaceInGenericClass
    {
        string GetTypeName();
    }
}