using Microsoft.WindowsAzure.ServiceRuntime;

namespace ConfigurationUtils.Azure
{
    public class RoleConfigurationReader : IConfigurationReader
    {
        public string GetSettingValue(string settingKey)
        {
            return RoleEnvironment.GetConfigurationSettingValue(settingKey);
        }
    }
}
