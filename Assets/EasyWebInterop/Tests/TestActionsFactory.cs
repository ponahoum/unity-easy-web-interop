using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NUnit.Framework;
using static Nahoum.EasyWebInterop.DyncallSignature;

namespace Nahoum.EasyWebInterop
{

    public class TestActionFactory
    {
        [Test]
        public void TestJsActionVoidCreation(){
            bool jsSideCalled = false;
            V theFakeJsAction = () =>
            {
                jsSideCalled = true;
            };
            IntPtr pointerToAction = Marshal.GetFunctionPointerForDelegate(theFakeJsAction);
            var action = ManagedActionFactory.GetWrappedActionFromJsDelegate(new string[] { }, pointerToAction);

            if (action is Action asAction)
            {
                asAction.Invoke();
                Assert.Pass();
            }

            else
                Assert.Fail();

            Assert.IsTrue(jsSideCalled);
        }

        [Test]
        public void TestJsActionTCreation()
        {
            // Simulate the js side calling the action
            bool jsSideCalled = false;
            IntPtr gatheredPtr = IntPtr.Zero;
            VI theFakeJsAction = (IntPtr ptr) =>
            {
                gatheredPtr = ptr;
                jsSideCalled = true;
            };

            // Get intptr from this action
            IntPtr pointerToAction = Marshal.GetFunctionPointerForDelegate(theFakeJsAction);

            // Now gather an Action<double> that internally calls this action with the double being wrapped
            var actionDouble = ManagedActionFactory.GetWrappedActionFromJsDelegate(new string[] { "System.Double" }, pointerToAction);

            // Ensure actionDouble is Action<double>
            if (actionDouble is Action<double> asActionDouble)
            {
                // Call the action
                asActionDouble.Invoke(125);
                Assert.Pass();
            }

            else
                Assert.Fail();

            Assert.IsTrue(jsSideCalled);
            Assert.IsFalse(gatheredPtr == IntPtr.Zero);

            // Now ensure the pointer contains the right information
            // The pointer should contain the double 125
            object doubleGathered = GCUtils.GetManagedObjectFromPtr(gatheredPtr);
            Assert.IsTrue(doubleGathered is double);
            Assert.AreEqual(125, (double)doubleGathered);
        }

        [Test]
        public void TestJsActionTUCreation()
        {
            // Simulate the js side calling the action
            bool jsSideCalled = false;
            IntPtr gatheredPtrA = IntPtr.Zero;
            IntPtr gatheredPtrB = IntPtr.Zero;
            VII theFakeJsAction = (IntPtr ptrA, IntPtr ptrB) =>
            {
                gatheredPtrA = ptrA;
                gatheredPtrA = ptrB;
                jsSideCalled = true;
            };

            // Get intptr from this action
            IntPtr pointerToFakeJsAction = Marshal.GetFunctionPointerForDelegate(theFakeJsAction);

            // Now gather an Action<double, string> that internally calls this action with the double being wrapped
            var actionDoubleString = ManagedActionFactory.GetWrappedActionFromJsDelegate(new string[] { "System.Double", "System.String" }, pointerToFakeJsAction);

            // Ensure actionDoubleString is Action<double, string>
            if (actionDoubleString is Action<double, string> asActionDoubleString)
            {
                // Call the action
                asActionDoubleString.Invoke(125, "Hello");
                Assert.Pass();
            }

            else
                Assert.Fail();
            
            Assert.IsTrue(jsSideCalled);
            Assert.IsFalse(gatheredPtrA == IntPtr.Zero);
            Assert.IsFalse(gatheredPtrB == IntPtr.Zero);
            object doubleGathered = GCUtils.GetManagedObjectFromPtr(gatheredPtrA);
            object stringGathered = GCUtils.GetManagedObjectFromPtr(gatheredPtrB);
            Assert.IsTrue(doubleGathered is double);
            Assert.IsTrue(stringGathered is string);
            Assert.AreEqual(125, (double)doubleGathered);
            Assert.AreEqual("Hello", (string)stringGathered);
        }

        [Test]
        public void TestJsActionTUVCreation()
        {
            // Simulate the js side calling the action
            bool jsSideCalled = false;
            IntPtr gatheredPtrA = IntPtr.Zero;
            IntPtr gatheredPtrB = IntPtr.Zero;
            IntPtr gatheredPtrC = IntPtr.Zero;
            VIII theFakeJsAction = (IntPtr ptrA, IntPtr ptrB, IntPtr ptrC) =>
            {
                gatheredPtrA = ptrA;
                gatheredPtrB = ptrB;
                gatheredPtrC = ptrC;
                jsSideCalled = true;
            };

            // Get intptr from this action
            IntPtr pointerToFakeJsAction = Marshal.GetFunctionPointerForDelegate(theFakeJsAction);

            // Now gather an Action<double, string, int> that internally calls this action with the double being wrapped
            var actionDoubleStringInt =  ManagedActionFactory.GetWrappedActionFromJsDelegate(new string[] { "System.Double", "System.String", "System.Int32" }, pointerToFakeJsAction);

            // Ensure actionDoubleStringInt is Action<double, string, int>
            if (actionDoubleStringInt is Action<double, string, int> asActionDoubleStringInt)
            {
                // Call the action
                asActionDoubleStringInt.Invoke(125, "Hello", 5);
                Assert.Pass();
            }

            else
                Assert.Fail();
            
            Assert.IsTrue(jsSideCalled);
            Assert.IsFalse(gatheredPtrA == IntPtr.Zero);
            Assert.IsFalse(gatheredPtrB == IntPtr.Zero);
            Assert.IsFalse(gatheredPtrC == IntPtr.Zero);
            object doubleGathered = GCUtils.GetManagedObjectFromPtr(gatheredPtrA);
            object stringGathered = GCUtils.GetManagedObjectFromPtr(gatheredPtrB);
            object intGathered = GCUtils.GetManagedObjectFromPtr(gatheredPtrC);
            Assert.IsTrue(doubleGathered is double);
            Assert.IsTrue(stringGathered is string);
            Assert.IsTrue(intGathered is int);
            Assert.AreEqual(125, (double)doubleGathered);
            Assert.AreEqual("Hello", (string)stringGathered);
            Assert.AreEqual(5, (int)intGathered);
        }

    }
}