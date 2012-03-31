namespace DependencyLocation
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IDependencyProvider))]
    internal abstract class IDependencyProviderCodeContract : IDependencyProvider
    {
        public TInterface GetSingleton<TInterface>(string key)
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

        public TInterface Create<TInterface>(object[] args)
        {
            return default(TInterface);
        }

        public T GetConfiguration<T>(object key)
        {
            Contract.Requires(key != null);
            return default(T);
        }

        public TInterface CreateNamed<TInterface>(string key, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}