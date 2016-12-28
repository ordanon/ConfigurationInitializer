using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ConfigurationInitializer.Azure
{
    public class RoleConfigurationReader : IConfigurationReader
    {
        public string GetSettingValue(string settingKey)
        {
            return RoleEnvironment.GetConfigurationSettingValue(settingKey);
        }
    }
}
