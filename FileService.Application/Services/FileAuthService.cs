using System;
using FileService.Application.Dto;
using FileService.Application.Interfaces;
using Comm100.Framework.Config;
using Comm100.Framework.Security;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Collections.Generic;
using FileService.Domain.Interfaces;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Linq;
using Comm100.Framework.Exceptions;
using Comm100.Framework.Common;
using System.Threading.Tasks;
using System.Net.Http;

namespace FileService.Application.Services
{
    public class FileAuthService : IFileAuthService
    {
        IConfigService _configService;
        IJwtCertificateService _jwtCertificateService;

        public FileAuthService(IConfigService configService, IJwtCertificateService jwtCertificateService)
        {
            this._configService = configService;
            this._jwtCertificateService = jwtCertificateService;
        }

        public async Task<FileServiceJwt> VerifyJwt(AuthJwt auth)
        {
            try
            {
                // check signature
                var publicKey = await this._configService.Get("JWTPublicKeys");
                //var cer = new X509Certificate2(Convert.FromBase64String(publicKey));
                var rsa = CertificateHelper.GetPublicKey(auth.Jwt, publicKey);//  (RSA)(cer.PublicKey.Key);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ValidateIssuer = false,
                };
                var result = new JwtSecurityTokenHandler().ValidateToken(auth.Jwt, validationParameters, out SecurityToken securityToken);
                var validateToken = securityToken as JwtSecurityToken;
                string scopeStr = validateToken.Claims.FirstOrDefault(c => c.Type == "scope")?.Value;
                string jti = validateToken.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
                string iss = validateToken.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;
                ScopeDto scope = new ScopeDto();
                if (!string.IsNullOrEmpty(scopeStr))
                {
                    scope = JsonConvert.DeserializeObject<ScopeDto>(scopeStr);
                }
                //LogHelper.Info($"Check ip: JWT->{scope.ip}, FileService->{auth.IP}");
                //此处ip校验，在对部分域名使用vpn加速情况下，可能造成ip不一致，所以暂时去掉
                /*
                if (scope.ip != auth.IP && scope.ip != "127.0.0.1")
                {
                    throw new UnauthorizedException();
                }
                */
                VerifyJti(jti);
                return new FileServiceJwt()
                {
                    ExpireInDays = scope.fileExpireInDays,
                    SiteId = scope.siteId,
                    AppId = iss
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, ex.Message);
                throw new UnauthorizedException();
            }

        }

        private void VerifyJti(string jti)
        {
            if (!string.IsNullOrEmpty(jti))
            {
                if (CacheHelper.CacheValue<string>(jti) == null)
                {
                    CacheHelper.CacheInsertAddMinutes<string>(jti, jti, 10);
                    return;
                }
            }
            throw new UnauthorizedException();
        }

        public async Task<string> GenerateToken(JwtPayloadDto jwtPayloadDto)
        {
            var rsaWithThumbprint = await _jwtCertificateService.GetPrivateKey();

            var claims = new List<Claim>
            {
                new Claim("scope",JsonConvert.SerializeObject(jwtPayloadDto.scope)),
                new Claim("jti",Guid.NewGuid().ToString()),
                new Claim(JwtTokenConstants.Thumbprint, rsaWithThumbprint.Thumbprint),
            };
            var iss = jwtPayloadDto.iss;
            var aud = "FileService";
            var iat = DateTime.UtcNow;
            var exp = iat.AddMinutes(10);
            var key = new RsaSecurityKey(rsaWithThumbprint.Rsa.ExportParameters(true));
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);
            var payload = new JwtPayload(iss, aud, claims, null, exp, iat);
            var token = new JwtSecurityToken(new JwtHeader(creds), payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task VerifyComm100Platform(AuthComm100Platform auth)
        {
            await VerifySharedSecret(auth.SharedSecret);
            await VerifyIP(auth.IP);
        }

        private async Task VerifySharedSecret(string clientSharedSecret)
        {
            var sharedSecret = await _configService.Get("SharedSecret");
            if (clientSharedSecret != sharedSecret)
            {
                throw new UnauthorizedException();
            }
        }

        private async Task VerifyIP(string ip)
        {
            var whiteList = await _configService.GetJson<IPRange[]>("IPWhiteList");
            var ifValid = false;
            LogHelper.Info($"ip:{ip},list:{string.Join(";", whiteList.Select(i => $"{i.From}->{i.To}"))}");

            foreach (var item in whiteList)
            {
                if ((IP2Long(ip) >= IP2Long(item.From)) && (IP2Long(ip) <= IP2Long(item.To)))
                {
                    ifValid = true;
                    break;
                }
            }
            if (!ifValid)
            {
                throw new UnauthorizedException();
            }
        }

        class IPRange
        {
            public string From { get; set; }
            public string To { get; set; }
        }

        private static long IP2Long(string ip)
        {
            string[] ipBytes;
            double num = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }
            }
            return (long)num;
        }

        public async Task VerifFile(string appId, string name, byte[] content)
        {
            var fileBlackList = await this._configService.GetJson<string[]>("FileBlackList");
            if (FileHelper.CheckFileNameLegitimacy(name, content, fileBlackList) == false)
            {
                throw new FileNotAllowedException();
            }

        }

    }
}