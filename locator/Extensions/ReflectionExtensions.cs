namespace CommonUtilities.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

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
    }
}
