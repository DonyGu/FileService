
using Comm100.Framework.Common;
using Comm100.Framework.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Controllers
{
    public abstract class BaseController : Controller
    {
        private string configKey = "";
        private string defaultPrivateKey = "49d7f932-4aa8-4511-96b9-49fd836145d0";
        private string cookieKey = "logToken";
        private int tokenExpireMinutes = 60;
        private readonly IConfigService _configService;

        protected BaseController(IConfigService configService)
        {
            _configService = configService;
        }


        protected async Task<bool> HasLogin()
        {
            HttpContext.Request.Cookies.TryGetValue(cookieKey, out string jwtToken);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                var privateKey = await _configService.TryGet(configKey, defaultPrivateKey);
                var principal = await ValidateLogToken(jwtToken, ConvertToBase64String(privateKey));
                if (principal!=null)
                { 
                    return true; 
                }
            }
            LogHelper.Error($"HasLogin: false");
            return false;
        }

        private string ConvertToBase64String(string str)
        {
            string base64PrivateKey = str.PadRight(16, '0');
            var bytes = System.Text.Encoding.UTF8.GetBytes(base64PrivateKey);
            return Convert.ToBase64String(bytes);
        }

        protected async Task<bool> Login(string token)
        {
            var privateKey = await _configService.TryGet(configKey, defaultPrivateKey);
            if (token == privateKey)
            {
                var jwtToken = await GenerateLogToken(ConvertToBase64String(privateKey), tokenExpireMinutes);
                HttpContext.Response.Cookies.Append(cookieKey, jwtToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpireMinutes)
                });
                return true;
            }
            return false;
        }

        private async Task<string> GenerateLogToken(string appKey, int tokenExpireMinutes)
        {
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("userId", Guid.NewGuid().ToString()));


            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(appKey)), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(tokenExpireMinutes),
                Issuer = "",
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                SigningCredentials = signingCredentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return await Task.Run(() => { return token; }).ConfigureAwait(false);

        }

        private async Task<ClaimsPrincipal> ValidateLogToken(string token, string privateKey)
        {
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(privateKey));
            if (securityKey == null)
            {
                return null;
            }

            return await Task.Run(() => {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = securityKey,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters parameters) =>
                        {
                            return expires > DateTime.UtcNow;
                        }
                    };

                    SecurityToken validatedToken;
                    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                    return principal;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }).ConfigureAwait(false);
        }
    }
}
