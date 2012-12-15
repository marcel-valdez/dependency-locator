namespace Test.Locator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
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
        private static TestContext testContext;
        
        [ClassInitialize]
        public static void InitClass(TestContext context) {
            testContext = context;
        }
        /// <summary>
        ///A test for LoadDependencies
        ///</summary>
        [TestMethod()]
        public void LoadDependenciesTest()
        {
            // Arrange
            string configPath = GetFullFileNameInExecutionPath("TestApp.config");

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
            string configPath = GetFullFileNameInExecutionPath("TestApp.config");

            // Act
            actual = target.CallMethod("GetConfigSection", configPath) as DependencyConfiguration;

            // Assert
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// Gets the full file path of an existing output file in the execution environment
        /// </summary>
        /// <returns>The full path to the file.</returns>
        /// <param name="filename">The filename without path</param>
        private static string GetFullFileNameInExecutionPath(string filename)
        {
            string deploymentPath = String.Format("{0}/{1}", testContext.DeploymentDirectory, filename);
            if (FileExists(filename))
            {
                return deploymentPath;
            }

            string testDeploymentPath = String.Format("{0}/{1}", testContext.TestDeploymentDir, filename);
            if (FileExists(testDeploymentPath))
            {
                return testDeploymentPath;
            }

            string testDirPath = String.Format("{0}/{1}", testContext.TestDir, filename);
            if (FileExists(testDirPath))
            {
                return testDirPath;
            }

            string testRunPath = String.Format("{0}/{1}", testContext.TestRunDirectory, filename);
            if (FileExists(testRunPath))
            {
                return testRunPath;
            }
            
            string codeBaseFilePath = typeof(DependencyLoaderTest).Assembly.EscapedCodeBase;
            int stringEnd = codeBaseFilePath.LastIndexOf('/');
            string codebaseFilePath = codeBaseFilePath.Substring(0, stringEnd + 1)
                                   .Replace("file:///", "") + filename;
            
            if (FileExists(codebaseFilePath))
            {
                return codebaseFilePath;
            }

            string appDomainPath = String.Format("{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, filename);
            if (FileExists(appDomainPath))
            {
                return appDomainPath;
            }


            string currentPath = String.Format("{0}/{1}", Directory.GetCurrentDirectory(), filename);
            if (FileExists(currentPath))
            {
                return currentPath;
            }

            var stackFrameFile = new FileInfo(new StackFrame(true).GetFileName());
            string stackFramePath = String.Format("{0}/{1}", stackFrameFile.Directory.FullName, filename);
            if (FileExists(stackFramePath))
            {
                return stackFramePath;
            }            

            throw new Exception(filename + " not found.");
        }

        private static bool FileExists(string filepath)
        {
            return new FileInfo(filepath).Exists;
        }
    }
}