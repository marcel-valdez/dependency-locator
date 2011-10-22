// ----------------------------------------------------------------------
// <copyright file="HashCodeHelper.cs" company="Route Manager de México">
//     Copyright Route Manager de México(c) 2011. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
namespace CommonUtilities
{
    using System;

    public static class HashCodeHelper
    {
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The args.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public static int GetHashCode<T>(params T[] args)
        {
            return args.GetArrayHashCode();
        }

        /// <summary>
        /// Gets the hash value for an array object.
        /// </summary>
        /// <typeparam name="T">Type of objects in the array</typeparam>
        /// <param name="objects">The objects.</param>
        /// <returns>int hash value</returns>
        public static int GetArrayHashCodeByByte<T>(this T[] objects)
        {
            byte[] data = new byte[objects.Length * 4];

            for (int i = 0; i < objects.Length; i++)
            {
                T obj = objects[i];

                Array datum = new byte[] { 0, 0, 0, 1 };
                if (obj != null)
                {
                    datum = BitConverter.GetBytes(obj.GetHashCode());
                    Array.Reverse(datum);
                }
                
                datum.CopyTo(data, i * 4);
            }

            return GetFnvHash(data);
        }

        /// <summary>
        /// Gets the hash value for an array object.
        /// </summary>
        /// <typeparam name="T">Type of objects in the array</typeparam>
        /// <param name="objects">The objects.</param>
        /// <returns>int hash value</returns>
        public static int GetArrayHashCode<T>(this T[] objects)
        {
            int[] data = new int[objects.Length];

            for (int i = 0; i < objects.Length; i++)
            {
                T obj = objects[i];
                data[i] = obj == null ? 1 : obj.GetHashCode();
            }

            return GetFnvHash(data);
        }

        /// <summary>
        /// Gets the FNV hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>int value for the hash</returns>
        public static int GetFnvHash(byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                uint hash = 2166136261;

                for (int i = 0; i < data.Length; i++)
                {
                    hash = (hash ^ data[i]) * p;
                }

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;

                return (int)hash;
            }
        }

        /// <summary>
        /// Gets the FNV hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>int value for the hash</returns>
        public static int GetFnvHash(int[] data)
        {
            unchecked
            {
                const int p = 16777619;
                long hash = 2166136261;

                for (int i = 0; i < data.Length; i++)
                {
                    hash = (hash ^ data[i]) * p;
                }

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;

                return (int)hash;
            }
        }
    }
}
