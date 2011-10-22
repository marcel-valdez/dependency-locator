namespace Test.Locator
{
    using System.Collections.Generic;
    using System.Linq;
    using DependencyLocation;
    using DependencyLocation.Configuration;
    using DependencyLocation.Setup;
    using Fasterflect;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestAssembly;
    
    /// <summary>
    ///This is a test class for DependencyLoaderTest and is intended
    ///to contain all DependencyLoaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyLoaderTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }
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
        ///A test for LoadDependencies
        ///</summary>
        [TestMethod()]
        public void LoadDependenciesTest()
        {
            // Arrange

            // Act
            DependencyLoader.Loader.LoadDependencies(@".\TestApp.Config");
            IServer concrete = Dependency.Locator.CreateNamedInstance<IServer>("testPrefix.testDefault");

            // Assert
            Assert.IsNotNull(concrete);
        }

        /// <summary>
        ///A test for GetDependencies
        ///</summary>
        [TestMethod()]
        public void GetDependenciesTest()
        {
            DependencyLoader target = DependencyLoader.Loader;
            DependencyConfiguration configSection = new DependencyConfiguration();
            configSection.Dependencies = (DependencyCollection)typeof(DependencyCollection).CreateInstance();
            configSection.Dependencies.Add(new DependencyElement("test1", "test2", "test3"));
            configSection.Dependencies.Add(new DependencyElement("test21", "test22", "test23"));
            configSection.Dependencies.Add(new DependencyElement("test31", "test32", "test33"));

            IEnumerable<DependencyElement> actual;
            actual = target.CallMethod("GetDependencies", configSection) as IEnumerable<DependencyElement>;
            DependencyElement[] dependenciesArray = actual.ToArray();
            foreach (DependencyElement dependency in configSection.Dependencies)
            {
                CollectionAssert.Contains(dependenciesArray, dependency);
            }
        }

        /// <summary>
        ///A test for GetConfigSection
        ///</summary>
        [TestMethod()]
        public void GetConfigSectionTest()
        {   
            DependencyLoader target = DependencyLoader.Loader;
            DependencyConfiguration actual;
            actual = target.CallMethod("GetConfigSection", @".\TestApp.Config") as DependencyConfiguration;
            Assert.IsNotNull(actual);
        }
    }
}
