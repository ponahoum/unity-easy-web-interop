namespace Nahoum.UnityJSInterop.Tests
{
    /// <summary>
    /// Sample values to be used in tests
    /// </summary>
    public static class SampleValues
    {
        public static readonly string TestString = "test string";
        public static readonly string TestString2 = "test string 2";
        public static readonly double TestDouble = 2147483647125d;
        public static readonly int TestInt = 450050554;
        public static readonly float TestFloat = 1234567f;
        public static readonly float[] TestFloatArray = new float[] { 123f, 789f, 101112f };
        public static readonly double[] TestDoubleArray = new double[] { 123d, 789d, 101112d };
        public static readonly bool TestBoolTrue = true;
        public static readonly bool TestBoolFalse = false;
        public static readonly byte TestByte = 255;
        public static readonly byte[] TestByteArray = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public static readonly string TestExceptionValue = "This is a test exception";

        // Unity basic struct types
        public static readonly UnityEngine.Vector2 TestVector2 = new UnityEngine.Vector2(1, 2);
        public static readonly UnityEngine.Vector3 TestVector3 = new UnityEngine.Vector3(1, 2, 3);
        public static readonly UnityEngine.Vector4 TestVector4 = new UnityEngine.Vector4(1, 2, 3, 4);
        public static readonly UnityEngine.Quaternion TestQuaternion = new UnityEngine.Quaternion(1, 2, 3, 4);
        public static readonly UnityEngine.Color TestColor = new UnityEngine.Color(1, 2, 3, 4);
        public static readonly UnityEngine.Color32 TestColor32 = new UnityEngine.Color32(1, 2, 3, 4);
    }
}