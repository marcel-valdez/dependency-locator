namespace DependencyLocation.Configuration
{
    using System.Configuration;

    public class DependencyElement : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyElement"/> class.
        /// </summary>
        public DependencyElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyElement"/> class.
        /// </summary>
        /// <param name="assembly name">The assembly name.</param>
        /// <param name="assembly path">The assembly path.</param>
        /// <param name="named instanc eprefix">The named instance prefix.</param>
        public DependencyElement(string assemblyname, string assemblypath = "", string namedinstanceprefix = "")
        {
            this.AssemblyName = assemblyname;
            this.AssemblyPath = assemblypath;
            this.NamedInstancePrefix = namedinstanceprefix;
        }

        [ConfigurationProperty("assemblyName", DefaultValue = "Arial", IsRequired = true, IsKey = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\")]
        public string AssemblyName
        {
            get
            {
                return (string)this["assemblyName"];
            }
            set
            {
                this["assemblyName"] = value;
            }
        }

        [ConfigurationProperty("assemblyPath", DefaultValue = (object)"", IsRequired = false)]
        [StringValidator(InvalidCharacters = "$%^&*()[]{};'\"|", MinLength = 0, MaxLength = 256)]
        public string AssemblyPath
        {
            get
            {
                return (string)this["assemblyPath"];
            }
            set
            {
                this["assemblyPath"] = value;
            }
        }

        [ConfigurationProperty("namedInstancePrefix", DefaultValue = (object)"", IsRequired = false)]
        public string NamedInstancePrefix
        {
            get
            {
                return (string)this["namedInstancePrefix"];
            }
            set
            {
                this["namedInstancePrefix"] = value;
            }
        }
    }
}
