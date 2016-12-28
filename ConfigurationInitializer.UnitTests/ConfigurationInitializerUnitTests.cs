using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConfigurationInitializer.UnitTests
{
    [TestClass]
    public class ConfigurationInitializerUnitTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var config = new TestConfiguration();

            Assert.AreEqual(3, config.Students.Count);
            Assert.AreEqual("De-Shalit", config.SchoolName);
            Assert.AreEqual(TimeSpan.FromMinutes(5), config.BreakPeriod);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void WhenEncrypted()
        {
            var customSettings = new Dictionary<string, string>()
            {
                { "SchoolName","MIICJwYJKoZIhvcNAQcDoIICGDCCAhQCAQAxggHAMIIBvAIBADCBozCBizELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEVMBMGA1UECxMMTWljcm9zb2Z0IElUMR4wHAYDVQQDExVNaWNyb3NvZnQgSVQgU1NMIFNIQTICE1oABbnBxTx24nVqf9YAAQAFucEwDQYJKoZIhvcNAQEBBQAEggEACOq1WacsiTfVLVt3+ChXONN7ISwB1p7HcKEFRWEZVVqdV9+qgS2XqQ2wo0dwmq+jcAzY8sl+2Q8R6+iLPUBH0kbZQoXbRjF2kKJMZZeOIjgFfpgcf8p98kuQS0Unx53Bl2ZbgNDralytz1TkBjz2CY71xJzokYuAbQOmW7iM/soOIGobBe5DmcNkxvotpqeqbn3+rS3vP/RYakoAvtOZXEZZn0S/BFeBZmUy5Hfxi8ncMruIcJT2xpykO3meTpH2Ci76qlcT8ldCSpbaPOET3Q23IlPZOGpqU+iFfeAijrBc3XJ3tTFoRt72LZkecyFfzY0eIl0slCSW0toPU0Ec3zBLBgkqhkiG9w0BBwEwFAYIKoZIhvcNAwcECCKOfRB+EE79gCjchYEHnOEqy9LqX77DIa+e2J6VLtMX6dZYNS3e1jPlrlBrwk08GK3G"}
            };

            var configurationReaderMock = GetConfigurationReaderMock(customSettings);

            var config = new TestConfiguration(configurationReaderMock.Object);
        }

        private Mock<IConfigurationReader> GetConfigurationReaderMock(Dictionary<string, string> customSettings)
        {
            var configurationReaderMock = new Mock<IConfigurationReader>();

            // Set custom behavior for selected settings.
            foreach (var customSetting in customSettings)
            {
                configurationReaderMock.Setup(r => r.GetSettingValue(customSetting.Key)).Returns(customSetting.Value);
            }

            // Set default behavior for non custom settings.
            var keys = customSettings.Keys.ToArray();

            configurationReaderMock.Setup(r => r.GetSettingValue(It.IsNotIn(keys)))
                .Returns<string>(key => ConfigurationManager.AppSettings.Get(key));

            return configurationReaderMock;
        }
    }
}
