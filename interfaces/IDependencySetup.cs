// ----------------------------------------------------------------------
// <copyright file="IDependencySetup.cs" company="Route Manager de México">
//     Copyright Route Manager de México(c) 2011. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
namespace DependencyLocation.Setup
{
    /// <summary>
    /// Interfaz de clases para inicializar las dependencias, cada 'Componente' que dependa de
    /// DependencyInjector, debe crear una clase que se llame: [Nombre De Assembly]DependencySetup
    /// que implemente esta interfaz
    /// </summary>
    public interface IDependencySetup
    {
        /// <summary>
        /// Setups the dependencies.
        /// </summary>
        /// <param name="container">The dependency container.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="defaultKey">The default key.</param>
        void SetupDependencies(IDependencyConfigurator container, string prefix, string defaultKey);
    }
}