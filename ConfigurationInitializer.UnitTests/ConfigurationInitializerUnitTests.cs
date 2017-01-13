using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConfigurationUtils.ConfigurationInitializer.UnitTests
{
    [TestClass]
    public class ConfigurationInitializerUnitTests
    {
        [TestMethod]
        public void ValidateGoodFlow()
        {
            var config = new TestConfiguration();

            Assert.AreEqual(3, config.Students.Count);
            Assert.AreEqual("De-Shalit", config.SchoolName);
            Assert.AreEqual(TimeSpan.FromMinutes(5), config.BreakPeriod);
        }

        [TestMethod]
        [ExpectedException(typeof(FieldInitializationFailureException))]
        public void WhenConfigSettingIsEncryptedAndRelevantCertificateIsMissingExceptionIsThrown()
        {
            var customSettings = new Dictionary<string, string>()
            {
                { "SchoolName","MIIB/QYJKoZIhvcNAQcDoIIB7jCCAeoCAQAxggG2MIIBsgIBADCBmTCBgTETMBEGCgmSJomT8ixkARkWA2NvbTEZMBcGCgmSJomT8ixkARkWCW1pY3Jvc29mdDEUMBIGCgmSJomT8ixkARkWBGNvcnAxFzAVBgoJkiaJk/IsZAEZFgdyZWRtb25kMSAwHgYDVQQDExdNU0lUIFRlc3QgQ29kZVNpZ24gQ0EgNgITHQBU1cky1/xqD6sIvQABAFTVyTANBgkqhkiG9w0BAQEFAASCAQA7fAqWCs/nEhDbA7ythU3IvvM7M7ihLLhSEDENG7cr+vhHPoI2OidgCt2o7NemEOgM2duwbIXxdO6XyN2D76zUS//8NvVkr9y4KA0+KQCcA7UpldL/cQhFZvJFGyUdqxJp4e27HxvJgwFhx3Rx0w0d3qO/gxlXxlrThoOKzGdjOVnU/uzo6UDSLWJveNIV9XOpAUVEIo0J+sgZsE49nG1cdf1oClN742gZKl7oKwdB7x/LLsxWywAMsZFPC0VgFIrBM0EWeBrW6uSpQV1bznLDYibds+rjRqREpFvZip25w3ls+0khNrRwe3FllRibmmvIJj440/Fi3nKcTTimvOV6MCsGCSqGSIb3DQEHATAUBggqhkiG9w0DBwQI/ze6ikl+g92ACGEgsiWnL/AD"}
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
