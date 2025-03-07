using System.Collections.Generic;
using System.Linq;

namespace Nahoum.UnityJSInterop.Tests
{

    public class TestInstanceMethods
    {
        [ExposeWeb] public static TestInstanceMethods GetNewInstance() => new TestInstanceMethods();
        [ExposeWeb] public string TestGetString() => SampleValues.TestString;
        [ExposeWeb] public double TestGetDouble() => SampleValues.TestDouble;
        [ExposeWeb] public int TestGetInt() => SampleValues.TestInt;
        [ExposeWeb] public float TestGetFloat() => SampleValues.TestFloat;
        [ExposeWeb] public float[] TestGetFloatArray() => SampleValues.TestFloatArray;
        [ExposeWeb] public List<float> TestGetFloatList() => SampleValues.TestFloatArray.ToList();
        [ExposeWeb] public double[] TestGetDoubleArray() => SampleValues.TestDoubleArray;
        [ExposeWeb] public bool TestGetBoolTrue() => SampleValues.TestBoolTrue;
        [ExposeWeb] public bool TestGetBoolFalse() => SampleValues.TestBoolFalse;
        [ExposeWeb] public string TestGetNullString() => null;
        [ExposeWeb] public int TestAdditionInt(int a, int b) => (a + b);
        [ExposeWeb] public float TestAdditionFloat(float a, float b) => (a + b);
        [ExposeWeb] public double TestAdditionDouble(double a, double b) => (a + b);
        [ExposeWeb] public string TestAdditionString(string a, string b) => (a + b);

        // Test simple unity types
        [ExposeWeb] public UnityEngine.Vector2 TestGetVector2() => SampleValues.TestVector2;
        [ExposeWeb] public UnityEngine.Vector3 TestGetVector3() => SampleValues.TestVector3;
        [ExposeWeb] public UnityEngine.Vector4 TestGetVector4() => SampleValues.TestVector4;
        [ExposeWeb] public UnityEngine.Quaternion TestGetQuaternion() => SampleValues.TestQuaternion;
        [ExposeWeb] public UnityEngine.Color TestGetColor() => SampleValues.TestColor;
        [ExposeWeb] public UnityEngine.Color32 TestGetColor32() => SampleValues.TestColor32;
    }
}