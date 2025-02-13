
using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nahoum.UnityJSInterop.Tests
{
    public class TestReflectionUtilities
    {
        [Test]
        public void TestTaskIsDelegate()
        {
            // Test returns: regular method
            Assert.IsFalse(ReflectionUtilities.DelegateReturnsTask(new Func<string>(() => "Hello World")));

            // Test returns: Task  case
            Assert.IsTrue(ReflectionUtilities.DelegateReturnsTask(new Func<Task>(() => Task.CompletedTask)));

            // Test returns: Task<string> case
            Assert.IsTrue(ReflectionUtilities.DelegateReturnsTask(new Func<Task<string>>(() => Task.FromResult("Hello World"))));

            // Test return: async regular method (not Task)
            async void RealAsyncMethodNotTask()
            {
                await Task.Delay(100);
            };
            Assert.IsFalse(ReflectionUtilities.DelegateReturnsTask(new Action(RealAsyncMethodNotTask)));

            // Test: returns async Task<string> case
            async Task<string> RealAsyncMethod()
            {
                await Task.Delay(100);
                return "Hello World";
            };
            Assert.IsTrue(ReflectionUtilities.DelegateReturnsTask(new Func<Task<string>>(RealAsyncMethod)));

            // Test returns: async Task case
            async Task RealAsyncMethodNoReturn() { await Task.Delay(100); };
            Assert.IsTrue(ReflectionUtilities.DelegateReturnsTask(new Func<Task>(RealAsyncMethodNoReturn)));
            

        }

        [Test]
        public void TestIsTypeTask(){

            // Check regular Task
            bool isTask = ReflectionUtilities.IsTypeTask(typeof(Task), out bool hasReturnValueTask, out Type returnTypeTask);
            Assert.IsTrue(isTask);
            Assert.IsFalse(hasReturnValueTask);
            Assert.IsNull(returnTypeTask);

            // Check Task<string>
            isTask = ReflectionUtilities.IsTypeTask(typeof(Task<string>), out hasReturnValueTask, out returnTypeTask);
            Assert.IsTrue(isTask);
            Assert.IsTrue(hasReturnValueTask);
            Assert.AreEqual(typeof(string), returnTypeTask);

            // Check a regular random type such as a string
            isTask = ReflectionUtilities.IsTypeTask(typeof(string), out hasReturnValueTask, out returnTypeTask);
            Assert.IsFalse(isTask);
            Assert.IsFalse(hasReturnValueTask);
            Assert.IsNull(returnTypeTask);

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

        private delegate string MyCustomDelegate(int abcd);

        [Test]
        public void TestTypeIsDelegate(){
            // Test simple case
            Assert.IsTrue(ReflectionUtilities.TypeIsDelegate(typeof(Func<string>) , out Type returnType, out Type[] parametersTypes));
            Assert.AreEqual(typeof(string), returnType);
            Assert.AreEqual(0, parametersTypes.Length);

            // Test with parameters
            Assert.IsTrue(ReflectionUtilities.TypeIsDelegate(typeof(Func<int, string>) , out returnType, out parametersTypes));
            Assert.AreEqual(typeof(string), returnType);
            Assert.AreEqual(1, parametersTypes.Length);
            Assert.AreEqual(typeof(int), parametersTypes[0]);

            // Test action
            Assert.IsTrue(ReflectionUtilities.TypeIsDelegate(typeof(Action) , out returnType, out parametersTypes));
            Assert.AreEqual(typeof(void), returnType);
            Assert.AreEqual(0, parametersTypes.Length);

            // Test action with parameters
            Assert.IsTrue(ReflectionUtilities.TypeIsDelegate(typeof(Action<int>) , out returnType, out parametersTypes));
            Assert.AreEqual(typeof(void), returnType);
            Assert.AreEqual(1, parametersTypes.Length);
            Assert.AreEqual(typeof(int), parametersTypes[0]);

            // Test custom delegate
            Assert.IsTrue(ReflectionUtilities.TypeIsDelegate(typeof(MyCustomDelegate) , out returnType, out parametersTypes));
            Assert.AreEqual(typeof(string), returnType);
            Assert.AreEqual(1, parametersTypes.Length);
            Assert.AreEqual(typeof(int), parametersTypes[0]);
            
            // Test something that is not a delegate
            Assert.IsFalse(ReflectionUtilities.TypeIsDelegate(typeof(string), out returnType, out parametersTypes));
            Assert.IsNull(returnType);
            Assert.IsNull(parametersTypes);

        }
    }
}