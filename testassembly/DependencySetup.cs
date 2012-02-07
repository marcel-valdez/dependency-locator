namespace TestAssembly
{
    using DependencyLocation;
    using DependencyLocation.Setup;

    public class DependencySetup : IDependencySetup
    {
        public void SetupDependencies(IDependencyConfigurator injector, string prefix, string defaultKey)
        {
            string key = prefix + defaultKey;

            injector.SetupDependency<ConcreteServer, IServer>(key);
        }
    }
}