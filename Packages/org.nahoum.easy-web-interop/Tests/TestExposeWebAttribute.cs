using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace Nahoum.UnityJSInterop.Tests
{
    public class TestExposeWebAttribute
    {
        [Test]
        public void TestExposeWebInheritance()
        {
            // Get all types with exposed methods
            System.Collections.Generic.IReadOnlyCollection<System.Type> allTypesWithExposedMethods = ExposeWebAttribute.GetAllTypesWithWebExposedMethods();

            // Ensure in the exposed types we have our class
            Assert.IsTrue(allTypesWithExposedMethods.Contains(typeof(TestClassInterface)));
            Assert.IsTrue(allTypesWithExposedMethods.Contains(typeof(TestAbstractClass)));

            // Now ensure the inheritign class has the exposed method
            Assert.IsTrue(ExposeWebAttribute.HasExposedMethods(typeof(TestClassInterface)));
            Assert.IsTrue(ExposeWebAttribute.HasExposedMethods(typeof(TestAbstractClass)));
        }
    }

    public class TestClassInterface : TestAbstractClass
    {
        public override string GetOtherString() => "hello";
    }

    public abstract class TestAbstractClass
    {

        [ExposeWeb]
        public abstract string GetOtherString();
    }

    public interface ITestInterface
    {
        [ExposeWeb]
        string GetOtherString();
    }
}