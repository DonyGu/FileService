using Comm100.Framework.Common;
using Comm100.Framework.Config;
using FileService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace FileService.Domain.Services
{
    public class JwtCertificateService : IJwtCertificateService
    {
        private readonly IConfigService _configService;
        private static DateTime _lastUpdatedTime = DateTime.MinValue;
        public static RSA rsa { get; private set; }
        public JwtCertificateService(IConfigService configService)
        {
            _configService = configService;
        }
        public RSA GetPrivateKey()
        {
            if (_lastUpdatedTime.AddMinutes(30)<DateTime.UtcNow)
            {
                UpdatePrivateKey();
                _lastUpdatedTime = DateTime.UtcNow;
            }
            return rsa ?? throw new ArgumentNullException("Private Key");
        }

        private void UpdatePrivateKey()
        {
            try
            {
                var privateKeyPath = _configService.Get("JWTPrivateKeyPath");
                var privateKeyPassword = _configService.Get("JWTPassword");
                if (privateKeyPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var bytes = httpClient.GetByteArrayAsync(privateKeyPath).Result;
                        rsa = CertificateHelper.GetRsaFromPrivateKeyFile(bytes, privateKeyPassword);
                    }
                }
                else
                {
                    rsa = CertificateHelper.GetRsaFromPrivateKeyFile(privateKeyPath, privateKeyPassword);
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            
            
        }
    }
}
