// ----------------------------------------------------------------------
// <copyright file="DependencyLoader.cs" company="Route Manager de México">
//     Copyright Route Manager de México(c) 2011. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
namespace DependencyLocation.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using Fasterflect;

    /// <summary>
    /// Clase para cargar todas las dependencias en el dominio de la aplicación en ejecución.
    /// Cada librería que desee registrarse en el DependencyInjector, debe tener una o más clases
    /// que implementen <typeparamref name="IDependencySetup"/>
    /// </summary>
    public class DependencyLoader
    {
        private const string CONFIGSECTIONPATH = @"dependencyInjector/dependencyConfiguration";
        private readonly static DependencyLoader loader = new DependencyLoader();

        /// <summary>
        /// Gets the loader.
        /// </summary>
        public static DependencyLoader Loader
        {
            get
            {
                return loader;
            }
        }

        /// <summary>
        /// Loads the dependencies.
        /// </summary>
        public void LoadDependencies(string configfilepath = null)
        {
            DependencyConfiguration configSection = GetConfigSection(configfilepath);
            string defaultKey = configSection.DefaultKey;
            DependencyController injector = Dependency.Locator as DependencyController;

            injector.DefaultKey = defaultKey;

            foreach (DependencyElement element in this.GetDependencies(configSection))
            {
                string name = element.AssemblyName;
                string path = element.AssemblyPath;
                string prefix = string.IsNullOrEmpty(element.NamedInstancePrefix) ? "" : element.NamedInstancePrefix + ".";
                Assembly assembly = LoadAssembly(name, path);
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IDependencySetup).IsAssignableFrom(type))
                    {
                        IDependencySetup setup = (IDependencySetup)type.CreateInstance();
                        setup.SetupDependencies(injector, prefix, defaultKey);
                    }
                }
            }
        }

        /// <summary>
        /// Loads an assembly, by using either its name or its path.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="path">The path.</param>
        /// <returns>The loaded Assembly</returns>
        private static Assembly LoadAssembly(string name, string path)
        {
            Contract.Requires(!String.IsNullOrEmpty(name) || !String.IsNullOrEmpty(path), "both path and name are null or empty.");

            AssemblyName assemblyName = null;
            if (string.IsNullOrEmpty(path))
            {
                assemblyName = new AssemblyName(name);
            }
            else
            {
                assemblyName = AssemblyName.GetAssemblyName(path);
            }

            return Assembly.Load(assemblyName);
        }

        /// <summary>
        /// Gets the dependencies in the configuration
        /// </summary>
        /// <returns>The dependencies</returns>
        /// <param name="configSection"></param>
        private IEnumerable<DependencyElement> GetDependencies(DependencyConfiguration configSection)
        {
            if (configSection != null)
            {
                foreach (DependencyElement element in configSection.Dependencies)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Gets the config section.
        /// </summary>
        /// <returns></returns>
        private DependencyConfiguration GetConfigSection(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                return ConfigurationManager.GetSection(CONFIGSECTIONPATH) as DependencyConfiguration;
            }
            else
            {
                // Read the configuration from the path
                ExeConfigurationFileMap fileMap = ReadConfigFileMap(configFilePath);
                // Open the configuration and get the section
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                ConfigurationSection section = config.GetSection(CONFIGSECTIONPATH);
                return (DependencyConfiguration)section;
            }
        }

        protected virtual ExeConfigurationFileMap ReadConfigFileMap(string configFilePath)
        {
            Contract.Requires(!string.IsNullOrEmpty(configFilePath), "El nombre de archivo no debe ser vacío o nulo.");
            Contract.Assume(new FileInfo(configFilePath).Exists,
                string.Format(Properties.Resources.ConfigFileNotFound, configFilePath));
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFilePath
            };

            return fileMap;
        }
    }
}