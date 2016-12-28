using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.ManagementServices.ConfigurationEncryption;

namespace ConfigurationInitializer
{
    public class ConfigurationDecryptor
    {
        private static readonly Regex Base64Regex = new Regex("(?:[A-Za-z0-9+/]{4}){20,}(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?");
        private readonly ConfigurationEncryptionProvider _configurationEncryptionProvider;

        public ConfigurationDecryptor()
        {
            _configurationEncryptionProvider = new ConfigurationEncryptionProvider();
        }

        public string DecryptAllSecrets(string encryptedValue)
        {
            if (encryptedValue == null) return null;

            var decryptedValue = Base64Regex.Replace(encryptedValue, match => DecryptBase64StringIfPossible(match.Value));
            return decryptedValue;
        }

        private string DecryptBase64StringIfPossible(string base64String)
        {
            try
            {
                return _configurationEncryptionProvider.Decrypt(base64String);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Ignore exception, base64 string is not a valid envelopedCms.
                // No secret to decrypt.
                return base64String;
            }
            catch (CryptographicException ex)
            {
                Trace.TraceError($"Failed to decrypt secret with exception {ex}");
                throw;
            }
        }
    }
}
