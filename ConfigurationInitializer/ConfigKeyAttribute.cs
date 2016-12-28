using System;

namespace ConfigurationInitializer
{
    /// <summary>
    /// Defines target property key in the config files.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ConfigKeyAttribute : Attribute
    {
        public string Key { get; private set; }

        public ConfigKeyAttribute(string configKey)
        {
            this.Key = configKey;
        }
    }
}
