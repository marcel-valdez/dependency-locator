// ----------------------------------------------------------------------
// <copyright file="TypeArrayComparer.cs" company="Route Manager de México">
//     Copyright Route Manager de México(c) 2011. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
namespace CommonUtilities.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Clase utilizada para comparar 2 arreglos de tipos.
    /// </summary>
    public class TypeArrayComparer : IEqualityComparer<Type[]>
    {
        /// <summary>
        /// Compared the array X with the array Y
        /// </summary>
        /// <param name="x">The array X.</param>
        /// <param name="y">The array Y.</param>
        /// <returns>true if they are equal, false otherwise.</returns>
        public bool Equals(Type[] x, Type[] y)
        {
            return !(x == null || y == null) &&
                (object.ReferenceEquals(x, y) ||
                x.Except(y).Count() == 0);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(Type[] obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else if (obj.Length == 0)
            {
                // If length is 0, return 1.
                return 1;
            }
            else
            {
                return obj.GetArrayHashCode();
            }
        }
    }
}