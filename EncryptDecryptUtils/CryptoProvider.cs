using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace EncryptDecryptUtils
{
    public interface ICryptoProvider
    {
        string Encrypt(string secret, X509Certificate2 publicKeyCertificate);

        string Decrypt(string encryptedSecret, X509Certificate2[] certificates = null);

        string Decrypt(EnvelopedCms envelopedCms, X509Certificate2[] certificates = null);

        bool TryParseEnvelopedCms(string base64EnvelopedCms, out EnvelopedCms envelopedCms);

        EnvelopedCms ParseEnvelopedCms(string base64EnvelopedCms);

        IEnumerable<X509Certificate2> FindCertificates(RecipientInfoCollection recipients, StoreLocation storeLocation, StoreName storeName);
    }

    public class CryptoProvider : ICryptoProvider
    {
        private static readonly Encoding SecretEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private readonly bool _allowNonValidCertificates;

        public CryptoProvider(bool allowNonValidCertificates = true)
        {
            _allowNonValidCertificates = allowNonValidCertificates;
        }

        public string Encrypt(string secret, X509Certificate2 publicKeyCertificate)
        {
            if (string.IsNullOrEmpty(secret)) throw new ArgumentNullException(nameof(secret));
            if (publicKeyCertificate == null) throw new ArgumentNullException(nameof(publicKeyCertificate));

            if (!_allowNonValidCertificates && !publicKeyCertificate.Verify())
            {
                throw new ArgumentException("publicKey must be valid (trusted and not expired)", nameof(publicKeyCertificate));
            }
                
            EnvelopedCms envelopedCms = new EnvelopedCms(new ContentInfo(SecretEncoding.GetBytes(secret)));
            envelopedCms.Encrypt(new CmsRecipient(publicKeyCertificate));
            return Convert.ToBase64String(envelopedCms.Encode());
        }

        public string Decrypt(string encryptedSecret, X509Certificate2[] certificates = null)
        {
            EnvelopedCms envelopedCms = ParseEnvelopedCms(encryptedSecret);
            return Decrypt(envelopedCms, certificates);
        }

        public string Decrypt(EnvelopedCms envelopedCms, X509Certificate2[] certificates = null)
        {
            certificates = certificates ??
                           this.FindCertificates(envelopedCms.RecipientInfos, StoreLocation.CurrentUser,StoreName.My)
                           .Union(this.FindCertificates(envelopedCms.RecipientInfos, StoreLocation.LocalMachine,StoreName.My))
                           .ToArray();

            return DecryptByCertificates(envelopedCms, certificates);
        }

        public bool TryParseEnvelopedCms(string base64EnvelopedCms, out EnvelopedCms envelopedCms)
        {
            envelopedCms = null;

            try
            {
                envelopedCms = ParseEnvelopedCms(base64EnvelopedCms);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public EnvelopedCms ParseEnvelopedCms(string base64EnvelopedCms)
        {
            if (string.IsNullOrEmpty(base64EnvelopedCms))
            {
                throw new ArgumentNullException(nameof(base64EnvelopedCms));
            }

            byte[] encodedMessage;
            try
            {
                encodedMessage = Convert.FromBase64String(base64EnvelopedCms);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid base-64 format", nameof(base64EnvelopedCms), ex);
            }
            EnvelopedCms envelopedCms = new EnvelopedCms();
            try
            {
                envelopedCms.Decode(encodedMessage);
            }
            catch (CryptographicException ex)
            {
                throw new ArgumentException("Invalid enveloped CMS format", nameof(base64EnvelopedCms), ex);
            }
            if (envelopedCms.RecipientInfos == null || envelopedCms.RecipientInfos.Count < 1)
            {
                throw new ArgumentException("Enveloped CMS must contain at least one recipient info", nameof(base64EnvelopedCms));
            }

            return envelopedCms;
        }

        public IEnumerable<X509Certificate2> FindCertificates(RecipientInfoCollection recipients, StoreLocation storeLocation, StoreName storeName)
        {
            if (recipients == null)
            {
                throw new ArgumentNullException(nameof(recipients));
            }

            X509Store x509Store = new X509Store(storeName, storeLocation);
            try
            {
                x509Store.Open(OpenFlags.ReadOnly);
                foreach (RecipientInfo recipientInfo in recipients)
                {
                    if (recipientInfo.RecipientIdentifier.Value is X509IssuerSerial)
                    {
                        X509IssuerSerial recipientCertificateIssuerSerial = (X509IssuerSerial)recipientInfo.RecipientIdentifier.Value;
                        string issuerName = recipientCertificateIssuerSerial.IssuerName;
                        string serialNumber = recipientCertificateIssuerSerial.SerialNumber;
                        X509Certificate2Collection matchingCertificates = x509Store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, (object)issuerName, false).Find(X509FindType.FindBySerialNumber, (object)serialNumber, false);
                        foreach (X509Certificate2 x509Certificate2 in matchingCertificates)
                        {
                            if ((_allowNonValidCertificates || x509Certificate2.Verify()) && x509Certificate2.HasPrivateKey)
                                yield return x509Certificate2;
                        }
                    }
                }
            }
            finally
            {
                x509Store.Close();
            }
        }

        private static string DecryptByCertificates(EnvelopedCms envelopedCms, X509Certificate2[] certificates)
        {
            if (certificates == null || certificates.Length == 0)
            {
                throw new CryptographicException("No matching private key found in local machine and current user stores");
            }
                
            try
            {
                envelopedCms.Decrypt(new X509Certificate2Collection(certificates));
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException("Failed decrypting envelopedCms", ex);
            }

            try
            {
                return SecretEncoding.GetString(envelopedCms.ContentInfo.Content);
            }
            catch (DecoderFallbackException ex)
            {
                throw new DecoderFallbackException("Cannot decode decrypted bytes into a unicode string", ex);
            }
        }
    }
}
