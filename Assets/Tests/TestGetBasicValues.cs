namespace Nahoum.UnityJSInterop.Tests
{
    public class TestGetBasicValues
    {
        [ExposeWeb]
        public static string GetTestString() => "This is a test string";

        [ExposeWeb]
        public static double GetTestDouble() => 123456789.123456789;

        [ExposeWeb]
        public static int GetTestInt() => 123456789;

        [ExposeWeb]
        public static bool GetTestBoolTrue() => true;

        [ExposeWeb]
        public static bool GetTestBoolFalse() => false;

        [ExposeWeb]
        public static string GetNullString() => null;
    }
}