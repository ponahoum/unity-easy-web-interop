namespace Nahoum.UnityJSInterop.Tests
{
    /// <summary>
    /// Sample values to be used in tests
    /// </summary>
    public static class SampleValues
    {
        public static readonly string TestString = "test string";
        public static readonly double TestDouble = 2147483647125d;
        public static readonly int TestInt = 450050554;
        public static readonly float TestFloat = 2147483647125f;
        public static readonly float[] TestFloatArray = new float[] { 123f, 789f, 101112f };
        public static readonly double[] TestDoubleArray = new double[] { 123d, 789d, 101112d };
        public static readonly bool TestBoolTrue = true;
        public static readonly bool TestBoolFalse = false;
        public static readonly byte TestByte = 255;
        public static readonly byte[] TestByteArray = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public static readonly string TestExceptionValue = "This is a test exception";
    }
}