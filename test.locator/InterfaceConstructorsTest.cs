﻿namespace Test.Locator
{
    using System;
    using DependencyLocation;
    using Fasterflect;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestingTools.Core;
    using TestingTools.Extensions;

    /// <summary>
    ///This is a test class for InterfaceConstructorsTest and is intended
    ///to contain all InterfaceConstructorsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class InterfaceConstructorsTest
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
        ///A test for InterfaceConstructors Constructor
        ///</summary>
        [TestMethod()]
        public void CanCreateAndAssignAnInterfaceType()
        {
            // Arrange
            Type interfaceType = typeof(IStubDependency);
            InterfaceConstructors target;

            // Act
            target = new InterfaceConstructors(interfaceType);

            // Assert
            Verify.That(target.GetInterface())
                  .IsEqualTo(interfaceType)
                  .Now();
        }


        /// <summary>
        ///A test for IsType
        ///</summary>
        [TestMethod()]
        public void CanVerifyItsType()
        {
            // Arrange
            Type interfaceType = typeof(IStubDependency);
            InterfaceConstructors target;
            Type unequalType = typeof(IDisposable);
            Type equalType = typeof(IStubDependency);
            bool unequal;
            bool equal;

            // Act
            target = new InterfaceConstructors(interfaceType);
            unequal = target.IsType(unequalType);
            equal = target.IsType(equalType);

            // Assert
            Verify.That(equal).IsTrue().Now();
            Verify.That(unequal).IsFalse().Now();
        }

        /// <summary>
        ///A test for IsType
        ///</summary>
        public void IsTypeTest1Helper<TInterface, TCheck>(bool expected)
        {
            // Arrange
            Type interfaceType = typeof(TInterface);
            InterfaceConstructors target;
            bool actual;

            // Act
             target = new InterfaceConstructors(interfaceType);
            actual = target.IsType<TCheck>();

            // Assert
            Assert.AreEqual(expected, actual);
            Verify.That(actual).IsEqualTo(expected).Now();
        }

        [TestMethod()]
        public void IsTypeTest1()
        {
            IsTypeTest1Helper<IStubDependency, IStubDependency>(true);
            IsTypeTest1Helper<IStubDependency, IDisposable>(false);
        }

        /// <summary>
        ///A test for SetConcrete
        ///</summary>
        public void SetConcreteTestHelper<TInterface, TConcrete>()
        {
            // Arrange
            Type interfaceType = typeof(TInterface);
            InterfaceConstructors target;

            // Act
            target = new InterfaceConstructors(interfaceType)
                    .SetConcrete<TConcrete>();

            // Assert
            Verify.That(target.GetFieldValue("mConcreteType"))
                  .IsEqualTo(typeof(TConcrete))
                  .Now();
        }

        /// <summary>
        ///A test for TryGetConstructor
        ///</summary>
        [TestMethod()]
        public void TryGetConstructorTest()
        {
            // Arrange
            Type interfaceType = typeof(IStubDependency);
            InterfaceConstructors target;
            ConstructorInvoker ctor1 = null;
            ConstructorInvoker ctor2 = null;
            ConstructorInvoker ctor3 = null;
            object[] emptyParams = new object[] { };
            object[] existingParams = new object[] { "1", 2, "3" };
            object[] nonExistingParams = new object[] { "1", 2, "3", 4 };
            bool existing1;
            bool existing2;
            bool nonexisting;

            // Act
            target = new InterfaceConstructors(interfaceType)
                    .SetConcrete<ConcreteStubDependency>();
            existing1 = target.TryGetConstructor(out ctor1, emptyParams);
            existing2 = target.TryGetConstructor(out ctor2, existingParams);
            nonexisting = target.TryGetConstructor(out ctor3, nonExistingParams);

            // Assert
            Verify.That(existing1).IsTrue().Now();
            Verify.That(ctor1).IsNotNull().Now();

            Verify.That(existing2).IsTrue().Now();
            Verify.That(ctor2).IsNotNull().Now();

            Verify.That(nonexisting).IsFalse().Now();
            Verify.That(ctor3).IsNull().Now();

            Verify.That(ctor1)
                  .IsNotEqualTo(ctor2)
                  .And()
                  .IsNotEqualTo(ctor3)
                  .Now();

            Verify.That(ctor2)
                  .IsNotEqualTo(ctor3)
                  .Now();
        }

        /// <summary>
        ///A test for TryGetConstructor
        ///</summary>
        [TestMethod()]
        public void TryGetConstructorTest1()
        {
            // Arrange
            Type interfaceType = typeof(IStubDependency);
            InterfaceConstructors target;
            ConstructorInvoker ctor1 = null;
            ConstructorInvoker ctor2 = null;
            ConstructorInvoker ctor3 = null;
            Type[] emptyParams = Type.EmptyTypes;
            Type[] existingParams = Type.GetTypeArray(new object[] { "1", 2, "3" });
            Type[] nonExistingParams = Type.GetTypeArray(new object[] { "1", 2, "3", 4 });
            bool existing1;
            bool existing2;
            bool nonexisting;

            // Act
            target = new InterfaceConstructors(interfaceType)
                    .SetConcrete<ConcreteStubDependency>();
            existing1 = target.TryGetConstructor(out ctor1, emptyParams);
            existing2 = target.TryGetConstructor(out ctor2, existingParams);
            nonexisting = target.TryGetConstructor(out ctor3, nonExistingParams);

            // Assert
            Verify.That(existing1).IsTrue().Now();
            Verify.That(ctor1).IsNotNull().Now();

            Verify.That(existing2).IsTrue().Now();
            Verify.That(ctor2).IsNotNull().Now();

            Verify.That(nonexisting).IsFalse().Now();
            Verify.That(ctor3).IsNull().Now();

            Verify.That(ctor1)
                  .IsNotEqualTo(ctor2)
                  .And()
                  .IsNotEqualTo(ctor3)
                  .Now();

            Verify.That(ctor2)
                  .IsNotEqualTo(ctor3)
                  .Now();
        }


        /// <summary>
        ///A test for TryGetConstructor
        ///</summary>
        [TestMethod()]
        public void TryGetFromInheritedTypes()
        {
            // Arrange
            Type interfaceType = typeof(IConstructorableStub);
            InterfaceConstructors target;
            ConstructorInvoker ctorBase = null;
            ConstructorInvoker ctorConcrete = null;

            Type[] baseParam = new Type[] { typeof(IStubDependency) };
            Type[] concreteParam = new Type[] { typeof(ConcreteStubDependency) };
            
            bool existingBase;
            bool existingConcrete;

            // Act
            target = new InterfaceConstructors(interfaceType)
                        .SetConcrete<ConstructorableStub>();
            existingBase = target.TryGetConstructor(out ctorBase, baseParam);
            existingConcrete = target.TryGetConstructor(out ctorConcrete, concreteParam);
            

            // Assert
            Verify.That(existingBase).IsTrue().Now();
            Verify.That(ctorBase).IsNotNull().Now();

            Verify.That(existingConcrete).IsTrue().Now();
            Verify.That(ctorConcrete).IsNotNull().Now();          

            Verify.That(ctorBase)
                  .IsEqualTo(ctorConcrete)
                  .Now();
        }
    }
}