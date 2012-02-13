﻿namespace Test.Locator
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using DependencyLocation;
    using TestingTools.Core;
    using TestingTools.Extensions;
    using Fasterflect;


    /// <summary>
    ///This is a test class for GenericDefinitionContainerTest and is intended
    ///to contain all GenericDefinitionContainerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GenericDefinitionContainerTest
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
        ///A test for GenericDefinitionContainer Constructor
        ///</summary>
        [TestMethod()]
        public void GenericDefinitionContainerConstructorTest()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target;

            // Act
            target = new GenericDefinitionContainer(genericDefinition);

            // Assert
            Verify.That(target.GetFieldValue("mGenericDefinition"))
                  .IsNotNull()
                  .And()
                  .IsEqualTo(genericDefinition)
                  .Now();
        }

        /// <summary>
        ///A test for CanMake
        ///</summary>
        [TestMethod()]
        public void CanMakeValidTest()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                                .SetConcrete(typeof(Generic<>));
            Type generic = typeof(IGeneric<string>);
            bool actual;

            // Act
            actual = target.CanMake(generic);

            // Assert
            Verify.That(actual).IsTrue("Debe poder construirse un tipo de la interfaz.").Now();
        }

        /// <summary>
        ///A test for CanMake
        ///</summary>
        [TestMethod()]
        public void CanMakeValidTest1()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                                    .SetConcrete(typeof(Generic<>));
            Type generic = typeof(BaseGeneric<string>);
            bool actual;

            // Act
            actual = target.CanMake(generic);

            // Assert
            Verify.That(actual).IsTrue("Debe responder verdadero cuando se trata de un tipo abstracto"
                                     + "\n que es una instancia de la definición de tipo genérico"
                                     + "\n y del cuál deriva el tipo concreto con constructor.").Now();
        }

        /// <summary>
        ///A test for CanMake
        ///</summary>
        [TestMethod()]
        public void CanMakeInValidTest()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                                .SetConcrete(typeof(CompetingGeneric<>));

            Type generic = typeof(BaseGeneric<string>);
            bool actual;

            // Act
            actual = target.CanMake(generic);

            // Assert
            Verify.That(actual)
                  .IsFalse("No debe poder construirse un tipo abstracto, del cuál no deriva el tipo concreto.")
                  .Now();
        }

        /// <summary>
        ///A test for CanMake
        ///</summary>
        [TestMethod()]
        public void CanMakeInValidTest1()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                                .SetConcrete(typeof(CompetingGeneric<>));

            Type generic = typeof(IEquatable<string>);
            bool actual;

            // Act
            actual = target.CanMake(generic);

            // Assert
            Verify.That(actual)
                  .IsFalse("No debe poder construirse un tipo genérico que no es una instancia de la definición de tipo genérico.")
                  .Now();
        }

        /// <summary>
        ///A test for CanMake
        ///</summary>
        [TestMethod()]
        public void CanMakeInvalidTest2()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                                .SetConcrete(typeof(Generic<>));
            Type generic = typeof(BaseGeneric<>);
            bool actual = false;

            // Act
            actual = target.CanMake(generic);

            // Assert
            Verify.That(actual)
                  .IsFalse("Debe arrojar una excepción cuando se trata de una definición de tipo genérico"
                                      + "\n aunque sea una instancia de la definición de tipo genérico del contenedor"
                                      + "\n y del cuál deriva el tipo concreto.")
                  .Now();
        }

        /// <summary>
        ///A test for CanMake
        ///</summary>
        [TestMethod()]
        public void CanMakeInvalidTest3()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                                .SetConcrete(typeof(Generic<>));
            Type generic = typeof(Generic<object>);
            bool actual = false;

            // Act
            actual = target.CanMake(generic);

            // Assert
            Verify.That(actual)
                  .IsFalse("Debe arrojar una excepción cuando se trata cualquier tipo concreto.")
                  .Now();
        }


        /// <summary>
        ///A test for GetGenericDefinition
        ///</summary>
        [TestMethod()]
        public void GetGenericDefinitionTest()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition);
            Type actual;

            // Act
            actual = target.GetGenericDefinition();

            // Assert
            Verify.That(actual)
                  .IsEqualTo(genericDefinition)
                  .And()
                  .IsEqualTo(target.GetFieldValue("mGenericDefinition") as Type)
                  .Now();
        }

        /// <summary>
        ///A test for MakeConstructorContainer
        ///</summary>
        [TestMethod()]
        public void MakeConstructorContainerTestValid()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                                .SetConcrete(typeof(Generic<>));
            Type generic = typeof(IGeneric<object>);
            InterfaceConstructors actual;

            // Act
            actual = target.MakeInterfaceConstructors(generic);

            // Assert
            Verify.That(actual)
                  .IsNotNull()
                  .And()
                  .ItsTrueThat(t => t.IsType(generic))
                  .Now();
        }
                
        /// <summary>
        ///A test for MakeConstructorContainer
        ///</summary>
        [TestMethod()]
        public void MakeConstructorContainerTestInvalid()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                                .SetConcrete(typeof(Generic<>));
            Type generic = typeof(IEquatable<object>);
            InterfaceConstructors actual;

            // Act
            Action action = () => actual = target.MakeInterfaceConstructors(generic);

            // Assert
            Verify.That(action)
                  .ThrowsException("Debería arrojar una excepción al intentar crear una interfaz para la cuál no está definido")
                  .Now();
        }

        /// <summary>
        ///A test for MakeConstructorContainer
        ///</summary>
        [TestMethod()]
        public void MakeConstructorContainerTestInvalid1()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                                .SetConcrete(typeof(Generic<>));
            Type generic = typeof(Generic<object>);
            InterfaceConstructors actual;

            // Act
            Action action = () => actual = target.MakeInterfaceConstructors(generic);

            // Assert
            Verify.That(action)
                  .ThrowsException("Debería arrojar una excepción al intentar crear el tipo concreto de la instancia")
                  .Now();
        }

        /// <summary>
        ///A test for SetConcrete
        ///</summary>
        [TestMethod()]
        public void SetConcreteValidTest()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition);
            Type concrete = typeof(Generic<>);

            // Act
            Func<GenericDefinitionContainer> action = () => target.SetConcrete(concrete);

            // Assert
            Verify.That(action())
                  .IsNotNull()
                  .And()
                  .ItsTrueThat(actual => actual.Equals(target))
                  .And()
                  .ItsTrueThat(actual => actual.GetFieldValue("mConcreteGenericDefinition").Equals(concrete))
                  .Now();
        }

        /// <summary>
        ///A test for SetConcrete
        ///</summary>
        [TestMethod()]
        public void SetConcreteInvalidTest()
        {
            // Arrange
            Type genericDefinition = typeof(IGeneric<>);
            GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition);
            Type concrete = typeof(BaseGeneric<>);
            GenericDefinitionContainer actual = null;

            // Act
            Action action = () =>
                {
                    actual = target.SetConcrete(concrete);
                };

            // Assert
            Verify.That(action)
                  .ThrowsException("Debió haber lanzado una excepción de contrato")
                  .And()
                  .ItsTrueThat(_ => actual == null, "No debería regresar ningún valor, porque debió haber una excepción de contrato.")
                  .Now();
        }
    }
}