using System;
using FileService.Application.Dto;
using FileService.Application.Interfaces;
using Comm100.Framework.Config;
using Comm100.Framework.Security;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Comm100.Framework.Common;

namespace FileService.Web.Controllers
{
    [Route("v1/jwtoken")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        IFileAuthService _fileAuthService;
        IConfigService _configService;
        public JwtController(IFileAuthService authService, IConfigService configService)
        {
            this._fileAuthService = authService;
            this._configService = configService;
        }

        [HttpPost("")]
        public async Task<ActionResult<string>> Sign(JwtPayloadDto jwtPayloadDto)
        {
            //{"siteId":"1","ip":"192.168.1.1","fileExpireInDays":"10","exp":1574681417,"iat":1574680817,"iss":"host","aud":"FileService"}
            //var payLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwt));
            await this._fileAuthService.VerifyComm100Platform(
                new AuthComm100Platform
                {
                    SharedSecret = Request.Headers["Authorization"].ToArray().FirstOrDefault(a => a.StartsWith("Bearer "))?.Replace("Bearer ",string.Empty),
                    IP = this.HttpContext.GetRemoteIPAddress().MapToIPv4().ToString(),
                });

            return await _fileAuthService.GenerateToken(jwtPayloadDto);
        }

    }
}