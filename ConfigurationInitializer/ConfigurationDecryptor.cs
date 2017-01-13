using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Text.RegularExpressions;
using EncryptDecryptUtils;

namespace ConfigurationUtils
{
    public class ConfigurationDecryptor
    {
        private static readonly Regex Base64Regex = new Regex("(?:[A-Za-z0-9+/]{4}){20,}(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?");
        private readonly ICryptoProvider _cryptoProvider;

        public ConfigurationDecryptor()
        {
            _cryptoProvider = new CryptoProvider();
        }

        public string DecryptAllSecrets(string encryptedValue)
        {
            if (encryptedValue == null) return null;

            var decryptedValue = Base64Regex.Replace(encryptedValue, match => DecryptBase64StringIfPossible(match.Value));
            return decryptedValue;
        }

        private string DecryptBase64StringIfPossible(string base64String)
        {
            EnvelopedCms envelopedCms;
            if (!_cryptoProvider.TryParseEnvelopedCms(base64String, out envelopedCms))
            {
                // base64 string is not a valid envelopedCms.
                // No secret to decrypt.
                return base64String;
            }

            try
            {
                return _cryptoProvider.Decrypt(envelopedCms);
            }
            catch (CryptographicException ex)
            {
                Trace.TraceError($"Failed to decrypt secret with exception {ex}");
                throw;
            }
        }
    }
}
