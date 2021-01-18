using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Comm100.Framework.Common
{
    public class CertificateHelper
    {
        private static volatile List<RsaWithThumbprint> PublicKeyWithThumbprintList = new List<RsaWithThumbprint>();

        private static DateTime _lastUpdatedTime = DateTime.MinValue;

        public static RsaWithThumbprint GetRsaFromPrivateKeyFile(string filePath, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                password = "";
            }
            var certificate = new X509Certificate2(filePath, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            var thumbprint = certificate.Thumbprint;
            return new RsaWithThumbprint(certificate.PrivateKey as RSA, thumbprint);
        }

        public static RsaWithThumbprint GetRsaFromPrivateKeyFile(byte[] bytes, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return getRsaFromBytes(bytes);
            }
            var certificate = new X509Certificate2(bytes, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            var thumbprint = certificate.Thumbprint;
            return new RsaWithThumbprint(certificate.PrivateKey as RSA, thumbprint);
        }

        private static RsaWithThumbprint getRsaFromBytes(byte[] bytes)
        {
            var certificate = new X509Certificate2(bytes);
            var thumbprint = certificate.Thumbprint;
            return new RsaWithThumbprint(certificate.PrivateKey as RSA, thumbprint);
        }

        public static RSA GetPublicKey(string jwtToken, string publicKey = null)
        {
            if (_lastUpdatedTime.AddMinutes(30) < DateTime.UtcNow)
            {
                UpdatePublicKeyWithThumbprintList(publicKey);
                _lastUpdatedTime = DateTime.UtcNow;
            }

            var t = new JwtSecurityToken(jwtToken);
            var thumbprint = t.GetThumbprint();
            var rsaKey = PublicKeyWithThumbprintList.FirstOrDefault(p => p.Thumbprint == thumbprint)?.Rsa;
            if (rsaKey == null)
            {
                throw new Exception(string.Format("Failed to get public key. \r\n Thumbprint: {0} \r\n JwtToken: {1} \r\n PublicKey: {2} \r\n Cache Publick Key List: {3} \r\n",
                    thumbprint, jwtToken, publicKey,
                    string.Join(", ", PublicKeyWithThumbprintList.Select(p => p.Thumbprint)?.ToArray() ?? new string[0])));
            }
            return rsaKey;
        }

        private static void UpdatePublicKeyWithThumbprintList(string config)
        {
            var list = new List<RsaWithThumbprint>();
            if (string.IsNullOrWhiteSpace(config))
            {
                throw new Exception("CertificateHelper:UpdatePublicKeyWithThumbprintList config cannot be null");
            }
            config = config.Trim();
            string publicKeys = null;
            if (config.StartsWith("nslookup", StringComparison.OrdinalIgnoreCase))
            {
                var arr = config.Split(' ');
                var domain = arr[arr.Length - 1];
                Dictionary<string, string> keys = new Dictionary<string, string>();
                var dnsRecords = DnsHelper.GetTextRecords(domain);
                var regBegin = new Regex("-----BEGIN (.*)-----(.*)");
                var regEnd = new Regex("(.*)-----END (.*)-----");
                foreach (var r in dnsRecords)
                {
                    var match = regBegin.Match(r);
                    if (match.Success)
                    {
                        var certKey = match.Groups[1].Value;
                        var content = match.Groups[2].Value;
                        if (keys.ContainsKey(certKey))
                        {
                            keys[certKey] = content + keys[certKey];
                        }
                        else
                        {
                            keys.Add(certKey, content);
                        }
                    }
                    else
                    {
                        var matchEnd = regEnd.Match(r);
                        if (matchEnd.Success)
                        {
                            var certKey = matchEnd.Groups[2].Value;
                            var content = matchEnd.Groups[1].Value;
                            if (keys.ContainsKey(certKey))
                            {
                                keys[certKey] = keys[certKey] + content;
                            }
                            else
                            {
                                keys.Add(certKey, content);
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid jwt token content: " + r);
                        }

                    }
                }
                publicKeys = string.Join(",", keys.Values.ToArray()).Trim(',');
            }
            else
            {
                publicKeys = config;
            }
            PublicKeyWithThumbprintList = publicKeys.Split(',').Select(p => GetPublicKeyWithThumbprint(p)).ToList();
        }
        private static string RemoveSpaceAndHeaderFooter(string publicKey)
        {
            var regBegin = "-----BEGIN CERTIFICATE-----";
            var regEnd = "-----END CERTIFICATE-----";
            return publicKey.Replace(regBegin, "").Replace(regEnd, "").Replace(" ", "").Replace("\\n", "");
        }

        /// <summary>
        /// Gets thumbprint from public key string.
        /// </summary>
        private static RsaWithThumbprint GetPublicKeyWithThumbprint(string publicKey)
        {
            var realPublicKey = RemoveSpaceAndHeaderFooter(publicKey);
            var publicKeyBytes = Convert.FromBase64String(realPublicKey);
            var certificate = new X509Certificate2(publicKeyBytes);
            return new RsaWithThumbprint(certificate.PublicKey.Key as RSA, certificate.Thumbprint);
        }

    }

    public class RsaWithThumbprint
    {
        public RSA Rsa { get; set; }

        public string Thumbprint { get; set; }

        public RsaWithThumbprint(RSA rsa, string thumbprint)
        {
            Rsa = rsa;
            Thumbprint = thumbprint;
        }
    }
}
