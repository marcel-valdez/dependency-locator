// ----------------------------------------------------------------------
// <copyright file="IDependencyInjector.cs" company="Route Manager de México">
//     Copyright Route Manager de México(c) 2011. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
namespace DependencyLocation
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(IDependencyConfiguratorCodeContract))]
    public interface IDependencyConfigurator
    {
        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void SetConfigurationValue(object key, object value);

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <typeparam name="T">Type of the configuration value</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="lazyEvaluator">The lazy evaluator.</param>
        void SetConfigurationValue<T>(object key, Func<T> lazyEvaluator);

        /// <summary>
        /// Setups the singleton dependency.
        /// </summary>
        /// <typeparam name="TConcrete">The type of the concrete.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="singleton">The singleton.</param>
        /// <param name="key">The key.</param>
        void SetupSingletonDependency<TInterface>(TInterface singleton, string key = null);

        /// <summary>
        /// Setups the singleton dependency, using a lazy evaluator (obtains the singleton in a lazy evaluation)
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="lazyEvaluator">The lazy evaluator.</param>
        /// <param name="key">The key.</param>
        void SetupSingletonDependency<TInterface>(Func<TInterface> lazyEvaluator, string key = null);

        /// <summary>
        /// Setups the dependency.
        /// </summary>
        /// <typeparam name="TConcrete">The type of the concrete.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="key">The key.</param>
        void SetupDependency<TConcrete, TInterface>(string key = null)
            where TConcrete : class, TInterface;

        /// <summary>
        /// Setups the dependency.
        /// </summary>
        /// <param name="concreteType">Type of the concrete.</param>
        /// <param name="abstractType">Type of the abstract.</param>
        /// <param name="key">The key.</param>
        void SetupDependency(Type concreteType, Type abstractType, string key = null);

        /// <summary>
        /// Releases the injections resources.
        /// </summary>
        void ReleaseInjections();
    }
}