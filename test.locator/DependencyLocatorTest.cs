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
        [TestCleanup()]
        public void MyTestCleanup()
        {
            (this.GetLocator() as IDependencyConfigurator).ReleaseInjections();
        }

        #endregion Additional test attributes

        /// <summary>
        ///A test for CreateInstance
        ///</summary>
        public TInterface CreateInstanceTestHelper<TInterface>(string key = "default", params object[] args)
        {
            // Arrange
            IDependencyProvider target = this.GetLocator() as IDependencyProvider;
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
            (this.GetLocator() as IDependencyConfigurator).SetupDependency<ConcreteStubDependency, IStubDependency>();

            CreateInstanceTestHelper<IStubDependency>();
        }

        [TestMethod()]
        public void CanSetupAndCreateParameterizedInstance()
        {
            // Arrange
            (this.GetLocator() as IDependencyConfigurator).SetupDependency<ConcreteStubDependency, IStubDependency>();

            // Act
            IStubDependency actual = CreateInstanceTestHelper<IStubDependency>("default", "test");
            string firstConstructorResult = actual.Data;
            actual = CreateInstanceTestHelper<IStubDependency>("default", "test", 10, "postfix");
            string secondConstructorResult = actual.Data;

            // Assert
            Assert.AreEqual("test", firstConstructorResult);
            Assert.AreEqual("test" + 10 + "postfix", secondConstructorResult);
        }

        /// <summary>
        ///A test for GetSingleton
        ///</summary>
        public TInterface GetSingletonTestHelper<TInterface>(string key = "default", TInterface expected = null)
            where TInterface : class
        {
            // Arrange
            IDependencyProvider target = this.GetLocator() as IDependencyProvider;
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
            (this.GetLocator() as IDependencyConfigurator).SetupSingleton<IStubDependency>(expected);

            // Act
            GetSingletonTestHelper<IStubDependency>(expected: expected);

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
            IDependencyConfigurator target = this.GetLocator() as IDependencyConfigurator;
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
            var target = this.GetLocator() as IDependencyConfigurator;
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
            IDependencyConfigurator target = this.GetLocator() as IDependencyConfigurator;
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
            var target = this.GetLocator() as IDependencyConfigurator;
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
            var target = this.GetLocator() as IDependencyConfigurator;
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
            var target = this.GetLocator() as IDependencyConfigurator;
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
            var target = this.GetLocator() as IDependencyConfigurator;
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
            var target = this.GetLocator() as IDependencyConfigurator;
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
            var target = this.GetLocator() as IDependencyConfigurator;
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

        public virtual object GetLocator()
        {
            return Dependency.Locator;
        }

        private GenericParameterHelper lazyValue = null;
    }
}