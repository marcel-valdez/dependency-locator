namespace Test.Locator
{
    using DependencyLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///This is a test class for DependencyInjectorTest and is intended
    ///to contain all DependencyContainerTest Unit Tests
    ///</summary>
    [TestClass()]
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

        [TestMethod()]
        public void CanSetupAndCreateParameterlessInstance()
        {
            // Arrange
            object target = this.MakeLocator();
            (target as IDependencyConfigurator).SetupDependency<ConcreteStubDependency, IStubDependency>();

            // Act & Assert
            CreateInstanceTestHelper<IStubDependency>(target as IDependencyProvider);
        }

        [TestMethod()]
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

        [TestMethod()]
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
        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
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

        [TestMethod()]
        public void CanSetupAndRetrieveLazySingleton()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            var expected = new GenericParameterHelper(10);

            // Act
            target.SetupSingleton<GenericParameterHelper>(() => lazyValue, "default");
            lazyValue = expected;
            GenericParameterHelper actual = (target as IDependencyProvider).GetSingleton<GenericParameterHelper>("default");

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CanSetupAndRetrieveLazyConfigValue()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            var expected = new GenericParameterHelper(10);

            // Act
            target.PutConfiguration<GenericParameterHelper>("key", () => lazyValue);
            lazyValue = expected;
            GenericParameterHelper actual = (target as IDependencyProvider).GetConfiguration<GenericParameterHelper>("key");

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CanSetupAndCreateGenericAbstractType()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            BaseGeneric<GenericParameterHelper> result;

            // Act
            target.SetupDependency(typeof(Generic<>), typeof(BaseGeneric<>), "test");
            result = (target as IDependencyProvider).CreateNamed<BaseGeneric<GenericParameterHelper>>("test");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void CanSetupAndCreateGenericInterfaceType()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            IGeneric<GenericParameterHelper> result;

            // Act
            target.SetupDependency(typeof(Generic<>), typeof(IGeneric<>), "test");
            result = (target as IDependencyProvider).CreateNamed<IGeneric<GenericParameterHelper>>("test");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void CanSetupAndCreateGenericInterfaceTypeWithParameter()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            IGeneric<GenericParameterHelper> result;
            GenericParameterHelper expected = new GenericParameterHelper(10);

            // Act
            target.SetupDependency(typeof(Generic<>), typeof(IGeneric<>), "test");
            result = (target as IDependencyProvider).CreateNamed<IGeneric<GenericParameterHelper>>("test", expected);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Property);
            Assert.AreEqual(expected, result.Property);
        }

        [TestMethod()]
        public void ConcreteTypeHasPriorityOverGeneric()
        {
            // Arrange
            var target = this.MakeLocator() as IDependencyConfigurator;
            IGeneric<GenericParameterHelper> result;
            GenericParameterHelper expected = new GenericParameterHelper(10);

            // Act
            target.SetupDependency<Generic<GenericParameterHelper>, IGeneric<GenericParameterHelper>>("default");
            target.SetupDependency(typeof(CompetingGeneric<>), typeof(IGeneric<>), "default");
            result = (target as IDependencyProvider).CreateNamed<IGeneric<GenericParameterHelper>>("default", expected);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Generic<GenericParameterHelper>));
        }

        public virtual object MakeLocator()
        {
            return new DependencyController();
        }

        private GenericParameterHelper lazyValue = null;
    }
}