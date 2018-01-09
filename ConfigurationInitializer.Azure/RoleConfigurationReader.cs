using Microsoft.WindowsAzure.ServiceRuntime;
using System.Configuration;

namespace ConfigurationUtils.Azure
{
    public class RoleConfigurationReader : IConfigurationReader
    {
        public string GetSettingValue(string settingKey)
        {
            if (RoleEnvironment.IsAvailable)
            {
                return RoleEnvironment.GetConfigurationSettingValue(settingKey);
            }
            else
            {
                return ConfigurationManager.AppSettings[settingKey];
            }
        }
    }
}
