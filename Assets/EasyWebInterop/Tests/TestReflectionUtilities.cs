
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

        /// <summary>
        /// Tests the IsTask method that tells if an object is a Task or Task<T> and returns the task
        /// </summary>
        [Test]
        public void IsTaskTests()
        {
            // Test for Task<string> (return string)
            object taskString = Task.FromResult("Test String");
            Assert.IsTrue(ReflectionUtilities.IsTask(taskString, out bool hasReturnValueString, out Task aTask));
            Assert.IsTrue(hasReturnValueString);
            Assert.AreEqual(taskString, aTask);
            Assert.IsInstanceOf<Task<string>>(aTask);

            // Test for Task (returns void)
            object taskVoid = Task.CompletedTask;
            Assert.IsTrue(ReflectionUtilities.IsTask(taskVoid, out bool hasReturnValueTask, out Task aTaskVoid));
            Assert.IsFalse(hasReturnValueTask);
            Assert.AreEqual(taskVoid, aTaskVoid);

            // Test for a non-task object
            object notATask = "abcd";
            Assert.IsFalse(ReflectionUtilities.IsTask(notATask, out bool hasReturnValueNotTask, out Task asTaskNotTask));
            Assert.IsFalse(hasReturnValueNotTask);
            Assert.IsNull(asTaskNotTask);

            // Test for real syntax Task
            async Task TestVoidReturn() { await Task.Delay(100); };
            var taskRunning = TestVoidReturn();
            Assert.IsTrue(ReflectionUtilities.IsTask(taskRunning, out bool hasReturnValueRealTask, out Task asTaskRealTask));
            Assert.IsFalse(hasReturnValueRealTask);
            Assert.AreEqual(taskRunning, asTaskRealTask);
        }

        [Test]
        public async Task TestGetTaskResult(){
            // Test with fromresult
            Task taskResult = Task.FromResult("Hello World");
            Assert.AreEqual("Hello World", ReflectionUtilities.GetTaskResult(taskResult));


            // Test with real async task
            async Task<string> asyncTaskResult(){
                await Task.Delay(100);
                return "Hello World";
            }
            Task<string> taskResultAsync = asyncTaskResult();
            while(!taskResultAsync.IsCompleted)
                await Task.Yield();
            Assert.AreEqual("Hello World", ReflectionUtilities.GetTaskResult(taskResultAsync));
        }
    }
}