using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Nahoum.UnityJSInterop.Tests
{
    public class TestSerialization
    {
        [Test]
        // Ensure object serialization works on malformed string
        public void TestMalformedStringSerialization()
        {
            string malformedString = "this is a mal\"formed string";
            ObjectSerializer.TryGetSerializer(typeof(string), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(malformedString);
            Assert.AreEqual("\"this is a mal\\\"formed string\"", serialized);
        }

        [Test]
        // Test serialization of regular string
        public void TestStringSerialization()
        {
            string testString = "this is a test string";
            ObjectSerializer.TryGetSerializer(typeof(string), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testString);
            Assert.AreEqual("\"this is a test string\"", serialized);
        }

        [Test]
        // Test serialization of a few primitives types
        public void TestIntSerialization()
        {
            int testInt = 125;
            ObjectSerializer.TryGetSerializer(typeof(int), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testInt);
            Assert.AreEqual("125", serialized);
        }

        [Test]
        public void TestFloatSerialization()
        {
            float testFloat = 125.5f;
            ObjectSerializer.TryGetSerializer(typeof(float), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testFloat);
            Assert.AreEqual("125.5", serialized);
        }

        [Test]
        public void TestDoubleSerialization()
        {
            double testDouble = 125.5;
            ObjectSerializer.TryGetSerializer(typeof(double), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testDouble);
            Assert.AreEqual("125.5", serialized);
        }

        [Test]
        public void TestLongSerialization()
        {
            long testLong = 125;
            ObjectSerializer.TryGetSerializer(typeof(long), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testLong);
            Assert.AreEqual("125", serialized);
        }

        [Test]
        public void TestStringArraySerialization()
        {
            string[] testStringArray = new string[] { "this", "is", "a", "test", "array" };
            ObjectSerializer.TryGetSerializer(typeof(string[]), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testStringArray);
            Assert.AreEqual("[\"this\",\"is\",\"a\",\"test\",\"array\"]", serialized);
        }

        [Test]
        public void TestIntArraySerialization()
        {
            int[] testIntArray = new int[] { 1, 2, 3, 4, 5 };
            ObjectSerializer.TryGetSerializer(typeof(int[]), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testIntArray);
            Assert.AreEqual("[1,2,3,4,5]", serialized);
        }

        [Test]
        public void TestFloatArraySerialization()
        {
            float[] testFloatArray = new float[] { 1.5f, 2.5f, 3.5f, 4.5f, 5.5f };
            ObjectSerializer.TryGetSerializer(typeof(float[]), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testFloatArray);
            Assert.AreEqual("[1.5,2.5,3.5,4.5,5.5]", serialized);
        }

        [Test]
        public void TestDoubleArraySerialization()
        {
            double[] testDoubleArray = new double[] { 1.5, 2.5, 3.5, 4.5, 5.5 };
            ObjectSerializer.TryGetSerializer(typeof(double[]), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testDoubleArray);
            Assert.AreEqual("[1.5,2.5,3.5,4.5,5.5]", serialized);
        }

        [Test]
        public void TestLongArraySerialization()
        {
            long[] testLongArray = new long[] { 1, 2, 3, 4, 5 };
            ObjectSerializer.TryGetSerializer(typeof(long[]), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testLongArray);
            Assert.AreEqual("[1,2,3,4,5]", serialized);
        }

        [Test]
        public void TestBooleanSerialization()
        {
            bool testBool = true;
            ObjectSerializer.TryGetSerializer(typeof(bool), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testBool);
            Assert.AreEqual("true", serialized);
        }

        [Test]
        public void TestVector2Serialization()
        {
            UnityEngine.Vector2 testVector2 = new UnityEngine.Vector2(1f, 2f);
            ObjectSerializer.TryGetSerializer(typeof(UnityEngine.Vector2), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testVector2);
            Assert.AreEqual("{\"x\":1.0,\"y\":2.0}", serialized);
        }

        [Test]
        public void TestNullSerialization()
        {
            ObjectSerializer.TryGetSerializer(typeof(string), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(null);
            Assert.AreEqual("null", serialized);
        }

        [Test]
        public void TestColorSerialization()
        {
            UnityEngine.Color testColor = new UnityEngine.Color(0.5f, 0.5f, 0.5f, 0.5f);
            ObjectSerializer.TryGetSerializer(typeof(UnityEngine.Color), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testColor);
            Assert.AreEqual("{\"r\":0.5,\"g\":0.5,\"b\":0.5,\"a\":0.5}", serialized);
        }

        enum TestEnum
        {
            Value1,
            Value2,
            Value3
        }

        [Test]
        public void TestEnumSerialization()
        {
            TestEnum testEnum = TestEnum.Value2;
            ObjectSerializer.TryGetSerializer(typeof(TestEnum), out IJsJsonSerializer serializer);
            string serialized = serializer.Serialize(testEnum);
            Assert.AreEqual("\"Value2\"", serialized);
        }

    }
}