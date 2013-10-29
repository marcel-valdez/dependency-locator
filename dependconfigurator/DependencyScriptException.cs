namespace DependencyLocation.Script
{
  using System;

  public class DependencyScriptException : Exception
  {
    public DependencyScriptException(string msg)
      : base(msg)
    {
    }
  }
}
