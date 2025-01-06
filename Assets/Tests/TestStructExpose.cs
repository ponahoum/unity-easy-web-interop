namespace Nahoum.UnityJSInterop.Tests
{
    [ExposeWebSerialization(typeof(TsSerializerTestStructExpose))]
    public struct TestStructExpose
    {
        public int A;
        public float F;
        public string Name;

        [ExposeWeb]
        public static TestStructExpose GetExposeInstance()
        {
            return new TestStructExpose
            {
                A = SampleValues.TestInt,
                F = SampleValues.TestFloat,
                Name = SampleValues.TestString
            };
        }
    }

    public class TsSerializerTestStructExpose : DefaultTypescriptSerializer<TestStructExpose>
    {
        protected override string GetTsTypeDefinition() => "{A: number, F: number, Name: string}";

        protected override string SerializeToJavascript(TestStructExpose targetObject) => $"{{\"A\": {targetObject.A} , \"F\": {targetObject.F}, \"Name\": \"{targetObject.Name}\"}}";
    }
}