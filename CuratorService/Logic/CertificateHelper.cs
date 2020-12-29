using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace curator.Logic
{
    internal class CertificateHelper
    {
        public static byte[] ConvertPemToData(string pem)
        {
            var noLabel = Regex.Replace(pem, "-----.*?-----", "",
                RegexOptions.Multiline & RegexOptions.Compiled & RegexOptions.IgnoreCase & RegexOptions.ECMAScript);
            var b64String = Regex.Replace(noLabel, "\r|\n", "",
                RegexOptions.Multiline & RegexOptions.Compiled & RegexOptions.IgnoreCase & RegexOptions.ECMAScript);
            return Convert.FromBase64String(b64String);
        }

        public static X509Certificate2 CreateDefaultSslCertificate()
        {
            var certSize = 2048;
            var certSubjectName = "CN=CuratorServiceServerSSL";

            using (RSA rsa = RSA.Create(certSize))
            {
                var certificateRequest = new CertificateRequest(certSubjectName, rsa,
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                certificateRequest.CertificateExtensions.Add(
                    new X509BasicConstraintsExtension(false, false, 0, false));

                certificateRequest.CertificateExtensions.Add(
                    new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                        true));

                certificateRequest.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection
                        {
                            new Oid("1.3.6.1.5.5.7.3.1")
                        },
                        true));

                certificateRequest.CertificateExtensions.Add(
                    new X509SubjectKeyIdentifierExtension(certificateRequest.PublicKey, false));

                return certificateRequest.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10));
            }
        }

    }
}
