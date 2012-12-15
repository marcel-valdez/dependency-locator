
namespace Test.Locator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DependencyLocation.Containers;
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
            Verify.That(target.GetFieldValue("mCtorsList") as IEnumerable<InterfaceConstructorsContainer>)
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
            Verify.That(target.GetFieldValue("mCtorsList") as IEnumerable<InterfaceConstructorsContainer>)
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

        /// <summary>
        ///A test for HasRegistered
        ///</summary>
        [TestMethod()]
        public void HasRegisteredValidTest()
        {
            // Arrange
            Type interfaceType = typeof(IGeneric<object>);
            Type concreteType = typeof(Generic<object>);
            ConstructorContainer target = new ConstructorContainer()
                                            .AddInterfaceConstructors(interfaceType, concreteType);
            bool result;
            // Act
            result = target.HasRegistered(interfaceType);

            // Assert
            Verify.That(result).IsTrue().Now();
        }

        /// <summary>
        ///A test for HasRegistered
        ///</summary>
        [TestMethod()]
        public void HasRegisteredInvalidTest()
        {
            // Arrange
            Type interfaceType = typeof(IGeneric<object>);
            Type baseType = typeof(BaseGeneric<object>);
            Type concreteType = typeof(Generic<object>);
            ConstructorContainer target = new ConstructorContainer()
                                            .AddInterfaceConstructors(interfaceType, concreteType);
            bool result;
            // Act
            result = target.HasRegistered(baseType);

            // Assert
            Verify.That(result).IsFalse().Now();
        }

        /// <summary>
        ///A test for HasRegistered
        ///</summary>
        [TestMethod()]
        public void HasRegisteredInvalidTest1()
        {
            // Arrange
            Type interfaceType = typeof(IGeneric<object>);
            Type baseType = typeof(BaseGeneric<object>);
            Type concreteType = typeof(Generic<object>);
            ConstructorContainer target = new ConstructorContainer()
                                            .AddInterfaceConstructors(interfaceType, concreteType);
            bool result;
            // Act
            result = target.HasRegistered(concreteType);

            // Assert
            Verify.That(result).IsFalse().Now();
        }

        /// <summary>
        ///A test for HasRegistered
        ///</summary>
        [TestMethod()]
        public void HasRegisteredInvalidTest2()
        {
            // Arrange
            Type interfaceType = typeof(IGeneric<object>);
            Type targetType = typeof(IEnumerable<object>);
            Type concreteType = typeof(Generic<object>);
            ConstructorContainer target = new ConstructorContainer()
                                            .AddInterfaceConstructors(interfaceType, concreteType);
            bool result;
            // Act
            result = target.HasRegistered(concreteType);

            // Assert
            Verify.That(result).IsFalse().Now();
        }
    }
}
