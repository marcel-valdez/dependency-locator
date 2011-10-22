// ----------------------------------------------------------------------
// <copyright file="DependencyConfigurationSection.cs" company="Route Manager de México">
//     Copyright Route Manager de México(c) 2011. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
namespace DependencyLocation.Configuration
{
    using System.Configuration;

    public class DependencyConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("defaultKey", DefaultValue = "default", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string DefaultKey
        {
            get
            {
                return (string)this["defaultKey"];
            }

            set
            {
                this["defaultKey"] = value;
            }
        }

        [ConfigurationProperty("dependencies", IsDefaultCollection = false),
        ConfigurationCollection(typeof(DependencyCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public DependencyCollection Dependencies
        {
            get
            {
                return (DependencyCollection)this["dependencies"];
            }

            set
            {
                this["dependencies"] = value;
            }
        }
    }
}