namespace Nahoum.UnityJSInterop.Tests
{

    public enum MyEnum
    {
        Red,
        Green,
        Blue
    }

    public class TestEnums
    {
        [ExposeWeb]
        public static TestEnums GetInstance() => new TestEnums();

        [ExposeWeb]
        public MyEnum GetEnumFirstValue()
        {
            return MyEnum.Red;
        }

        [ExposeWeb]
        public MyEnum GetEnumSecondValue()
        {
            return MyEnum.Green;
        }

        [ExposeWeb]
        public MyEnum GetEnumThirdValue()
        {
            return MyEnum.Blue;
        }

        [ExposeWeb]
        public string GetEnumValueAsString(MyEnum value)
        {
            return value.ToString();
        }
    }
}