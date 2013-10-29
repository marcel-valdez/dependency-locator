using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TestingTools;
using TestingTools.Core;
using TestingTools.Extensions;

namespace DependencyLocation.Script.Test
{

  [TestFixture]
  public class ScriptLoaderTest
  {

    [TearDown]
    public void cleanup()
    {
      (Dependency.Locator as IDependencyConfigurator).ReleaseInjections();
    }

    [TestAttribute]
    public void TestIfItCantLoadAScriptFile()
    {
      // Arrange
      string scriptPath = TestEnvironment.GetExecutionFilepath("test.dependfile");

      // Act
      ScriptLoader.Load(scriptPath);

      // Assert
      Verify.That(GetServiceImpl()).IsNotNull().Now();
    }

    [TestAttribute]
    public void TestIfItCantLoadAScriptFile_WithoutConfigurationInIt()
    {
      // Arrange
      string scriptPath = TestEnvironment.GetExecutionFilepath("no-config.dependfile");

      // Act
      ScriptLoader.Load(Path.GetFullPath(scriptPath));

      // Assert
      Verify.That<Func<object>>(() => GetServiceImpl()).ThrowsException().Now();
    }

    [Test]
    public void TestIfItCandLoadAnInterfaceDll()
    {
      // Arrange
      string interfaceDllPath = TestEnvironment.GetExecutionFilepath("interfaces.test.dll");
      string implDllPath = TestEnvironment.GetExecutionFilepath("impl.test.dll");
      string script = "@Using(" + interfaceDllPath + ");" + Environment.NewLine +
                      "@Using(" + implDllPath + ");" + Environment.NewLine +
        /* *
         * How can I resolve the issue of finding IService and Service without forcing the user to explicitly declare them
         * Ugly/Easy solution: Add a using statement to the generated code per each namespace in the DLLs
         * Better/Hard solution: Decompose the SetupDependency<IService, Service> instruction and add a using statement per each type
         * Technical/Hard solution: Add some sort of 'custom class loader' and attempt to find the type when it is not found, this
         *                          requires intercepting .Net's type loader
         * */
                      "using Interfaces.Test;" + Environment.NewLine +
                      "using Impl.Test;" + Environment.NewLine +
                      "Config.SetupDependency<Service, IService>();";

      // Act
      ScriptLoader.Process(script);

      // Assert
      Verify.That(AppDomain.CurrentDomain.GetAssemblies())
            .IsTrueForAny(ass => ass.FullName.Contains("interfaces.test"))
            .Now();

      Verify.That(AppDomain.CurrentDomain.GetAssemblies())
            .IsTrueForAny(ass => ass.FullName.Contains("impl.test"))
            .Now();

      Object service = GetServiceImpl();
      Verify.That(service).IsNotNull().Now();
      Verify.That(service.GetType().FullName).IsEqualTo("Impl.Test.Service").Now();
    }

    private static Object GetServiceImpl()
    {
      Type serviceType = AppDomain.CurrentDomain.GetAssemblies()
                                   .First(a => a.FullName.Contains("interfaces.test"))
                                   .GetType("Interfaces.Test.IService");

      MethodInfo genericCreateMethod = Dependency.Locator.GetType().GetMethod("Create");
      MethodInfo createMethod = genericCreateMethod.MakeGenericMethod(serviceType);
      return createMethod.Invoke(Dependency.Locator, new object[] { new object[0] });
    }
  }
}
