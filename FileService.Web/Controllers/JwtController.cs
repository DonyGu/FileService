using System;
using FileService.Application.Dto;
using FileService.Application.Interfaces;
using Comm100.Framework.Config;
using Comm100.Framework.Security;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<string> Sign([FromBody] string jwt)
        {
            this._fileAuthService.VerifyComm100Platform(
                new AuthComm100Platform
                {
                    SharedSecret = "",
                    IP = "",
                });

            throw new NotImplementedException();
        }
    }
}