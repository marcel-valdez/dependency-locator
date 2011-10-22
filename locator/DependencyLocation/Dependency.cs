namespace DependencyLocation
{
    public abstract class Dependency
    {
        private readonly static DependencyContainer injector = new DependencyContainer();

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
