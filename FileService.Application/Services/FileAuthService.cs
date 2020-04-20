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

        public FileServiceJwt VerifyJwt(AuthJwt auth)
        {
            try
            {
                // check signature
                var publicKey = this._configService.Get("JWTPublicKeys");
                var cer = new X509Certificate2(Convert.FromBase64String(publicKey));
                var rsa = (RSA)(cer.PublicKey.Key);
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
                if (scope.ip != auth.IP)
                {
                    throw new UnauthorizedException();
                }
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
                LogHelper.ErrorLog(ex.Message, ex);
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
            throw new FileNotAllowedException();
        }

        public string GenerateToken(JwtPayloadDto jwtPayloadDto)
        {
            var claims = new List<Claim>
            {
                new Claim("scope",JsonConvert.SerializeObject(jwtPayloadDto.scope)),
                new Claim("jti",Guid.NewGuid().ToString())
            };
            var iss = jwtPayloadDto.iss;
            var aud = "FileService";
            var iat = DateTime.UtcNow;
            var exp = iat.AddMinutes(10);
            var rsa = _jwtCertificateService.GetPrivateKey();
            var key = new RsaSecurityKey(rsa.ExportParameters(true));
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);
            var payload = new JwtPayload(iss, aud, claims, null, exp, iat);
            var token = new JwtSecurityToken(new JwtHeader(creds), payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void VerifyComm100Platform(AuthComm100Platform auth)
        {
            VerifySharedSecret(auth.SharedSecret);
            VerifyIP(auth.IP);
        }

        private void VerifySharedSecret(string clientSharedSecret)
        {
            var sharedSecret = _configService.Get("SharedSecret");
            if (clientSharedSecret != sharedSecret)
            {
                throw new UnauthorizedException();
            }
        }

        private void VerifyIP(string ip)
        {
            var whiteList = _configService.GetJson<IPRange[]>("IPWhiteList");
            var ifValid = false;
            LogHelper.WriteLog($"ip:{ip},list:{string.Join(";",whiteList.Select(i=>$"{i.From}->{i.To}"))}");

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

        public void VerifFile(string appId, string name, byte[] content)
        {
            var fileBlackList = this._configService.GetJson<string[]>("FileBlackList");
            if (FileHelper.CheckFileNameLegitimacy(name, content, fileBlackList) == false)
            {
                throw new FileNotAllowedException();
            }

        }
    }
}