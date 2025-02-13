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

    public class GenericClassBase<T> : ITestInterfaceInGenericClass
    {
        [ExposeWeb]
        public string GetTypeName() => typeof(T).Name;

        [ExposeWeb]
        public string GetTypeNameFromExternal(ITestInterfaceInGenericClass input) => input.GetTypeName();
    }

    public interface ITestInterfaceInGenericClass
    {
        string GetTypeName();
    }
}