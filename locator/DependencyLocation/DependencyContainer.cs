// ----------------------------------------------------------------------
// <copyright file="DependencyInjector.cs" company="Route Manager de México">
// Copyright Route Manager de México(c) 2011. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
namespace DependencyLocation
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using CommonUtilities.Reflection;
    using Fasterflect;

    /// <summary>
    /// Clase sencilla, utilizada para cargar dependencias dinámicamente.
    /// </summary>
    internal class DependencyContainer : IDependencyLocator, IDependencyProvider
    {

        /// <summary>
        /// Son los valores de configuración del usuario
        /// </summary>
        private readonly Dictionary<object, object> mConfiguration = new Dictionary<object, object>();

        /// <summary>
        /// Son los constructores de tipos concretos registrados
        /// </summary>
        private readonly Dictionary<KeyValuePair<Type, string>, Dictionary<Type[], ConstructorInvoker>> mConstructors =
                                    new Dictionary<KeyValuePair<Type, string>, Dictionary<Type[], ConstructorInvoker>>();

        /// <summary>
        /// Es el caché de constructores genérico pedidos
        /// </summary>
        private readonly Dictionary<KeyValuePair<Type, string>, Dictionary<Type[], ConstructorInvoker>> mGenericConstructors =
                            new Dictionary<KeyValuePair<Type, string>, Dictionary<Type[], ConstructorInvoker>>();

        /// <summary>
        /// Son las relaciones de tipos genéricos registrados (GenericTypeDefinition -> AbstractGenericTypeDefinition)
        /// </summary>
        private readonly Dictionary<KeyValuePair<Type, string>, object> mGenericTypeRelations = new Dictionary<KeyValuePair<Type, string>, object>();

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

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <typeparam name="T">Type of the configuration value</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="lazyEvaluator">The lazy evaluator.</param>
        public void SetConfigurationValue<T>(object key, Func<T> lazyEvaluator)
        {
            this.SetConfigurationValue(key, new Lazy<T>(lazyEvaluator));
        }

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetConfigurationValue(object key, object value)
        {
            try
            {
                this.mConfiguration.Add(key, value);
            }
            catch (ArgumentException ex)
            {

                throw new ConfigurationErrorsException("Configuration value already set.", ex);
            }
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <typeparam name="T">Type of the value stored in the configuration</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Configuration value</returns>
        public T GetConfigurationValue<T>(object key)
        {
            try
            {
                object value = this.mConfiguration[key];
                return value as Lazy<T> != null ? (value as Lazy<T>).Value : (T)value;
            }
            catch (ArgumentException ex)
            {
                string mensaje = string.Format("Configuration value for {0} not set.", key);
                throw new ConfigurationErrorsException(mensaje, ex);
            }
        }


        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="args">The constructor arguments</param>
        /// <returns>Una instancia del tipo de la <TInterface>interfaz</TInterface></returns>
        public TInterface CreateInstance<TInterface>(params object[] args)
        {
            return CreateNamedInstance<TInterface>(this.DefaultKey, args);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="args">The args.</param>
        /// <returns>Una instancia del tipo de la <TInterface>interfaz</TInterface></returns>
        public TInterface CreateNamedInstance<TInterface>(string key, params object[] args)
        {
            Type[] argTypes = args != null && args.Length > 0 ? Type.GetTypeArray(args) : Type.EmptyTypes;
            Type interfaceType = typeof(TInterface);
            try
            {
                Dictionary<Type[], ConstructorInvoker> tInterfaceConstructors = null;
                if (!this.mConstructors.TryGetValue(GetPair(interfaceType, key), out tInterfaceConstructors) && interfaceType.IsGenericType)
                {
                    return this.CreateGenericInstance<TInterface>(key, args);
                }
                else if (tInterfaceConstructors != null)
                {
                    ConstructorInvoker constructor = GetMatchingConstructor(tInterfaceConstructors, argTypes);
                    return args != null && args.Length > 0 ?
                        (TInterface)constructor(args) :
                        (TInterface)constructor();
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            catch (KeyNotFoundException ex)
            {
                string tipos = argTypes == null || argTypes.Length == 0 ? "con constructor sin parámetros" : "con constructor de parámetros";
                foreach (Type type in argTypes)
                {
                    tipos += ", " + type.FullName;
                }

                string mensaje = string.Format("El tipo {0} no está registrado {1}", interfaceType.FullName, tipos);
                throw new ConfigurationErrorsException(mensaje, ex);
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <returns>La interfaz</returns>
        public TInterface GetInstance<TInterface>(string key = null)
            where TInterface : class
        {
            key = key ?? this.DefaultKey ?? "default";
            Type interfaceType = typeof(TInterface);
            try
            {
                return LookupPair<TInterface>(this.mSingletons, interfaceType, key);
            }
            catch (KeyNotFoundException ex)
            {
                string mensaje = string.Format("El tipo {0} no tiene registrado un singleton", typeof(TInterface).FullName);
                throw new ConfigurationErrorsException(mensaje, ex);
            }
        }

        /// <summary>
        /// Setups the singleton dependency.
        /// </summary>
        /// <typeparam name="TConcrete">The type of the concrete.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="singleton">The singleton.</param>
        /// <param name="key">The key.</param>
        public void SetupSingletonDependency<TInterface>(TInterface singleton, string key = null)
        {
            key = key ?? this.DefaultKey ?? "default";
            Type abstractType = typeof(TInterface);
            lock (this.mSingletons)
            {
                this.mSingletons.Add(GetPair(abstractType, key), singleton);
            }
        }

        /// <summary>
        /// Setups the singleton dependency, using a lazy evaluator (obtains the singleton in a lazy evaluation)
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="lazyEvaluator">The lazy evaluator.</param>
        /// <param name="key">The key.</param>
        public void SetupSingletonDependency<TInterface>(Func<TInterface> lazyEvaluator, string key = null)
        {
            key = key ?? this.DefaultKey ?? "default";
            Lazy<TInterface> lazySingleton = new Lazy<TInterface>(lazyEvaluator);
            Type type = typeof(TInterface);
            lock (this.mSingletons)
            {
                this.mSingletons.Add(GetPair(type, key), lazySingleton);
            }
        }

        /// <summary>
        /// Setups the dependency.
        /// </summary>
        /// <param name="concreteType">Type of the concrete.</param>
        /// <param name="abstractType">Type of the abstract.</param>
        /// <param name="key">The key.</param>
        public void SetupDependency(Type concreteType, Type abstractType, string key = null)
        {
            if (concreteType.IsGenericTypeDefinition)
            {
                this.SetupGenericDependency(concreteType, abstractType, key);
                return;
            }

            Contract.Assume(abstractType.IsAssignableFrom(concreteType));
            key = key ?? this.DefaultKey ?? "default";
            ConstructorInfo[] constructorsInfo = concreteType.GetConstructors();
            foreach (var constructorInfo in constructorsInfo)
            {
                lock (this.mConstructors)
                {
                    Dictionary<Type[], ConstructorInvoker> constructors = null;
                    if (!this.mConstructors.TryGetValue(GetPair(abstractType, key), out constructors))
                    {
                        constructors = new Dictionary<Type[], ConstructorInvoker>(new TypeArrayComparer());
                        this.mConstructors.Add(
                            GetPair(abstractType, key),
                            constructors);
                    }

                    try
                    {
                        Type[] paramTypes = constructorInfo.GetParameters()
                                                .Select(info => info.ParameterType)
                                                .ToArray();
                        if (paramTypes.Length == 0)
                        {
                            paramTypes = Type.EmptyTypes;
                        }

                        ConstructorInvoker invoker = null;
                        if (!constructors.TryGetValue(paramTypes, out invoker))
                        {
                            invoker = constructorInfo.DelegateForCreateInstance();
                            constructors.Add(paramTypes, invoker);
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        string message = string.Format("El tipo de {0} ya ha sido agregado anteriormente.", abstractType.FullName);
                        throw new ConfigurationErrorsException(message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Setups the dependency.
        /// </summary>
        /// <typeparam name="TConcrete">The type of the concrete.</typeparam>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="key">The key.</param>
        public void SetupDependency<TConcrete, TInterface>(string key = null)
            where TConcrete : class, TInterface
        {
            key = key ?? this.DefaultKey ?? "default";

            Type concreteType = typeof(TConcrete);
            Type abstractType = typeof(TInterface);
            this.SetupDependency(concreteType, abstractType, key);
        }

        /// <summary>
        /// Releases the injections resources.
        /// </summary>
        public void ReleaseInjections()
        {
            this.mSingletons.Clear();
            this.mConfiguration.Clear();
            this.mConstructors.Clear();
            this.mGenericTypeRelations.Clear();
            this.mGenericConstructors.Clear();
        }

        /// <summary>
        /// Lookups the config pair.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <returns>Generated object</returns>
        private static T LookupPair<T>(Dictionary<KeyValuePair<Type, string>, object> dict, Type type, string key)
            where T : class
        {
            Contract.Requires(dict != null, "dict is null.");
            Contract.Requires(type != null, "type is null.");
            Contract.Requires(key != null, "key is null.");
            Contract.Ensures(Contract.Result<T>() != null);

            var pair = GetPair(type, key);
            object result = dict[pair];
            return result as Lazy<T> != null ? ((Lazy<T>)result).Value : (T)result;
        }

        /// <summary>
        /// Gets the pair.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <returns>Generated KeyValuePair</returns>
        private static KeyValuePair<Type, string> GetPair(Type type, string key)
        {
            Contract.Requires(type != null, "type is null.");
            Contract.Requires(key != null, "key is null.");

            return new KeyValuePair<Type, string>(type, key);
        }

        /// <summary>
        /// Matches the types.
        /// </summary>
        /// <param name="constructors">The constructors.</param>
        /// <param name="argTypes">The arg types.</param>
        /// <returns>ConstructorInvoker that matches the <paramref name="argTypes"/></returns>
        private static ConstructorInvoker GetMatchingConstructor(Dictionary<Type[], ConstructorInvoker> constructors, Type[] argTypes)
        {
            Contract.Requires(constructors != null, "constructors is null.");
            Contract.Requires(argTypes != null, "argumentTypes is null or empty.");
            Contract.Ensures(Contract.Result<ConstructorInvoker>() != null);

            // First match the amount of arguments
            Type[] ctorArgsTypes = constructors.Keys
                            .Where(types => types.Length == argTypes.Length)
                            .Where(ctorTypes =>
                            {
                                Contract.Assume(ctorTypes != null);
                                bool match = true;
                                for (int i = 0; match && i < argTypes.Length; i++)
                                {
                                    match = ctorTypes[i].IsAssignableFrom(argTypes[i]);
                                }

                                return match;
                            }).First();

            return constructors[ctorArgsTypes];
        }

        /// <summary>
        /// Determines whether [is subclass of raw generic] [the specified sub generic].
        /// </summary>
        /// <param name="subGeneric">The sub generic.</param>
        /// <param name="baseGeneric">The base generic.</param>
        /// <returns>
        /// <c>true</c> if [is subclass of raw generic] [the specified sub generic]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSubclassOfRawGeneric(Type subGeneric, Type baseGeneric)
        {
            Contract.Requires(subGeneric != null, "subGeneric is null.");
            Contract.Requires(baseGeneric != null, "baseGeneric is null.");
            Contract.Requires(subGeneric.IsGenericType);
            Contract.Requires(baseGeneric.IsGenericType);

            while (subGeneric != typeof(object))
            {
                Type currentType = subGeneric.IsGenericType ? subGeneric.GetGenericTypeDefinition() : subGeneric;
                if (currentType.IsGenericType && baseGeneric.GetGenericTypeDefinition() == currentType.GetGenericTypeDefinition())
                {
                    return true;
                }

                subGeneric = subGeneric.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Creates the constructor.
        /// </summary>
        /// <param name="genericTypeDefinition">The generic type definition.</param>
        /// <param name="requestedGenericType">The requestested generic type.</param>
        /// <param name="argumentTypes">The types of the constructor arguments.</param>
        /// <returns>A ConstructorInvoker to create an instance of the requested generic type</returns>
        private ConstructorInvoker CreateConstructor(Type genericTypeDefinition, Type requestedGenericType, Type[] argumentTypes)
        {
            Contract.Requires(argumentTypes != null, "argumentTypes is null.");
            Contract.Requires(requestedGenericType != null, "requestedGenericType is null.");
            Contract.Requires(genericTypeDefinition != null, "genericTypeDefinition is null.");
            Contract.Requires(genericTypeDefinition.IsGenericTypeDefinition);
            Contract.Ensures(Contract.Result<ConstructorInvoker>() != null);

            Type constructorableGenericType = genericTypeDefinition.MakeGenericType(requestedGenericType.GetGenericArguments());
            ConstructorInfo ctor = constructorableGenericType.GetConstructor(argumentTypes);
            return ctor.DelegateForCreateInstance();
        }

        /// <summary>
        /// Creates the generic instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="args">The constructor arguments.</param>
        /// <returns>An instance of <typeparamref name="TInterface"/></returns>
        private TInterface CreateGenericInstance<TInterface>(string key, params object[] args)
        {
            Contract.Requires(key != null);
            Contract.Requires(typeof(TInterface).IsGenericType);
            Contract.Ensures(Contract.Result<TInterface>() != null);

            Type[] argTypes = Type.GetTypeArray(args);
            Type tInterfaceType = typeof(TInterface);
            Type genericTypeDefinition = tInterfaceType.GetGenericTypeDefinition();
            KeyValuePair<Type, string> cacheKeyPair = GetPair(tInterfaceType, key);

            Dictionary<Type[], ConstructorInvoker> constructors = null;
            if (!this.mGenericConstructors.TryGetValue(cacheKeyPair, out constructors))
            {
                constructors = new Dictionary<Type[], ConstructorInvoker>(new TypeArrayComparer());
                this.mGenericConstructors.Add(cacheKeyPair, constructors);
            }

            ConstructorInvoker constructor = null;
            if (!constructors.TryGetValue(argTypes, out constructor))
            {
                Contract.Assume(genericTypeDefinition != null);
                Type storedGenericTypeDefinition = (Type)this.mGenericTypeRelations[GetPair(genericTypeDefinition, key)];
                Contract.Assume(storedGenericTypeDefinition != null, "There is no stored generic type definition for: " + genericTypeDefinition);
                constructor = CreateConstructor(storedGenericTypeDefinition, tInterfaceType, argTypes);
                constructors.Add(argTypes, constructor);
            }

            Contract.Assume(constructor != null, "Could not find the cache'd constructor, nor could a constructor be created.");

            return (TInterface)constructor(args);
        }

        /// <summary>
        /// Setups the generic dependency.
        /// </summary>
        /// <param name="concreteType">Type of the concrete.</param>
        /// <param name="abstractType">Type of the abstract.</param>
        /// <param name="key">The key.</param>
        private void SetupGenericDependency(Type concreteType, Type abstractType, string key = null)
        {
            Contract.Requires(concreteType != null);
            Contract.Requires(abstractType != null);
            Contract.Requires(concreteType.IsGenericTypeDefinition);
            Contract.Requires(abstractType.IsGenericTypeDefinition);
            Contract.Assume(this.IsSubclassOfRawGeneric(concreteType, abstractType)
                            || concreteType.GetInterfaces()
                                .Select(iface => iface.GUID)
                                .Contains(abstractType.GUID));

            key = key ?? this.DefaultKey ?? "default";
            this.mGenericTypeRelations.Add(new KeyValuePair<Type, string>(abstractType, key), concreteType);
        }

        /// <summary>
        /// Invariant contract method.
        /// </summary>
        [ContractInvariantMethod]
        private void InvariantMethod()
        {
            Contract.Invariant(this.mSingletons != null);
            Contract.Invariant(this.mConfiguration != null);
            Contract.Invariant(this.mConstructors != null);
        }
    }
}