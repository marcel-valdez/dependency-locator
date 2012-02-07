namespace DependencyLocation.DependencyLocation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using CommonUtilities.Reflection;
    using Fasterflect;

    /// <summary>
    /// This class is in charge of containing and giving access to constructors of a concrete type
    /// that satisfies a given interface
    /// </summary>
    internal class InterfaceConstructors
    {
        private readonly Type mInterfaceType;
        private readonly Dictionary<Type[], ConstructorInvoker> mConstructors;
        private Type mConcreteType;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceConstructors"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        public InterfaceConstructors(Type interfaceType)
        {
            Contract.Requires(interfaceType != null);
            this.mInterfaceType = interfaceType;
            this.mConstructors = new Dictionary<Type[], ConstructorInvoker>(new TypeArrayComparer());
        }

        /// <summary>
        /// Sets the concrete implementation of the interface
        /// </summary>
        /// <typeparam name="T">Concrete type of the interface implementation</typeparam>
        public void SetConcrete<T>()
        {
            Contract.Requires(this.GetInterface().IsAssignableFrom(typeof(T)));
            this.SetConcrete(typeof(T));
        }

        /// <summary>
        /// Sets the concrete implementation of the interface
        /// </summary>
        /// <param name="concreteType">Concrete type of the interface implementation</param>
        public void SetConcrete(Type concreteType)
        {
            Contract.Requires(this.GetInterface().IsAssignableFrom(concreteType));
            this.mConcreteType = concreteType;
            this.ClearConstructors();
            this.DefineConstructors();
        }

        /// <summary>
        /// Verifies if the interface satisfied by these constructors is of type T
        /// </summary>
        /// <typeparam name="T">Type of interface satisfied</typeparam>
        /// <returns>
        ///   <c>true</c> if this instance is type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsType<T>()
        {
            return typeof(T).Equals(this.mInterfaceType);
        }

        /// <summary>
        /// Verifies if the interface satisfied by these constructors is of type 'type'
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsType(Type type)
        {
            Contract.Requires(type != null);
            return type.Equals(this.mInterfaceType);
        }

        /// <summary>
        /// Gets the interface for which the constructors are defined
        /// </summary>
        /// <returns></returns>
        public Type GetInterface()
        {
            Contract.Ensures(Contract.Result<Type>() != null);
            return this.mInterfaceType;
        }

        /// <summary>
        /// Tries to get a constructor that can be called with the given parameter types
        /// </summary>
        /// <param name="ctor">The ctor.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool TryGetConstructor(out ConstructorInvoker ctor, params Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        // Tries to get a constructor that can be called with the given parameters
        /// </summary>
        /// <param name="ctor">The ctor.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public bool TryGetConstructor(out ConstructorInvoker ctor, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        private void DefineConstructors()
        {

        }

        private void ClearConstructors()
        {

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
        private ConstructorInvoker GetMatchingConstructor(Type[] argTypes)
        {
            Contract.Requires(argTypes != null, "argumentTypes is null or empty.");
            Contract.Ensures(Contract.Result<ConstructorInvoker>() != null);

            Type[] ctorArgsTypes =
                            this.mConstructors.Keys
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

            return this.mConstructors[ctorArgsTypes];
        }
    }

    internal class InterfaceConstructors<T> : InterfaceConstructors
    {
        public InterfaceConstructors()
            : base(typeof(T))
        {
        }
    }
}
