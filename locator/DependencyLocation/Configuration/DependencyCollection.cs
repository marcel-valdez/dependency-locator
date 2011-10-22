// ----------------------------------------------------------------------
// <copyright file="DependencyCollection.cs" company="Route Manager de México">
//     Copyright Route Manager de México(c) 2011. All rights reserved.
// </copyright>
using System.Diagnostics.Contracts;
// ------------------------------------------------------------------------
namespace DependencyLocation.Configuration
{
    using System.Collections;
    using System.Configuration;

    /// <summary>
    /// Colección de elementos de configuración de dependencias
    /// </summary>
    public class DependencyCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyCollection"/> class.
        /// </summary>
        protected DependencyCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyCollection"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="T:System.Collections.IComparer"/> comparer to use.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="comparer"/> is null.</exception>
        protected DependencyCollection(IComparer comparer)
            : base(comparer)
        {
            Contract.Requires(comparer != null, "comparer is null.");
        }

        /// <summary>
        /// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <returns>The <see cref="T:System.Configuration.ConfigurationElementCollectionType"/> of this collection.</returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            var dependency = new DependencyElement();
            return dependency;
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as DependencyElement).AssemblyName;
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(DependencyElement element)
        {
            Contract.Requires(element != null, "element is null.");
            base.BaseAdd(element);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            base.BaseClear();
        }

        /// <summary>
        /// Removes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Remove(DependencyElement element)
        {
            base.BaseRemove(element.AssemblyName);
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        /// <summary>
        /// Removes at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }
    }
}
