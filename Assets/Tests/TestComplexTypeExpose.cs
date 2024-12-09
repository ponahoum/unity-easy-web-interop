using System;

namespace Nahoum.UnityJSInterop.Tests
{
    [ExposeWebSerialization(typeof(TsSerializerTestComplexExpose))]
    public struct TestStructExpose
    {
        public int X;
        public int Y;
        public string Name;

        public static TestStructExpose GetExposeInstance()
        {
            return new TestStructExpose
            {
                X = 10,
                Y = 20,
                Name = "Test"
            };
        }
    }

    public class TsSerializerTestComplexExpose : DefaultTypescriptSerializer<TestStructExpose>
    {
        protected override string GetTsTypeDefinition()
        {
            return "{X: number, Y: number, Name: string}";
        }

        protected override string SerializeToJavascript(TestStructExpose targetObject)
        {
            return $"{{X: {targetObject.X}, Y: {targetObject.Y}, Name: \"{targetObject.Name}\"}}";
        }
    }
}