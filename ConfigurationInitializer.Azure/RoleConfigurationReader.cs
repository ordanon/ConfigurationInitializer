using Microsoft.WindowsAzure.ServiceRuntime;

namespace ConfigurationUtils.ConfigurationInitializer.Azure
{
    public class RoleConfigurationReader : IConfigurationReader
    {
        public string GetSettingValue(string settingKey)
        {
            return RoleEnvironment.GetConfigurationSettingValue(settingKey);
        }
    }
}
