namespace DependencyLocation
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using Fasterflect;
    using System.Linq;
    using CommonUtilities.Reflection;
    using System.Reflection;

    /// <summary>
    /// Esta clase esta encargada de contener y proveer acceso a constructores de instancias
    /// </summary>
    internal class ConstructorContainer
    {
        private readonly Dictionary<KeyValuePair<Type, string>, Dictionary<Type[], ConstructorInvoker>> mConstructors =
                                    new Dictionary<KeyValuePair<Type, string>, Dictionary<Type[], ConstructorInvoker>>();

        /// <summary>
        /// Adds the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="target">The target type to manifest.</param>
        /// <param name="key">The key.</param>
        public void Add(ConstructorInfo constructor, Type target, string key)
        {
            Contract.Requires(target != null);
            Contract.Requires(constructor != null);
            Contract.Requires(key != null);

            Dictionary<Type[], ConstructorInvoker> constructors = null;
            if (!this.mConstructors.TryGetValue(MakePair(target, key), out constructors))
            {
                constructors = new Dictionary<Type[], ConstructorInvoker>(new TypeArrayComparer());
                this.mConstructors.Add(
                    MakePair(target, key),
                    constructors);
            }

            Type[] paramTypes = GetParamTypes(constructor);

            if (paramTypes.Length == 0)
            {
                paramTypes = Type.EmptyTypes;
            }

            ConstructorInvoker invoker = null;

            if (!constructors.TryGetValue(paramTypes, out invoker))
            {
                invoker = constructor.DelegateForCreateInstance();
                constructors.Add(paramTypes, invoker);
            }
        }

        /// <summary>
        /// Sets the specified constructor for the target type and key.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="target">The target.</param>
        /// <param name="key">The key.</param>
        public void Set(ConstructorInfo constructor, Type target, string key)
        {
            Contract.Requires(target != null);
            Contract.Requires(constructor != null);
            Contract.Requires(key != null);

            Type[] paramTypes = GetParamTypes(constructor);
            KeyValuePair<Type, string> pair = MakePair(target, key);
            var invoker = constructor.DelegateForCreateInstance();
            this.mConstructors[pair][paramTypes] = invoker;
        }

        /// <summary>
        /// Tries to set the constructor for target type and key
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="target">The target.</param>
        /// <param name="key">The key.</param>
        /// <returns>True if the target type and key have already been set (and was modified), false otherwise.</returns>
        public bool TrySet(ConstructorInfo constructor, Type target, string key)
        {
            Contract.Requires(target != null);
            Contract.Requires(constructor != null);
            Contract.Requires(key != null);

            bool success = true;
            Type[] paramTypes = GetParamTypes(constructor);
            KeyValuePair<Type, string> pair = MakePair(target, key);
            var invoker = constructor.DelegateForCreateInstance();
            try
            {
                this.mConstructors[pair][paramTypes] = invoker;
            }
            catch (KeyNotFoundException)
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="argTypes">The argument types.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="key">The key.</param>
        /// <returns>The callable constructor delegate</returns>
        public ConstructorInvoker GetConstructor(Type[] argTypes, Type interfaceType, string key)
        {
            Contract.Requires(argTypes != null, "argTypes is null.");
            Contract.Requires(interfaceType != null, "interfaceType is null.");
            Contract.Requires(!String.IsNullOrEmpty(key), "key is null or empty.");

            Dictionary<Type[], ConstructorInvoker> interfaceConstructors = null;
            if (!this.mConstructors.TryGetValue(MakePair(interfaceType, key), out interfaceConstructors) && interfaceConstructors != null)
            {
                return GetMatchingConstructor(interfaceConstructors, argTypes);
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

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
            Contract.Ensures(
                Contract.Result<KeyValuePair<Type, string>>().Key == type &&  
                Contract.Result<KeyValuePair<Type, string>>().Value == key);

            return new KeyValuePair<Type, string>(type, key);
        }

        /// <summary>
        /// Gets the parameter types of a constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The parameter types</returns>
        private static Type[] GetParamTypes(ConstructorInfo constructor)
        {
            Contract.Requires(constructor != null);
            Contract.Ensures(Contract.Result<Type[]>() != null);

            Type[] types = constructor.GetParameters()
                                    .Select(info => info.ParameterType)
                                    .ToArray();

            if (types == null || types.Length == 0)
            {
                types = Type.EmptyTypes;
            }

            return types;
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

            Type[] ctorArgsTypes =
                            constructors.Keys
                            .Where(
                            (types) =>
                            {
                                // First match the amount of arguments
                                bool match = types.Length == argTypes.Length;
                                if (match)
                                {
                                    for (int i = 0; match && i < argTypes.Length; i++)
                                    {
                                        // Now match each argument
                                        match = types[i].IsAssignableFrom(argTypes[i]);
                                    }
                                }

                                return match;
                            }).First();

            return constructors[ctorArgsTypes];
        }
    }
}