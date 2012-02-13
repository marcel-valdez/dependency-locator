namespace Test.Locator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DependencyLocation;
    using Fasterflect;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestingTools.Core;
    using TestingTools.Extensions;

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
        /// A test for ConstructorContainer Constructor
        ///</summary>
        [TestMethod()]
        public void ConstructorContainerConstructorTest()
        {
            // Arrange
            ConstructorContainer target;

            // Act
            target = new ConstructorContainer();

            // Assert
            Verify.That(target.GetFieldValue("mCtorsList") as IEnumerable<InterfaceConstructors>)
                  .IsNotNull()
                  .And()
                  .ItsTrueThat(coll => coll.Count() == 0)
                  .Now();
        }

        /// <summary>
        /// A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            // Arrange
            ConstructorContainer target = new ConstructorContainer();
            Type cType = typeof(ConcreteStubDependency);
            Type iType = typeof(IStubDependency);

            // Act
            target.AddInterfaceConstructors(iType, cType);

            // Assert
            Verify.That(target.GetFieldValue("mCtorsList") as IEnumerable<InterfaceConstructors>)
                  .IsTrueForAny(ctors => ctors.IsType(iType))
                  .And()
                  .ItsTrueThat(coll => coll.Count() == 1)
                  .Now();
        }

        /// <summary>
        ///A test for GetConstructor
        ///</summary>
        [TestMethod()]
        public void GetLimitsConstructorTest()
        {
            // Arrange
            ConstructorContainer target;
            Type[] argTypes = Type.GetTypeArray(new object[] { "", 1, "" });
            Type[] emptyTypes = Type.EmptyTypes;
            Type[] unknownTypes = Type.GetTypeArray(new object[] { "", 1, "", 12 });
            Type interfaceType = typeof(IStubDependency);
            Type concreteType = typeof(ConcreteStubDependency);
            ConstructorInvoker valid;
            ConstructorInvoker validEmpty;
            Func<ConstructorInvoker> lGetInvalidConstructor;

            // Act
            target = new ConstructorContainer()
                        .AddInterfaceConstructors(interfaceType, concreteType);
            lGetInvalidConstructor = () => target.GetConstructor(unknownTypes, interfaceType);
            valid = target.GetConstructor(argTypes, interfaceType);
            validEmpty = target.GetConstructor(emptyTypes, interfaceType);

            // Assert
            Verify.That(valid)
                  .IsNotNull()
                  .Now();

            Verify.That(validEmpty)
                  .IsNotNull()
                  .Now();

            Verify.That(valid("", 1, ""))
                  .IsOfType(interfaceType, "El tipo debe ser del tipo de la interfaz definida.")
                  .And()
                  .IsOfType(concreteType, "El tipo debe ser del tipo de la clase concreta.")
                  .Now();

            Verify.That(validEmpty())
                  .IsOfType(interfaceType, "El tipo debe ser del tipo de la interfaz definida.")
                  .And()
                  .IsOfType(concreteType, "El tipo debe ser del tipo de la clase concreta.")
                  .Now();

            Verify.That(lGetInvalidConstructor)
                  .ThrowsException()
                  .Now();
        }

        private static ConstructorContainer_Accessor MakeAccesor(ConstructorContainer target)
        {
            return new ConstructorContainer_Accessor(new PrivateObject(target));
        }
    }
}
