namespace Test.Locator
{
  using System.Collections.Generic;
  using System.Linq;
  using DependencyLocation;
  using DependencyLocation.Configuration;
  using DependencyLocation.Setup;
  using Fasterflect;
  using NUnit.Framework;
  using TestAssembly;
  using TestingTools;

  /// <summary>
  ///This is a test class for DependencyLoaderTest and is intended
  ///to contain all DependencyLoaderTest Unit Tests
  ///</summary>
  [TestFixture]
  public class DependencyLoaderTest
  {

    /// <summary>
    ///A test for LoadDependencies
    ///</summary>
    [Test]
    public void LoadDependenciesTest()
    {
      // Arrange
      string configPath = TestEnvironment.GetExecutionFilepath("TestApp.config");

      // Act
      DependencyLoader.Loader.LoadDependencies(configPath);
      IServer concrete = Dependency.Locator.CreateNamed<IServer>("testPrefix.testDefault");

      // Assert
      Assert.IsNotNull(concrete);
    }

    /// <summary>
    ///A test for GetDependencies
    ///</summary>
    [Test]
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
    [Test]
    public void GetConfigSectionTest()
    {
      // Arrange 
      DependencyLoader target = DependencyLoader.Loader;
      DependencyConfiguration actual;
      string configPath = TestEnvironment.GetExecutionFilepath("TestApp.config");

      // Act
      actual = target.CallMethod("GetConfigSection", configPath) as DependencyConfiguration;

      // Assert
      Assert.IsNotNull(actual);
    }
  }
}