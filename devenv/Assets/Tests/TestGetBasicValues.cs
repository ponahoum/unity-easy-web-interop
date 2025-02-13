namespace Nahoum.UnityJSInterop.Tests
{
    public class TestGetBasicValues
    {
        [ExposeWeb]
        public static string GetTestString() => SampleValues.TestString;

        [ExposeWeb]
        public static double GetTestDouble() => SampleValues.TestDouble;

        [ExposeWeb]
        public static int GetTestInt() => SampleValues.TestInt;

        [ExposeWeb]
        public static bool GetTestBoolTrue() => true;

        [ExposeWeb]
        public static bool GetTestBoolFalse() => false;

        [ExposeWeb]
        public static string GetNullString() => null;
    }
}