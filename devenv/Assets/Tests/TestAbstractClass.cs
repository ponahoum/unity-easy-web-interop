namespace Nahoum.UnityJSInterop.Tests
{
    public class TestAbstractClass : TestAbstractClassAbstract
    {
        [ExposeWeb]
        public static TestAbstractClassAbstract GetInstance() => new TestAbstractClass();

        public override string GetString() => SampleValues.TestString;
    }

    public abstract class TestAbstractClassAbstract
    {
        [ExposeWeb]
        public abstract string GetString();
    }
}