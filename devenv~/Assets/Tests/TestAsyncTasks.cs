using System.Threading.Tasks;

namespace Nahoum.UnityJSInterop.Tests
{
    public class TestTasks
    {
        [ExposeWeb]
        public static TestTasks GetInstance()
        {
            return new TestTasks();
        }

        [ExposeWeb]
        public async Task TestTaskVoid()
        {
            await Task.Yield();

        }
        [ExposeWeb]
        public async Task<string> TestTaskString()
        {
            await Task.Yield();
            return SampleValues.TestString;
        }

        [ExposeWeb]
        public async Task<int> TestTaskInt()
        {
            await Task.Yield();
            return SampleValues.TestInt;
        }

        [ExposeWeb]
        public async Task<float> TestTaskFloat()
        {
            await Task.Yield();
            return SampleValues.TestFloat;
        }

        [ExposeWeb]
        public async void AsyncVoidMethod() => await Task.Yield();
    }
}