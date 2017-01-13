using System.Configuration;

namespace ConfigurationUtils
{
    public interface IConfigurationReader
    {
        string GetSettingValue(string settingKey);
    }

    public class AppSettingsConfigurationReader : IConfigurationReader
    {
        public string GetSettingValue(string settingKey)
        {
            return ConfigurationManager.AppSettings.Get(settingKey);
        }
    }
}
