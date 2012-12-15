namespace DependencyLocation.Containers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Extensions;
    using Fasterflect;

    /// <summary>
    /// Esta clase esta encargada de contener y proveer acceso a constructores de instancias
    /// </summary>
    internal class ConstructorContainer
    {
        private readonly IList<InterfaceConstructorsContainer> mCtorsList =
                                    new List<InterfaceConstructorsContainer>();

        /// <summary>
        /// Adds the interface constructors, using the signature of the constructors of the concrete type.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="concreteType">Type of the concrete.</param>
        /// <returns>Itself</returns>
        public ConstructorContainer AddInterfaceConstructors(Type interfaceType, Type concreteType)
        {
            Contract.Requires(interfaceType != null, "interfaceType is null.");
            Contract.Requires(concreteType != null, "concreteType is null.");

            this.mCtorsList.Add(new InterfaceConstructorsContainer(interfaceType)
                            .SetConcrete(concreteType));

            return this;
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="argTypes">The argument types.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="key">The key.</param>
        /// <returns>The callable constructor delegate</returns>
        public ConstructorInvoker GetConstructor(Type[] argTypes, Type interfaceType)
        {
            Contract.Requires(argTypes != null, "argTypes is null.");
            Contract.Requires(interfaceType != null, "interfaceType is null.");

            InterfaceConstructorsContainer ctors = this.mCtorsList.FirstOrDefault(cCtors => cCtors.IsType(interfaceType));
            if (ctors != null)
            {
                return GetMatchingConstructor(ctors, argTypes);
            }

            throw new KeyNotFoundException(string.Format("No esta registrado el tipo de interfaz {0}.", interfaceType.ToString()));
        }

        /// <summary>
        /// Determines whether the specified interface type has been registered.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>
        ///   <c>true</c> if the specified interface type has been registered; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRegistered(Type interfaceType)
        {
            Contract.Requires(interfaceType != null);
            return this.mCtorsList.Any(cCtors => cCtors.IsType(interfaceType));
        }

        /// <summary>
        /// Matches the types.
        /// </summary>
        /// <param name="constructors">The constructors.</param>
        /// <param name="argTypes">The arg types.</param>
        /// <returns>ConstructorInvoker that matches the <paramref name="argTypes"/></returns>
        private static ConstructorInvoker GetMatchingConstructor(InterfaceConstructorsContainer constructors, Type[] argTypes)
        {
            Contract.Requires(constructors != null, "constructors is null.");
            Contract.Requires(argTypes != null, "argumentTypes is null or empty.");
            Contract.Ensures(Contract.Result<ConstructorInvoker>() != null);

            ConstructorInvoker ctor = null;
            if (!constructors.TryGetConstructor(out ctor, argTypes) || ctor == null)
            {
                throw new KeyNotFoundException(MakeErrorMsg(constructors, argTypes));
            }

            return ctor;
        }

        /// <summary>
        /// Makes the error message.
        /// </summary>
        /// <param name="constructors">The constructors.</param>
        /// <param name="argTypes">The arg types.</param>
        /// <returns>The error message</returns>
        private static string MakeErrorMsg(InterfaceConstructorsContainer constructors, Type[] argTypes)
        {
            Contract.Requires(constructors != null, "constructors is null.");
            Contract.Requires(argTypes != null, "argTypes is null or empty.");
            Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<string>()));

            StringBuilder lParamTypesMsgBuilder = new StringBuilder();
            lParamTypesMsgBuilder.Append("Un constructor para el tipo {0} con los parámetros: ");
            object[] msgObjects = new object[argTypes.Length + 1];
            msgObjects[0] = constructors.GetInterface();
            if (argTypes.Length == 0)
            {
                lParamTypesMsgBuilder.Append("Sin parámetros, ");
            }
            else
            {
                for (int i = 0; i < argTypes.Length; i++)
                {
                    msgObjects[i + 1] = argTypes[i];
                    lParamTypesMsgBuilder.Append("{" + (i + 1) + "}, ");
                }
            }

            lParamTypesMsgBuilder.Append("no existe.");

            return string.Format(lParamTypesMsgBuilder.ToString(), msgObjects);
        }

    }
}