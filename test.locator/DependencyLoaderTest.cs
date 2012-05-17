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
    using System.IO;

    /// <summary>
    ///This is a test class for DependencyLoaderTest and is intended
    ///to contain all DependencyLoaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyLoaderTest
    {

        /// <summary>
        ///A test for LoadDependencies
        ///</summary>
        [TestMethod()]
        public void LoadDependenciesTest()
        {
            // Arrange
            string configPath = GetConfigFilePath();

            // Act
            DependencyLoader.Loader.LoadDependencies(configPath);
            IServer concrete = Dependency.Locator.CreateNamed<IServer>("testPrefix.testDefault");

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
            // Arrange 
            DependencyLoader target = DependencyLoader.Loader;
            DependencyConfiguration actual;
            string configPath = GetConfigFilePath();

            // Act
            actual = target.CallMethod("GetConfigSection", configPath) as DependencyConfiguration;

            // Assert
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Gets the dependencies config file path.
        /// </summary>
        /// <returns>The full path to the configuration file.</returns>
        private static string GetConfigFilePath()
        {
            string codeBaseFilePath = typeof(DependencyLoaderTest).Assembly.EscapedCodeBase;
            int stringEnd = codeBaseFilePath.LastIndexOf('/');
            return codeBaseFilePath.Substring(0, stringEnd + 1)
                                   .Replace("file:///", "") + "TestApp.config";
        }
    }
}