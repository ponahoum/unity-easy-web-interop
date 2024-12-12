namespace Nahoum.UnityJSInterop.Tests
{
    public class TestClassImplementingInterface : ITestInterface
    {
        [ExposeWeb]
        /// <summary>
        /// Tests that even though the method is not directly exposed, it may be used because the interface exposes it
        /// </summary>
        public string TestGetStringFromClass() => SampleValues.TestString;

        /// <summary>
        /// Exposed method from the interface - Also need to add the attribute here for safety
        /// </summary>
        [ExposeWeb]
        public string TestGetStringFromInterfaceDeclaration() => SampleValues.TestString;

        /// <summary>
        /// Gets the interface instance
        /// </summary>
        [ExposeWeb] public static ITestInterface GetNewInstanceOfInterface() => new TestClassImplementingInterface();

        /// <summary>
        /// This one is not exposed, but it is used by the interface
        /// </summary>
        public float GetSampleFloat() => SampleValues.TestFloat;
    }

    public interface ITestInterface
    {
        /// <summary>
        /// Test returning a string (direct interface implementation / aka c# feature) on static
        /// </summary>
        [ExposeWeb] public static string TestGetStringFromInterfaceStatic() => SampleValues.TestString;

        /// <summary>
        /// Test what it gives to exposeweb on the interface but not on the implementation
        /// </summary>
        [ExposeWeb] public float GetSampleFloat();

        /// <summary>
        /// Test returning a string (direct interface implementation / aka c# feature) on any instance
        /// This WONT work as it is not static, and that's ok - it's just a test to ensure it doesn't bug on generation but is just ignored
        /// </summary>
        [ExposeWeb] public string TestGetStringFromInterface() => SampleValues.TestString;

        [ExposeWeb] public string TestGetStringFromInterfaceDeclaration();

    }
}