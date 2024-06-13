using System.Threading.Tasks;

namespace Nahoum.UnityJSInterop.Tests
{
    public class TestExceptions
    {
        [ExposeWeb]
        public static void ThrowSimpleException()
        {
            throw new System.Exception(SampleValues.TestExceptionValue);
        }

        [ExposeWeb]
        public void TestUnraisedException()
        {
            string obj = null;
            obj.ToString();
        }

        [ExposeWeb]
        public async Task ThrowSimpleExceptionAsync()
        {
            await Task.Yield();
            throw new System.Exception(SampleValues.TestExceptionValue);
        }

        [ExposeWeb]
        public async Task TestUnraisedExceptionAsync()
        {
            await Task.Yield();
            string obj = null;
            obj.ToString();
        }
    }
}