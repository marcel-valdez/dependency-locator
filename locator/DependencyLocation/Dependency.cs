namespace DependencyLocation
{
    public abstract class Dependency
    {
        private readonly static DependencyController injector = new DependencyController();

        /// <summary>
        /// Gets the dependency injector.
        /// </summary>
        public static IDependencyProvider Locator
        {
            get
            {
                return injector;
            }
        }
    }
}