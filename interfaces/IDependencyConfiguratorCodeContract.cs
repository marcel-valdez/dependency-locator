namespace DependencyLocation
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IDependencyConfigurator))]
    internal abstract class IDependencyConfiguratorCodeContract : IDependencyConfigurator
    {
        #region IDependencyLocator Members

        public void PutConfiguration(object key, object value)
        {
            Contract.Requires(key != null);
        }

        public void PutConfiguration<T>(object key, Func<T> lazyEvaluator)
        {
            Contract.Requires(key != null, "key is null.");
            Contract.Requires(lazyEvaluator != null, "lazyEvaluator is null.");
        }

        public void SetupSingleton<TInterface>(TInterface singleton, string key = null)
        {
            throw new NotImplementedException();
        }

        public void SetupSingleton<TInterface>(Func<TInterface> lazyEvaluator, string key = null)
        {
            throw new NotImplementedException();
        }

        public void SetupDependency<TConcrete, TInterface>(string key = null) where TConcrete : class, TInterface
        {
            throw new NotImplementedException();
        }

        public void SetupDependency(Type concreteType, Type abstractType, string key = null)
        {
            Contract.Requires(concreteType != null);
            Contract.Requires(abstractType != null);
            throw new NotImplementedException();
        }

        public void ReleaseInjections()
        {
            throw new NotImplementedException();
        }

        #endregion IDependencyLocator Members
    }
}