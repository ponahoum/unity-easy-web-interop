
using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nahoum.EasyWebInterop.Tests
{
    public class TestReflectionUtilities
    {
        [Test]
        public void TestIsDelegateAsyncTask()
        {
            // Test returns: regular method not async
            Assert.IsFalse(ReflectionUtilities.IsDelegateAsyncTask(new Func<string>(() => "Hello World")));

            // Test returns: Task not async case
            Assert.IsFalse(ReflectionUtilities.IsDelegateAsyncTask(new Func<Task>(() => Task.CompletedTask)));

            // Test returns: Task<string> not async case
            Assert.IsFalse(ReflectionUtilities.IsDelegateAsyncTask(new Func<Task<string>>(() => Task.FromResult("Hello World"))));

            // Test return: async regular method (not Task)
            async void RealAsyncMethodNotTask()
            {
                await Task.Delay(100);
            };
            Assert.IsFalse(ReflectionUtilities.IsDelegateAsyncTask(new Action(RealAsyncMethodNotTask)));

            // Test: returns async Task<string> case
            async Task<string> RealAsyncMethod()
            {
                await Task.Delay(100);
                return "Hello World";
            };
            Assert.IsTrue(ReflectionUtilities.IsDelegateAsyncTask(new Func<Task<string>>(RealAsyncMethod)));

            // Test returns: async Task case
            async Task RealAsyncMethodNoReturn() { await Task.Delay(100); };
            Assert.IsTrue(ReflectionUtilities.IsDelegateAsyncTask(new Func<Task>(RealAsyncMethodNoReturn)));

        }
    }
}