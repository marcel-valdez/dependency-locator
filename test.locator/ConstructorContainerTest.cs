namespace Test.Locator
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using DependencyLocation;
    using Fasterflect;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestingTools.Extensions;   
    using TestingTools;
    using TestingTools.Core;
    using System.Linq;
    
    /// <summary>
    ///This is a test class for ConstructorContainerTest and is intended
    ///to contain all ConstructorContainerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConstructorContainerTest
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
        #endregion


        /// <summary>
        ///A test for ConstructorContainer Constructor
        ///</summary>
        [TestMethod()]
        public void ConstructorContainerConstructorTest()
        {
            // Arrange
            ConstructorContainer target;

            // Act
            target = new ConstructorContainer();

            // Assert
            ConstructorContainer_Accessor accesor = MakeAccesor(target);
            Verify.That(accesor.mConstructors)
                .IsNotNull()
                .Now();
            Verify.That(accesor.mConstructors)
                .ItsTrueThat(coll => coll.Count == 0)
                .Now();
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            // Arrange
            ConstructorContainer target = new ConstructorContainer();
            ConstructorInfo constructor = typeof(ConcreteStubDependency).Constructors()[0];
            Type target1 = typeof(IStubDependency);
            string key = "name";

            // Act
            target.Add(constructor, target1, key);

            // Assert
            Verify.That(MakeAccesor(target).mConstructors)
                .ItsTrueThat(ctors => ctors.ContainsKey(new KeyValuePair<Type, string>(target1, key)))
                .And()
                .ItsTrueThat(ctors => ctors[new KeyValuePair<Type, string>(target1, key)] != null)
                .Now();
        }

        /// <summary>
        ///A test for GetConstructor
        ///</summary>
        [TestMethod()]
        public void GetConstructorTest()
        {
            // Arrange
            ConstructorContainer target = new ConstructorContainer(); // TODO: Initialize to an appropriate value
            Type[] argTypes = null; // TODO: Initialize to an appropriate value
            Type interfaceType = null; // TODO: Initialize to an appropriate value
            string key = string.Empty; // TODO: Initialize to an appropriate value
            ConstructorInvoker expected = null; // TODO: Initialize to an appropriate value
            ConstructorInvoker actual;

            // Act
            actual = target.GetConstructor(argTypes, interfaceType, key);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetMatchingConstructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("DependencyLocator.dll")]
        public void GetMatchingConstructorTest()
        {
            // Arrange
            Dictionary<Type[], ConstructorInvoker> constructors = null; // TODO: Initialize to an appropriate value
            Type[] argTypes = null; // TODO: Initialize to an appropriate value
            ConstructorInvoker expected = null; // TODO: Initialize to an appropriate value
            ConstructorInvoker actual;

            // Act
            actual = ConstructorContainer_Accessor.GetMatchingConstructor(constructors, argTypes);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetParamTypes
        ///</summary>
        [TestMethod()]
        [DeploymentItem("DependencyLocator.dll")]
        public void GetParamTypesTest()
        {
            // Arrange
            ConstructorInfo constructor = null; // TODO: Initialize to an appropriate value
            Type[] expected = null; // TODO: Initialize to an appropriate value
            Type[] actual;

            // Act
            actual = ConstructorContainer_Accessor.GetParamTypes(constructor);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MakePair
        ///</summary>
        [TestMethod()]
        [DeploymentItem("DependencyLocator.dll")]
        public void MakePairTest()
        {
            // Arrange
            Type type = null; // TODO: Initialize to an appropriate value
            string key = string.Empty; // TODO: Initialize to an appropriate value
            KeyValuePair<Type, string> expected = new KeyValuePair<Type, string>(); // TODO: Initialize to an appropriate value
            KeyValuePair<Type, string> actual;

            // Act
            actual = ConstructorContainer_Accessor.MakePair(type, key);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Set
        ///</summary>
        [TestMethod()]
        public void SetTest()
        {
            // Arrange
            ConstructorContainer target = new ConstructorContainer(); // TODO: Initialize to an appropriate value
            ConstructorInfo constructor = null; // TODO: Initialize to an appropriate value
            Type target1 = null; // TODO: Initialize to an appropriate value
            string key = string.Empty; // TODO: Initialize to an appropriate value

            // Act
            target.Set(constructor, target1, key);

            // Assert
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for TrySet
        ///</summary>
        [TestMethod()]
        public void TrySetTest()
        {
            // Arrange
            ConstructorContainer target = new ConstructorContainer(); // TODO: Initialize to an appropriate value
            ConstructorInfo constructor = null; // TODO: Initialize to an appropriate value
            Type target1 = null; // TODO: Initialize to an appropriate value
            string key = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;

            // Act
            actual = target.TrySet(constructor, target1, key);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        private static ConstructorContainer_Accessor MakeAccesor(ConstructorContainer target)
        {
            return new ConstructorContainer_Accessor(new PrivateObject(target));
        }
    }
}
