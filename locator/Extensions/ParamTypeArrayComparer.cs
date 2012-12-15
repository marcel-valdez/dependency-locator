namespace DependencyLocation.Extensions
{
    using System;
    using System.Diagnostics.Contracts;

    public static class ParamTypeArrayComparer
    {
        /// <summary>
        /// Determines whether the specified [toTypes] are assignable from [fromTypes].
        /// </summary>
        /// <param name="toTypes">To types.</param>
        /// <param name="fromTypes">From types.</param>
        /// <returns>
        /// <c>true</c> if the specified "to types" are assignable "from types"; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAssignableFrom(this Type[] toTypes, Type[] fromTypes)
        {
            Contract.Requires(toTypes != null, "toTypes is null.");
            Contract.Requires(fromTypes != null, "fromTypes is null.");

            bool equals = toTypes != null && fromTypes != null && toTypes.Length == fromTypes.Length;
            if (equals)
            {
                for (int i = 0; i < toTypes.Length && equals; i++)
                {
                    equals = toTypes[i].IsAssignableFrom(fromTypes[i]);
                }
            }

            return equals;
        }

        /// <summary>
        /// Compared the array X with the array Y
        /// </summary>
        /// <param name="x">The array X.</param>
        /// <param name="y">The array Y.</param>
        /// <returns>true if they are equal, false otherwise.</returns>
        public static bool EqualsTo(this Type[] x, Type[] y)
        {
            Contract.Requires(x != null, "x is null.");
            Contract.Requires(y != null, "y is null.");

            bool equals = x != null && y != null && x.Length == y.Length;
            if (equals)
            {
                for (int i = 0; i < x.Length && equals; i++)
                {
                    equals = x[i].Equals(y[i]);
                }
            }

            return equals;
        }
    }
}
