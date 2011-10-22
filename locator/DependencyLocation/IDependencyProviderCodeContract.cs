namespace DependencyLocation
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IDependencyProvider))]
    internal abstract class IDependencyProviderCodeContract : IDependencyProvider
    {
        public TInterface GetInstance<TInterface>(string key)
            where TInterface : class
        {
            Contract.Ensures(Contract.Result<TInterface>() != null);
            return default(TInterface);
        }

        public TInterface CreateNamedInstance<TInterface>(object[] args, string key)
        {
            Contract.Ensures(Contract.Result<TInterface>() != null);
            return default(TInterface);
        }

        public TInterface CreateInstance<TInterface>(object[] args)
        {
            return default(TInterface);
        }

        public T GetConfigurationValue<T>(object key)
        {
            Contract.Requires(key != null);
            return default(T);
        }


        public TInterface CreateNamedInstance<TInterface>(string key, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
