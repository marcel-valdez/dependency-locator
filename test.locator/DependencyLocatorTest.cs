namespace Test.Locator
{
    using DependencyLocation;
    using NUnit.Framework;

    /// <summary>
    ///This is a test class for DependencyInjectorTest and is intended
    ///to contain all DependencyContainerTest Unit Tests
    ///</summary>
    [TestFixture]
    public class DependencyLocatorTest
    {
        /// <summary>
        ///A test for CreateInstance
        ///</summary>
        public TInterface CreateInstanceTestHelper<TInterface>(IDependencyProvider target, string key = "default", params object[] args)
        {
            // Arrange            
            TInterface actual;

            // Act
            actual = target.CreateNamed<TInterface>(key, args);

            // Assert
            Assert.IsNotNull(actual);
            return actual;
        }

        [Test]
        public void CanSetupAndCreateParameterlessInstance()
        {
            // Arrange
            object target = this.MakeLocator();
            (target as IDependencyConfigurator).SetupDependency<ConcreteStubDependency, IStubDependency>();

            // Act & Assert
            CreateInstanceTestHelper<IStubDependency>(target as IDependencyProvider);
        }

        [Test]
        public void CanSetupAndCreateParameterizedInstance()
        {
            // Arrange
            object target = this.MakeLocator();
            (target as IDependencyConfigurator).SetupDependency<ConcreteStubDependency, IStubDependency>();
            System.Threading.Thread.Sleep(10);

            // Act
            IStubDependency actual = CreateInstanceTestHelper<IStubDependency>(target as IDependencyProvider, "default", "test");
            string firstConstructorResult = actual.Data;
            actual = CreateInstanceTestHelper<IStubDependency>(target as IDependencyProvider, "default", "test", 10, "postfix");
            string secondConstructorResult = actual.Data;

            // Assert
            Assert.AreEqual("test", firstConstructorResult);
            Assert.AreEqual("test" + 10 + "postfix", secondConstructorResult);
        }

        /// <summary>
        ///A test for GetSingleton
        ///</summary>
        public TInterface GetSingletonTestHelper<TInterface>(IDependencyProvider target, string key = "default", TInterface expected = null)
            where TInterface : class
        {
            // Arrange
            TInterface actual;

            // Act
            actual = target.GetSingleton<TInterface>(key);

            // Assert
            Assert.AreEqual(expected, actual);
            return actual;
        }

        [Test]
        public void CanSetupAndRetrieveSingleton()
        {
            // Arrange
            ConcreteStubDependency expected = new ConcreteStubDependency("single");
            var locator = (this.MakeLocator() as IDependencyConfigurator);
            locator.SetupSingleton<IStubDependency>(expected);

            // Act
            GetSingletonTestHelper<IStubDependency>(locator as IDependencyProvider, expected: expected);

            // Assert
            // Automatically passes
        }

        /// <summary>
        ///A test for SetConfiguration
        ///</summary>
        [Test]
        public void CanSetAndGetConfigurationValues()
        {
            // Arrange
            IDependencyConfigurator target = this.MakeLocator() as IDependencyConfigurator;
            const string key = "key";
            const string expected = "configuration value";

            // Act
            target.PutConfiguration(key, expected);
            string actual = (target as IDependencyProvider).GetConfiguration<string>(key);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanCreateInstancesWithDerivedInstancesOfParameterTypes()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            target.SetupDependency<ConstructorableStub, IConstructorableStub>("default");
            InheritedStubDependency argument = new InheritedStubDependency
            {
                Data = "test"
            };

            // Act
            IConstructorableStub actual = (target as IDependencyProvider).CreateNamed<IConstructorableStub>("default", argument);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(argument.Data, actual.Data);
        }

        [Test]
        public void CanCreateInstancesWithDerivedInstancesOfTwoParameterTypes()
        {
            // Arrange
            IDependencyConfigurator target = this.MakeLocator() as IDependencyConfigurator;
            target.SetupDependency<ConstructorableStub, IConstructorableStub>("default");
            InheritedStubDependency argument = new InheritedStubDependency
            {
                Data = "test"
            };
            AnotherConstructorableStub anotherArgument = new AnotherConstructorableStub(argument);

            // Act
            IConstructorableStub actual = (target as IDependencyProvider).CreateNamed<IConstructorableStub>("default", argument, anotherArgument);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(argument.Data + anotherArgument.Data, actual.Data);
        }

        [Test]
        public void CanSetupAndRetrieveLazySingleton()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            var expected = new ParameterHelper(10);

            // Act
            target.SetupSingleton<ParameterHelper>(() => lazyValue, "default");
            lazyValue = expected;
            ParameterHelper actual = (target as IDependencyProvider).GetSingleton<ParameterHelper>("default");

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanSetupAndRetrieveLazyConfigValue()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            var expected = new ParameterHelper(10);

            // Act
            target.PutConfiguration<ParameterHelper>("key", () => lazyValue);
            lazyValue = expected;
            ParameterHelper actual = (target as IDependencyProvider).GetConfiguration<ParameterHelper>("key");

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanSetupAndCreateGenericAbstractType()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            BaseGeneric<ParameterHelper> result;

            // Act
            target.SetupDependency(typeof(Generic<>), typeof(BaseGeneric<>), "test");
            result = (target as IDependencyProvider).CreateNamed<BaseGeneric<ParameterHelper>>("test");

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CanSetupAndCreateGenericInterfaceType()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            IGeneric<ParameterHelper> result;

            // Act
            target.SetupDependency(typeof(Generic<>), typeof(IGeneric<>), "test");
            result = (target as IDependencyProvider).CreateNamed<IGeneric<ParameterHelper>>("test");

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CanSetupAndCreateGenericInterfaceTypeWithParameter()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            IGeneric<ParameterHelper> result;
            ParameterHelper expected = new ParameterHelper(10);

            // Act
            target.SetupDependency(typeof(Generic<>), typeof(IGeneric<>), "test");
            result = (target as IDependencyProvider).CreateNamed<IGeneric<ParameterHelper>>("test", expected);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Property);
            Assert.AreEqual(expected, result.Property);
        }

        [Test]
        public void ConcreteTypeHasPriorityOverGeneric()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            IGeneric<ParameterHelper> result;
            ParameterHelper expected = new ParameterHelper(10);

            // Act
            target.SetupDependency<Generic<ParameterHelper>, IGeneric<ParameterHelper>>("default");
            target.SetupDependency(typeof(CompetingGeneric<>), typeof(IGeneric<>), "default");
            result = (target as IDependencyProvider).CreateNamed<IGeneric<ParameterHelper>>("default", expected);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Generic<ParameterHelper>>(result);
        }

        public virtual object MakeLocator()
        {
            return new DependencyController();
        }

        private ParameterHelper lazyValue = null;
    }

    class ParameterHelper
    {
      public ParameterHelper()
      {
      }

      public ParameterHelper(int number)
      {
      }
    }
}