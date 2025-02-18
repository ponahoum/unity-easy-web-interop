namespace Nahoum.UnityJSInterop.Tests
{
    [ExposeWeb]
    public interface ITestInterfaceForBaseClass
    {
        string GetString();
    }

    public class TestBasicInheritance : ITestInterfaceForBaseClass
    {
        [ExposeWeb]
        public static TestBasicInheritance GetInstance() => new TestBasicInheritance();

        [ExposeWeb]
        public string GetString()
        {
            return SampleValues.TestString;
        }
    }

    public class SuperTestBasicInheritance : TestBasicInheritance
    {
        [ExposeWeb]
        public static SuperTestBasicInheritance GetInstanceSuper() => new SuperTestBasicInheritance();

        [ExposeWeb]
        public TestBasicInheritance GetAsBaseClass()
        {
            return this;
        }
    }
}