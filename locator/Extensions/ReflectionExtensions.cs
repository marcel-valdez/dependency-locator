namespace DependencyLocation.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        /// <summary>
        /// Determines whether [is subclass or implements base generic] [the specified sub generic].
        /// </summary>
        /// <param name="subGeneric">The generic type.</param>
        /// <param name="baseGeneric">The base generic.</param>
        /// <returns>
        ///   <c>true</c> if [is subclass of raw generic] [the specified sub generic]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSubclassOrImplementsBaseGeneric(this Type subGeneric, Type baseGeneric)
        {
            Contract.Requires(subGeneric != null, "subGeneric is null.");
            Contract.Requires(baseGeneric != null, "baseGeneric is null.");
            Contract.Requires(baseGeneric.IsGenericType);

            if (subGeneric.GetInterfaces()
                .Select(iface => iface.GUID)
                .Contains(baseGeneric.GUID))
            {
                return true;
            }

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
        /// Gets the parameter types of a constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The parameter types</returns>
        public static Type[] GetParamTypes(this ConstructorInfo constructor)
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
    }
}