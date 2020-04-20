using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Comm100.Framework.Common
{
    public class CertificateHelper
    {
        public static RSA GetRsaFromPrivateKeyFile(string filePath, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                password = "";
            }
            var certificate = new X509Certificate2(filePath, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            return (RSA)certificate.PrivateKey;
        }

        public static RSA GetRsaFromPrivateKeyFile(byte[] bytes, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return getRsaFromBytes(bytes);
            }
            var certificate = new X509Certificate2(bytes, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            var thumbprint = certificate.Thumbprint;
            return (RSA)certificate.PrivateKey;
        }

        private static RSA getRsaFromBytes(byte[] bytes)
        {
            var certificate = new X509Certificate2(bytes);
            return(RSA)certificate.PrivateKey;
        }
    }
}
