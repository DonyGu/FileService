using Comm100.Framework.Common;
using Comm100.Framework.Config;
using FileService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.Services
{
    public class JwtCertificateService : IJwtCertificateService
    {
        private readonly IConfigService _configService;
        private IConfiguration _configuration;
        private static DateTime _lastUpdatedTime = DateTime.MinValue;
        public static RsaWithThumbprint rsaWithThumbprint { get; private set; }
        public JwtCertificateService(IConfigService configService, IConfiguration configuration)
        {
            _configService = configService;
            _configuration = configuration;
        }
        public async Task<RsaWithThumbprint> GetPrivateKey()
        {
            if (_lastUpdatedTime.AddMinutes(30)<DateTime.UtcNow)
            {
                await UpdatePrivateKey();
                _lastUpdatedTime = DateTime.UtcNow;
            }
            return rsaWithThumbprint ?? throw new ArgumentNullException("Private Key");
        }

        private async Task UpdatePrivateKey()
        {
            try
            {
                var privateKeyPath = await _configService.Get("JWTPrivateKeyPath");
                var privateKeyPassword = await _configService.Get("JWTPassword");
                if (privateKeyPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var bytes = await httpClient.GetByteArrayAsync(privateKeyPath);
                        rsaWithThumbprint = CertificateHelper.GetRsaFromPrivateKeyFile(bytes, privateKeyPassword);
                    }
                }
                else
                {
                    rsaWithThumbprint = CertificateHelper.GetRsaFromPrivateKeyFile(privateKeyPath, privateKeyPassword);
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            
            
        }
    }
}
