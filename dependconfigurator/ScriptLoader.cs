namespace DependencyLocation.Script
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using CSScriptLibrary;
  using DependencyLocation.Setup;

  /// <summary>
  /// Class ScriptLoader loads the DependFile that will configure dependencies.
  /// </summary>
  public static class ScriptLoader
  {
    private static readonly Random rand = new Random(DateTime.Now.Millisecond);

    /// <summary>
    /// Loads the specified configuration script file specified by <paramref name="fullpath"/>.
    /// </summary>
    /// <param name="fullpath">The fullpath to the script file to load.</param>
    public static void Load(string fullpath)
    {
      if (String.IsNullOrEmpty(fullpath))
      {
        throw new ArgumentException("The path can't be null or empty.");
      }

      if (!File.Exists(fullpath))
      {
        throw new FileNotFoundException("File " + fullpath + " does not exist.");
      }

      String dependencyScript = File.ReadAllText(fullpath);
      Process(dependencyScript);
    }

    public static void Process(string script)
    {
      IEnumerable<String> assemblyFiles = FindUsedAssemblyFiles(script);
      if (!assemblyFiles.Any())
      {
        throw new DependencyScriptException("You must define at least one assembly DLL");
      }


      /*
       * Load DLL files into current Domain
       */
      foreach(String assemblyPath in assemblyFiles) {
        DependencyLoader.LoadAssembly("", assemblyPath);
      }

      string className = "Setup" + rand.Next();

      string coreHeader = "using DependencyLocation.Setup;" + Environment.NewLine +
                          "using DependencyLocation;" + Environment.NewLine;
      string header = "";
      if (GetUsingStatements(script).Any())
      {
        header = GetUsingStatements(script).Aggregate(coreHeader, (acc, current) => acc + current + Environment.NewLine);
      }

      string classDef = "public class " + className + " : IDependencySetup {" + Environment.NewLine +
                      "  public " + className + "() { }" + Environment.NewLine +
                      "  public void SetupDependencies(IDependencyConfigurator Config, string prefix, string defaultKey) {" + Environment.NewLine;
      string methodBody = "";
      if (GetSetupInstructions(script).Any())
      {
        methodBody += GetSetupInstructions(script).Aggregate((acc, current) => acc + current + Environment.NewLine);
      }
      
      string footer = "}" + Environment.NewLine + "}";

      string setupScript = header + classDef + methodBody + footer;
      IDependencySetup setup = CSScript.Evaluator.LoadCode<IDependencySetup>(setupScript);
      setup.SetupDependencies(Dependency.Locator as IDependencyConfigurator, "", "");
    }

    private static IEnumerable<string> FindUsedAssemblyFiles(string script)
    {
      return GetMethodCallsParameters(script, "@Using");
    }

    private static IEnumerable<string> GetUsingStatements(string script)
    {
      return script.Split('\n', ';').Where(line => line.Trim().StartsWith("using ")).Select(line => line.Trim() + ";");
    }

    private static IEnumerable<string> GetSetupInstructions(string script)
    {
      return script.Split('\n', ';').Where(str => str.Trim().StartsWith("Config."))
                                    .Select(str => str.Trim() + ";");
    }

    private static IEnumerable<string> GetMethodCallsParameters(string script, string method)
    {
      return script.Split('\n', ';').Where(line => line.Trim().StartsWith(method + "("))
                                    .Select(line => line.Replace(method + "(", "")
                                                        .Replace(")", "")
                                                        .Replace(";", "").Trim());
    }
  }
}