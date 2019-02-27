using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AspNetCore.Base.Security
{
    public static class SigningKey
    {
        public static RsaSecurityKey LoadPrivateRsaSigningKey(string privateKeyPath)
        {
            var rsaParameters = AsymmetricEncryptionHelper.RsaWithPEMKey.GetPrivateKeyRSAParameters(privateKeyPath);
            var key = new RsaSecurityKey(rsaParameters);
            return key;
        }

        public static RsaSecurityKey LoadPublicRsaSigningKey(string publicKeyPath)
        {
            var rsaParameters = AsymmetricEncryptionHelper.RsaWithPEMKey.GetPublicKeyRSAParameters(publicKeyPath);
            var key = new RsaSecurityKey(rsaParameters);
            return key;
        }

        public static X509SecurityKey LoadPrivateSigningCertificate(string privateSigningCertificatePath, string password)
        {
            X509Certificate2 privateCertificate = new X509Certificate2(privateSigningCertificatePath, password, X509KeyStorageFlags.PersistKeySet);
            return new X509SecurityKey(privateCertificate);
        }

        public static X509SecurityKey LoadPublicSigningCertificate(string publicSigningCertificatePath)
        {
            var publicCertificate = new X509Certificate2(publicSigningCertificatePath);
            var key = new X509SecurityKey(publicCertificate);
            return key;
        }
    }
}
