
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor.VersionControl;

namespace Nahoum.EasyWebInterop.Tests
{
    public class TestGCHandleBehaviour
    {
        [Test]
        public void TestGCHandleWellFreed()
        {
            // Assign new test class to object
            TestClass testClass = new TestClass();
            testClass.someValue = "Test Value";

            // Create handle to object
            IntPtr pointerToHandlerOfManagerObject = GCUtils.NewManagedObject(testClass);

            // Loose reference on purpose
            testClass = null;
            GC.Collect();

            // Now get the object back, make sure it's not null
            object obj = GCUtils.GetManagedObjectFromPtr(pointerToHandlerOfManagerObject);
            Assert.IsNotNull(obj);
            Assert.AreEqual("Test Value", ((TestClass) obj).someValue);

            // Free the object ref once again
            obj = null;
            GC.Collect();

            // Collect the object to make sure its GC
            GCUtils.CollectManagedObjectFromPtr(pointerToHandlerOfManagerObject);

            // Ensure we can't get the object back and it throws an exception
            Assert.Throws<ArgumentException>(() => GCUtils.GetManagedObjectFromPtr(pointerToHandlerOfManagerObject));
        }
    }

    public class TestClass
    {
        public string someValue = "Hello World!";
    }

    public struct TestStruct
    {
        public string someValue;
    }
}
