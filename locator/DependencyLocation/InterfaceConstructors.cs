namespace DependencyLocation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using Reflection;
    using Extensions;
    using Fasterflect;

    /// <summary>
    /// This class is in charge of containing and giving access to constructors of a concrete type
    /// that satisfies a given interface
    /// </summary>
    internal class InterfaceConstructors
    {
        private readonly Type mInterfaceType;
        private readonly Dictionary<Type[], ConstructorInvoker> mConstructors;

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
        public InterfaceConstructors SetConcrete<T>()
        {
            Contract.Requires(this.GetInterface().IsAssignableFrom(typeof(T)));
            Contract.Ensures(Contract.Result<InterfaceConstructors>() == this);
            return this.SetConcrete(typeof(T));
        }

        /// <summary>
        /// Sets the concrete implementation of the interface
        /// </summary>
        /// <param name="concreteType">Concrete type of the interface implementation</param>
        public InterfaceConstructors SetConcrete(Type concreteType)
        {
            Contract.Requires(this.GetInterface().IsAssignableFrom(concreteType));
            Contract.Ensures(Contract.Result<InterfaceConstructors>() == this);
            return this.ClearConstructors()
                       .DefineConstructors(concreteType);
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
            Contract.Requires(parameterTypes != null);

            ctor = this.mConstructors.FirstOrDefault(current => current.Key.EqualsTo(parameterTypes)).Value ??
                   this.mConstructors.FirstOrDefault(current => current.Key.IsAssignableFrom(parameterTypes)).Value;

            return ctor != null;
        }

        /// <summary>
        // Tries to get a constructor that can be called with the given parameters
        /// </summary>
        /// <param name="ctor">The ctor.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public bool TryGetConstructor(out ConstructorInvoker ctor, params object[] parameters)
        {
            Contract.Requires(parameters != null);
            return this.TryGetConstructor(out ctor, parameters.Length == 0 ? Type.EmptyTypes : Type.GetTypeArray(parameters));
        }

        /// <summary>
        /// Defines and stores in memory the constructors for the type concreteType.
        /// </summary>
        private InterfaceConstructors DefineConstructors(Type concreteType)
        {
            Contract.Requires(concreteType != null);
            Contract.Requires(!concreteType.IsAbstract, "Concrete type can't be abstract.");
            Contract.Ensures(Contract.Result<InterfaceConstructors>() == this);
            foreach (ConstructorInfo ctor in concreteType.GetConstructors())
            {
                this.AddConstructor(ctor);
            }

            return this;
        }

        /// <summary>
        /// Clears the constructors.
        /// </summary>
        private InterfaceConstructors ClearConstructors()
        {
            Contract.Ensures(Contract.Result<InterfaceConstructors>() == this);
            this.mConstructors.Clear();
            return this;
        }

        /// <summary>
        /// Adds the constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="target">The target.</param>
        /// <param name="key">The key.</param>
        private InterfaceConstructors AddConstructor(ConstructorInfo constructor)
        {
            Contract.Requires(constructor != null);
            Contract.Ensures(Contract.Result<InterfaceConstructors>() == this);

            Type[] lParamTypes = constructor.GetParamTypes();
            if (lParamTypes.Length == 0)
            {
                lParamTypes = Type.EmptyTypes;
            }

            ConstructorInvoker lInvoker = null;
            lInvoker = constructor.DelegateForCreateInstance();
            this.mConstructors.Add(lParamTypes, lInvoker);
            return this;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            bool lEquals = true;
            if (!base.Equals(other))
            {
                if (other as InterfaceConstructors != null)
                {
                    Dictionary<Type[], ConstructorInvoker> lOtherCtors = (other as InterfaceConstructors).mConstructors;
                    lEquals = this.mConstructors.Count == lOtherCtors.Count;
                    TypeArrayComparer comparer = new TypeArrayComparer();
                    for (int i = 0; lEquals && i < this.mConstructors.Count; i++)
                    {
                        Type[] xKey = this.mConstructors.Keys.ElementAt(i);
                        Type[] yKey = lOtherCtors.Keys.ElementAt(i);
                        lEquals = comparer.Equals(xKey, yKey);
                    }
                }
            }

            return lEquals;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.GetInterface().GetHashCode() * 7;
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
