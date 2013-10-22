namespace Test.Locator
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using NUnit.ConsoleRunner;

  /// <summary>
  /// Automated unit testing program for Windows Calculator
  /// </summary>
  public sealed class Program
  {
    /// <summary>
    /// Prevents a default instance of the <see cref="Program"/> class from being created.
    /// </summary>
    private Program()
    {
    }

    /// <summary>
    /// Main entry point for this program.
    /// </summary>
    /// <param name="args">The arguments received from the console.</param>
    [STAThread]
    public static void Main(string[] args)
    {
      var argList = new List<string>();
      argList.Add(Assembly.GetExecutingAssembly().Location);
      argList.AddRange(args);

      int returnCode = Runner.Main(argList.ToArray());

      if (returnCode != 0)
      {
        Console.Beep();
      }
    }
  }
}
