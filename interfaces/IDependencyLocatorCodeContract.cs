namespace DependencyLocation
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IDependencyLocator))]
    internal abstract class IDependencyLocatorCodeContract : IDependencyProviderCodeContract, IDependencyLocator
    {
        #region IDependencyLocator Members

        public void SetConfigurationValue(object key, object value)
        {
            Contract.Requires(key != null);
        }

        public void SetConfigurationValue<T>(object key, Func<T> lazyEvaluator)
        {
            Contract.Requires(key != null, "key is null.");
            Contract.Requires(lazyEvaluator != null, "lazyEvaluator is null.");
        }

        public void SetupSingletonDependency<TInterface>(TInterface singleton, string key = null)
        {
            throw new NotImplementedException();
        }

        public void SetupSingletonDependency<TInterface>(Func<TInterface> lazyEvaluator, string key = null)
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
        #endregion
    }
}
