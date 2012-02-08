namespace Test.Locator
{
    using System;
    using DependencyLocation.Extensions;
    using DependencyLocation.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///This is a test class for TypeArrayComparerTest and is intended
    ///to contain all TypeArrayComparerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TypeArrayComparerTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion Additional test attributes

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            TypeArrayComparer target = new TypeArrayComparer();
            Type[] x = new Type[] { typeof(string), typeof(Buffer), typeof(WeakReference) };
            Type[] xRef = x;
            Type[] y = new Type[] { typeof(string), typeof(Buffer), typeof(WeakReference) };
            Type[] z = new Type[] { typeof(OperatingSystem), typeof(Buffer), typeof(WeakReference) };

            Assert.IsTrue(target.Equals(x, xRef), "reference of a Type[] should be equal to another reference to the same Type[]");
            Assert.IsTrue(target.Equals(x, y), "Two arrays with the same elements, should be equal.");
            Assert.IsFalse(target.Equals(y, z), "Two arrays with different elements should be different.");
            Assert.IsFalse(target.Equals(x, z), "Two arrays with different elements should be different.");
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            TypeArrayComparer target = new TypeArrayComparer();
            Type[] x = new Type[] { typeof(string), typeof(Buffer), typeof(WeakReference) };
            Type[] xRef = x;
            Type[] y = new Type[] { typeof(string), typeof(Buffer), typeof(WeakReference) };
            Type[] z = new Type[] { typeof(OperatingSystem), typeof(Buffer), typeof(WeakReference) };

            int xHashCode = target.GetHashCode(x);
            int xRefHashCode = target.GetHashCode(xRef);
            int yHashCode = target.GetHashCode(y);
            int zHashCode = target.GetHashCode(z);
            Console.Out.WriteLine("HashCodes:\n\tx:{0}  xRef:{1}  y:{2}  z:{3}", xHashCode, xRefHashCode, yHashCode, zHashCode);
            Assert.AreEqual(xHashCode, xRefHashCode, "reference of a Type[] should be equal to another reference to the same Type[]");
            Assert.AreEqual(target.GetHashCode(x), yHashCode, "Two arrays with the same elements, should be equal.");
            Assert.AreNotEqual(target.GetHashCode(y), target.GetHashCode(z), "Two arrays with different elements should be different.");
            Assert.AreNotEqual(target.GetHashCode(x), zHashCode, "Two arrays with different elements should be different.");
        }
    }
}