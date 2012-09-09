namespace Test.Locator
{
	using System;
	using DependencyLocation.Containers;
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

		/// <summary>
		///A test for InterfaceConstructors Constructor
		///</summary>
		[TestMethod()]
		public void CanCreateAndAssignAnInterfaceType()
		{
			// Arrange
			Type interfaceType = typeof(IStubDependency);
			InterfaceConstructorsContainer target;

			// Act
			target = new InterfaceConstructorsContainer(interfaceType);

			// Assert
			Verify.That(target.GetInterface())
				  .IsEqualTo(interfaceType)
				  .Now();
		}


		/// <summary>
		///A test for IsType
		///</summary>
		[TestMethod()]
		public void CanVerifyATypeIsEqualOrUnequal()
		{
			// Arrange
			Type interfaceType = typeof(IStubDependency);
			InterfaceConstructorsContainer target;
			Type unequalType = typeof(IDisposable);
			Type equalType = typeof(IStubDependency);
			bool unequal;
			bool equal;

			// Act
			target = new InterfaceConstructorsContainer(interfaceType);
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
			InterfaceConstructorsContainer target;
			bool actual;

			// Act
			target = new InterfaceConstructorsContainer(interfaceType);
			actual = target.IsType<TCheck>();

			// Assert
			Assert.AreEqual(expected, actual);
			Verify.That(actual).IsEqualTo(expected).Now();
		}

		[TestMethod()]
		public void CanVerifyASubTypeIsOfABaseType()
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
			InterfaceConstructorsContainer target;

			// Act
			target = new InterfaceConstructorsContainer(interfaceType)
					.SetConcrete<TConcrete>();

			// Assert
			Verify.That(target.GetFieldValue("mConcreteType"))
				  .IsEqualTo(typeof(TConcrete))
				  .Now();
		}

		/// <summary>
		/// Tests if it can get a parameterless consructor (objec[] {}), a constructor with params, and not get a constructor that doesnt match the parameters
		///</summary>
		[TestMethod()]
		public void CanGetOrNotAnExistingConstructorWithEmptyParamsExistingParamsAndNonExistingParams()
		{
			// Arrange
			Type interfaceType = typeof(IStubDependency);
			InterfaceConstructorsContainer target;
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
			target = new InterfaceConstructorsContainer(interfaceType)
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
		/// Tests that it can get a parameterless constructor using Type.EmptyTypes as the argument
		///</summary>
		[TestMethod()]
		public void CanGetAConstructorUsingTypeEmptyTypesAsParams()
		{
			// Arrange
			Type interfaceType = typeof(IStubDependency);
			InterfaceConstructorsContainer target;
			ConstructorInvoker ctor1 = null;
			Type[] emptyParams = Type.EmptyTypes;
			bool existing1;

			// Act
			target = new InterfaceConstructorsContainer(interfaceType)
					.SetConcrete<ConcreteStubDependency>();
			existing1 = target.TryGetConstructor(out ctor1, emptyParams);

			// Assert
			Verify.That(existing1).IsTrue().Now();
			Verify.That(ctor1).IsNotNull().Now();
		}


		/// <summary>
		/// Tests wether it can get the constructor using parameters that derive from the actual parameters of the constructor
		/// and the actual parameter types of the constructor
		///</summary>
		[TestMethod()]
		public void CanGetAConstructorUsingInheritingTypeOrExactTypesAsParameters()
		{
			// Arrange
			Type interfaceType = typeof(IConstructorableStub);
			InterfaceConstructorsContainer target;
			ConstructorInvoker ctorBase = null;
			ConstructorInvoker ctorConcrete = null;

			Type[] baseParam = new Type[] { typeof(IStubDependency) };
			Type[] concreteParam = new Type[] { typeof(ConcreteStubDependency) };
			
			bool existingBase;
			bool existingConcrete;

			// Act
			target = new InterfaceConstructorsContainer(interfaceType)
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
