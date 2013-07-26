namespace Test.Locator
{
  using System;
  using DependencyLocation.Containers;
  using Fasterflect;
  using NUnit.Framework;
  using TestingTools.Core;
  using TestingTools.Extensions;

  /// <summary>
  /// This is a test class for GenericDefinitionContainerTest and is intended
  /// to contain all GenericDefinitionContainerTest Unit Tests
  ///</summary>
  [TestFixture]
  public class GenericDefinitionContainerTest
  {

    /// <summary>
    ///A test for GenericDefinitionContainer Constructor
    ///</summary>
    [Test]
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
    [Test]
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
    [Test]
    public void CanMakeInvalidTest4()
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
      Verify.That(actual).IsFalse("Debe responder false cuando se trata de un tipo abstracto"
                               + "\n que es una instancia de la definición de tipo genérico"
                               + "\n y del cuál deriva el tipo concreto con constructor.").Now();
    }

    /// <summary>
    ///A test for CanMake
    ///</summary>
    [Test]
    public void CanMakeInvalidTest()
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
    [Test]
    public void CanMakeInvalidTest1()
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
    [Test]
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
    [Test]
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
    [Test]
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
    [Test]
    public void MakeConstructorContainerTestValid()
    {
      // Arrange
      Type genericDefinition = typeof(IGeneric<>);
      GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                          .SetConcrete(typeof(Generic<>));
      Type generic = typeof(IGeneric<object>);
      ConstructorContainer container = new ConstructorContainer();
      // Act
      target.AddInterfaceConstructors(generic, container);

      // Assert
      Verify.That(container.GetConstructor(Type.EmptyTypes, typeof(IGeneric<object>)))
            .IsNotNull();
    }

    /// <summary>
    ///A test for MakeConstructorContainer
    ///</summary>
    [Test]
    public void MakeConstructorContainerTestInvalid()
    {
      // Arrange
      Type genericDefinition = typeof(IGeneric<>);
      GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                          .SetConcrete(typeof(Generic<>));
      Type generic = typeof(IEquatable<object>);

      // Act
      Action action = () => target.AddInterfaceConstructors(generic, new ConstructorContainer());

      // Assert
      Verify.That(action)
            .ThrowsException("Debería arrojar una excepción al intentar crear una interfaz para la cuál no está definido")
            .Now();
    }

    /// <summary>
    ///A test for MakeConstructorContainer
    ///</summary>
    [Test]
    public void MakeConstructorContainerTestInvalid1()
    {
      // Arrange
      Type genericDefinition = typeof(IGeneric<>);
      GenericDefinitionContainer target = new GenericDefinitionContainer(genericDefinition)
                                          .SetConcrete(typeof(Generic<>));
      Type generic = typeof(Generic<object>);

      // Act
      Action action = () => target.AddInterfaceConstructors(generic, new ConstructorContainer());

      // Assert
      Verify.That(action)
            .ThrowsException("Debería arrojar una excepción al intentar crear el tipo concreto de la instancia")
            .Now();
    }

    /// <summary>
    ///A test for SetConcrete
    ///</summary>
    [Test]
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
    [Test]
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
