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
            (Dependency.Locator as IDependencyLocator).ReleaseInjections();
        }

        #endregion Additional test attributes

        /// <summary>
        ///A test for CreateInstance
        ///</summary>
        public TInterface CreateInstanceTestHelper<TInterface>(string key = "default", params object[] args)
        {
            // Arrange
            var target = Dependency.Locator;
            TInterface actual;

            // Act
            actual = target.CreateNamedInstance<TInterface>(key, args);

            // Assert
            Assert.IsNotNull(actual);
            return actual;
        }

        [TestMethod()]
        public void CanSetupAndCreateParameterlessInstance()
        {
            (Dependency.Locator as IDependencyLocator).SetupDependency<ConcreteStubDependency, IStubDependency>();
            CreateInstanceTestHelper<IStubDependency>();
        }

        [TestMethod()]
        public void CanSetupAndCreateParameterizedInstance()
        {
            // Arrange
            (Dependency.Locator as IDependencyLocator).SetupDependency<ConcreteStubDependency, IStubDependency>();

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
            var target = Dependency.Locator;
            TInterface actual;

            // Act
            actual = target.GetInstance<TInterface>(key);

            // Assert
            Assert.AreEqual(expected, actual);
            return actual;
        }

        [TestMethod()]
        public void CanSetupAndRetrieveSingleton()
        {
            // Arrange
            ConcreteStubDependency expected = new ConcreteStubDependency("single");
            (Dependency.Locator as IDependencyLocator).SetupSingletonDependency<IStubDependency>(expected);

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
            IDependencyLocator target = Dependency.Locator as IDependencyLocator;
            const string key = "key";
            const string expected = "configuration value";

            // Act
            target.SetConfigurationValue(key, expected);
            string actual = (target as IDependencyProvider).GetConfigurationValue<string>(key);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CanCreateInstancesWithDerivedInstancesOfParameterTypes()
        {
            // Arrange
            var target = Dependency.Locator as IDependencyLocator;
            target.SetupDependency<ConstructorableStub, IConstructorableStub>("default");
            InheritedStubDependency argument = new InheritedStubDependency
            {
                Data = "test"
            };

            // Act
            IConstructorableStub actual = (target as IDependencyProvider).CreateNamedInstance<IConstructorableStub>("default", argument);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(argument.Data, actual.Data);
        }

        [TestMethod()]
        public void CanCreateInstancesWithDerivedInstancesOfTwoParameterTypes()
        {
            // Arrange
            IDependencyLocator target = Dependency.Locator as IDependencyLocator;
            target.SetupDependency<ConstructorableStub, IConstructorableStub>("default");
            InheritedStubDependency argument = new InheritedStubDependency
            {
                Data = "test"
            };
            AnotherConstructorableStub anotherArgument = new AnotherConstructorableStub(argument);

            // Act
            IConstructorableStub actual = (target as IDependencyProvider).CreateNamedInstance<IConstructorableStub>("default", argument, anotherArgument);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(argument.Data + anotherArgument.Data, actual.Data);
        }

        [TestMethod()]
        public void CanSetupAndRetrieveLazySingleton()
        {
            // Arrange
            var target = Dependency.Locator as IDependencyLocator;
            var expected = new GenericParameterHelper(10);

            // Act
            target.SetupSingletonDependency<GenericParameterHelper>(() => lazyValue, "default");
            lazyValue = expected;
            GenericParameterHelper actual = (target as IDependencyProvider).GetInstance<GenericParameterHelper>("default");

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CanSetupAndRetrieveLazyConfigValue()
        {
            // Arrange
            var target = Dependency.Locator as IDependencyLocator;
            var expected = new GenericParameterHelper(10);

            // Act
            target.SetConfigurationValue<GenericParameterHelper>("key", () => lazyValue);
            lazyValue = expected;
            GenericParameterHelper actual = (target as IDependencyProvider).GetConfigurationValue<GenericParameterHelper>("key");

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CanSetupGenericAbstractType()
        {
            // Arrange
            var target = Dependency.Locator as IDependencyLocator;
            BaseGeneric<GenericParameterHelper> result;

            // Act
            target.SetupDependency(typeof(Generic<>), typeof(BaseGeneric<>), "test");
            result = (target as IDependencyProvider).CreateNamedInstance<BaseGeneric<GenericParameterHelper>>("test");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void CanSetupGenericInterfaceType()
        {
            // Arrange
            var target = Dependency.Locator as IDependencyLocator;
            IGeneric<GenericParameterHelper> result;

            // Act
            target.SetupDependency(typeof(Generic<>), typeof(IGeneric<>), "test");
            result = (target as IDependencyProvider).CreateNamedInstance<IGeneric<GenericParameterHelper>>("test");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void CanSetupGenericInterfaceTypeWithParameter()
        {
            // Arrange
            var target = Dependency.Locator as IDependencyLocator;
            IGeneric<GenericParameterHelper> result;
            GenericParameterHelper expected = new GenericParameterHelper(10);

            // Act
            target.SetupDependency(typeof(Generic<>), typeof(IGeneric<>), "test");
            result = (target as IDependencyProvider).CreateNamedInstance<IGeneric<GenericParameterHelper>>("test", expected);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Property);
            Assert.AreEqual(expected, result.Property);
        }

        [TestMethod()]
        public void ConcreteTypeHasPriorityOverGeneric()
        {
            // Arrange
            var target = Dependency.Locator as IDependencyLocator;
            IGeneric<GenericParameterHelper> result;
            GenericParameterHelper expected = new GenericParameterHelper(10);

            // Act
            target.SetupDependency<Generic<GenericParameterHelper>, IGeneric<GenericParameterHelper>>("default");
            target.SetupDependency(typeof(CompetingGeneric<>), typeof(IGeneric<>), "default");
            result = (target as IDependencyProvider).CreateNamedInstance<IGeneric<GenericParameterHelper>>("default", expected);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Generic<GenericParameterHelper>));
        }

        private GenericParameterHelper lazyValue = null;
    }
}