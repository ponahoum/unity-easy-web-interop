namespace Nahoum.UnityJSInterop.Tests
{
    public class TestActionCallbacks
    {
        public static TestActionCallbacks GetInstance()
        {
            return new TestActionCallbacks();
        }

        [ExposeWeb]
        public void TestInvokeCallbackVoid(System.Action action)
        {
            action();
        }

        [ExposeWeb]
        public void TestInvokeCallbackInt(System.Action<int> action)
        {
            action(SampleValues.TestInt);
        }

        [ExposeWeb]
        public void TestInvokeCallbackString(System.Action<string> action)
        {
            action(SampleValues.TestString);
        }

        [ExposeWeb]
        public void TestInvokeCallbackFloat(System.Action<float> action)
        {
            action(SampleValues.TestFloat);
        }

        [ExposeWeb]
        public void TestInvokeDoubleStringCallback(System.Action<double, string> action)
        {
            action(SampleValues.TestDouble, SampleValues.TestString);
        }
    }
}