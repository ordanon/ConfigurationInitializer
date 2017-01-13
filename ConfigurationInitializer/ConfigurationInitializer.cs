using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ConfigurationUtils
{
    /// <summary>
    /// Uses properties attributes in order to initialize inherited configuration classs from the configuration files.
    /// </summary>
    public class ConfigurationInitializer
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly ConfigurationDecryptor _configurationDecryptor;

        /// <summary>
        /// ConfigurationInitializer Ctor.
        /// </summary>
        protected ConfigurationInitializer() : this(new AppSettingsConfigurationReader())
        {
        }

        /// <summary>
        /// ConfigurationInitializer Ctor.
        /// </summary>
        /// <param name="configurationReader">Reads values from the configuration files</param>
        protected ConfigurationInitializer(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
            _configurationDecryptor = new ConfigurationDecryptor();

            PopulatePropertiesFromConfiguration();
        }

        /// <summary>
        /// Update all the writeable properties in the class.
        /// </summary>
        private void PopulatePropertiesFromConfiguration()
        {
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties().Where(p => p.CanWrite))
            {
                string configKey = GetPropertyConfigKey(propertyInfo);

                try
                {
                    string rawSettingValue = _configurationReader.GetSettingValue(configKey);

                    string decryptedValue = _configurationDecryptor.DecryptAllSecrets(rawSettingValue);

                    object convertedObject = ConvertValueToPropertyType(propertyInfo, decryptedValue);

                    propertyInfo.SetValue(this, convertedObject, null);
                }
                catch (Exception ex)
                {
                    throw new FieldInitializationFailureException($"Failed initializing property: {propertyInfo.Name}, " +
                                                                  $"from config key: {configKey}, with exception: {ex}", ex);
                }
            }
        }

        private string GetPropertyConfigKey(PropertyInfo propertyInfo)
        {
            var configKeyAttribute = GetAttribute<ConfigKeyAttribute>(propertyInfo);

            if (configKeyAttribute != null)
            {
                return configKeyAttribute.Key;
            }

            return propertyInfo.Name;
        }

        private static T GetAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
        {
            return propertyInfo.GetCustomAttributes(typeof(T), inherit: true).FirstOrDefault() as T;
        }

        private object ConvertValueToPropertyType(PropertyInfo propertyInfo, string settingValue)
        {
            if (propertyInfo.PropertyType.IsInstanceOfType(settingValue))
            {
                return settingValue;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    return converter.ConvertFrom(settingValue);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Setting {propertyInfo.Name} : Conversion of [{settingValue}] to {propertyInfo.PropertyType} is not supported", ex);
                }
            }

            return JsonConvert.DeserializeObject(settingValue, propertyInfo.PropertyType);
        }
    }
}
