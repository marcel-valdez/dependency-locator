using DependencyLocation.Containers;
namespace DependencyLocation
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Reflection;
    using Fasterflect;
    using Containers;

    internal class DependencyController : IDependencyConfigurator, IDependencyProvider
    {
        /// <summary>
        /// Los constructores para los tipos sin parámetros genéricos sin definir
        /// </summary>
        private readonly Dictionary<string, ConstructorContainer> mConstructors = new Dictionary<string, ConstructorContainer>();

        /// <summary>
        /// Las definiciones de tipos genéricos configuradas
        /// </summary>
        private readonly Dictionary<string, List<GenericDefinitionContainer>> mGenericDefinitions = new Dictionary<string, List<GenericDefinitionContainer>>();

        /// <summary>
        /// Son los valores de configuración del usuario
        /// </summary>
        private readonly Dictionary<object, object> mConfiguration = new Dictionary<object, object>();

        /// <summary>
        /// Son los singletons registrados en la aplicación
        /// </summary>
        private readonly Dictionary<KeyValuePair<Type, string>, object> mSingletons = new Dictionary<KeyValuePair<Type, string>, object>();

        /// <summary>
        /// Gets or sets the default key.
        /// </summary>
        /// <value>
        /// The default key.
        /// </value>
        internal string DefaultKey
        {
            get;
            set;
        }

        #region IDependencyConfigurator Members
        public void SetConfigurationValue(object key, object value)
        {
            try
            {
                this.mConfiguration.TrySetOrAdd(key, value);
            }
            catch (ArgumentException ex)
            {
                throw new ConfigurationErrorsException(String.Format(Properties.Resources.AnErrorOcurredDuringConfiguration,
                                                           value,
                                                           key), ex);
            }
        }

        public void SetConfigurationValue<T>(object key, Func<T> lazyEvaluator)
        {
            this.SetConfigurationValue(key, new Lazy<T>(lazyEvaluator));
        }

        public void SetupSingletonDependency<TInterface>(TInterface singleton, string key = null)
        {
            key = key ?? this.DefaultKey ?? "default";
            Type abstractType = typeof(TInterface);
            lock (this.mSingletons)
            {
                this.mSingletons.Add(MakePair(abstractType, key), singleton);
            }
        }

        public void SetupSingletonDependency<TInterface>(Func<TInterface> lazyEvaluator, string key = null)
        {
            key = key ?? this.DefaultKey ?? "default";
            Lazy<TInterface> lazySingleton = new Lazy<TInterface>(lazyEvaluator);
            Type type = typeof(TInterface);
            lock (this.mSingletons)
            {
                this.mSingletons.Add(MakePair(type, key), lazySingleton);
            }
        }

        public void SetupDependency<TConcrete, TInterface>(string key = null) where TConcrete : class, TInterface
        {
            key = key ?? this.DefaultKey ?? "default";

            Type concreteType = typeof(TConcrete);
            Type abstractType = typeof(TInterface);
            this.SetupDependency(concreteType, abstractType, key);
        }

        public void SetupDependency(Type concreteType, Type abstractType, string key = null)
        {
            if (concreteType.IsGenericTypeDefinition)
            {
                this.SetupGenericDependency(concreteType, abstractType, key);
            }
            else
            {
                key = key ?? this.DefaultKey ?? "default";
                ConstructorContainer container = this.mConstructors.TryGetOrAdd(key);
                container.AddInterfaceConstructors(abstractType, concreteType);
            }
        }

        public void ReleaseInjections()
        {
            this.mConstructors.Clear();
            this.mGenericDefinitions.Clear();
            this.mConfiguration.Clear();
            this.mSingletons.Clear();
        }

        private void SetupGenericDependency(Type concreteType, Type abstractType, string key = null)
        {
            Contract.Requires(concreteType != null, "Concrete type can't be null.");
            Contract.Requires(abstractType != null, "Abstract type can't be null");
            Contract.Requires(concreteType.IsGenericTypeDefinition, "Concrete type must be a generic type definition");
            Contract.Requires(abstractType.IsGenericTypeDefinition, "Abstract type must be a generic type definition");
            Contract.Requires(!concreteType.IsAbstract, "Concrete type can't be abstract.");
            Contract.Requires(concreteType.IsSubclassOrImplementsBaseGeneric(abstractType));

            key = key ?? this.DefaultKey ?? "default";
            List<GenericDefinitionContainer> lGenericDefinitions = this.mGenericDefinitions.TryGetOrAdd(key);
            lGenericDefinitions.Add(new GenericDefinitionContainer(abstractType)
                                        .SetConcrete(concreteType));
        }
        #endregion

        #region IDependencyProvider Members

        public T GetConfigurationValue<T>(object key)
        {
            try
            {
                object value = this.mConfiguration[key];
                return value as Lazy<T> != null ? (value as Lazy<T>).Value : (T)value;
            }
            catch (ArgumentException ex)
            {
                string mensaje = string.Format(Properties.Resources.ConfigurationValueNotSetForKey, key);
                throw new ConfigurationErrorsException(mensaje, ex);
            }
        }

        public TInterface CreateInstance<TInterface>(params object[] args)
        {
            return CreateNamedInstance<TInterface>(this.DefaultKey, args);
        }

        public TInterface CreateNamedInstance<TInterface>(string key, params object[] args)
        {
            Type[] lArgTypes = args != null && args.Length > 0 ? Type.GetTypeArray(args) : Type.EmptyTypes;
            Type lInterfaceType = typeof(TInterface);
            try
            {
                if (!this.mConstructors.ContainsKey(key) || !this.mConstructors[key].HasRegistered(lInterfaceType))
                {
                    GenericDefinitionContainer lGenericDefinition = this.mGenericDefinitions[key]
                                                                    .First(cGenericDefinitions => cGenericDefinitions.CanMake(lInterfaceType));
                    ConstructorContainer lConstructors = this.mConstructors.TryGetOrAdd(key);
                    lGenericDefinition.AddInterfaceConstructors(lInterfaceType, lConstructors);
                }

                ConstructorInvoker constructor = this.mConstructors[key].GetConstructor(lArgTypes, lInterfaceType);
                return args != null && args.Length > 0 ?
                        (TInterface)constructor(args) :
                        (TInterface)constructor();
            }
            catch (Exception ex)
            {

                if (ex is KeyNotFoundException ||
                    ex is InvalidOperationException)
                {
                    string tipos = lArgTypes == null || lArgTypes.Length == 0 ? "con constructor sin parámetros" : "con constructor de parámetros";
                    foreach (Type type in lArgTypes)
                    {
                        tipos += ", " + type.FullName;
                    }

                    string mensaje = string.Format("El tipo {0} no está registrado {1}", lInterfaceType.FullName, tipos);
                    throw new ConfigurationErrorsException(mensaje, ex);
                }
                else
                {
                    throw;
                }
            }
        }

        public TInterface GetInstance<TInterface>(string key = null) where TInterface : class
        {
            key = key ?? this.DefaultKey ?? "default";
            Type interfaceType = typeof(TInterface);
            try
            {
                return this.mSingletons.LookupPair<TInterface>(interfaceType, key);
            }
            catch (KeyNotFoundException ex)
            {
                string mensaje = string.Format("El tipo {0} no tiene registrado un singleton", typeof(TInterface).FullName);
                throw new ConfigurationErrorsException(mensaje, ex);
            }
        }

        #endregion

        /// <summary>
        /// Gets the pair.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <returns>Generated KeyValuePair</returns>
        private static KeyValuePair<Type, string> MakePair(Type type, string key)
        {
            Contract.Requires(type != null, "type is null.");
            Contract.Requires(key != null, "key is null.");

            return new KeyValuePair<Type, string>(type, key);
        }
    }
}
