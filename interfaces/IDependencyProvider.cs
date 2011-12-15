namespace DependencyLocation
{
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(IDependencyProviderCodeContract))]
    public interface IDependencyProvider
    {
        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <typeparam name="T">Tipo de valor configurado</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Instance of type <typeparamref name="T"/></returns>
        T GetConfigurationValue<T>(object key);

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// Una instancia del tipo <typeparamref name="TInterface"/>
        /// </returns>
        TInterface CreateInstance<TInterface>(params object[] args);

        /// <summary>
        /// Creates a new instance of type <typeparamref name="TInterface"/>
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// Una instancia del tipo <typeparamref name="TInterface"/>
        /// </returns>
        TInterface CreateNamedInstance<TInterface>(string key, params object[] args);

        /// <summary>
        /// Gets an instance of the type <typeparamref name="TInterface"/>
        /// This instance has already been instantiated, and is the same each time
        /// its requeste.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <returns>Una instance del tipo <typeparamref name="TInterface"/></returns>
        TInterface GetInstance<TInterface>(string key = null)
                where TInterface : class;
    }
}