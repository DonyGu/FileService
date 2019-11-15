using System;
using FileService.Application.Dto;
using FileService.Application.Interfaces;
using Comm100.Framework.Config;
using Comm100.Framework.Exceptions;
using Comm100.Framework.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileAppService _fileAppService;
        private readonly IConfigService _configService;

        public FilesController(IFileAppService fileAppService, IConfigService configService)
        {
            this._fileAppService = fileAppService;
            this._configService = configService;
        }

        [HttpPost("")]
        public ActionResult<string> Upload([FromForm]IFormFile file)
        {
            var dto = new FileUploadDto();
            dto.Name = file.Name;
            dto.Content = StreamToBytes(file.OpenReadStream());
            //dto.Auth = new AuthJwt
            //{
            //    IP = this.HttpContext.Connection.RemoteIpAddress.ToString(),
            //    Jwt = this.Request.Headers["Authorization"].ToArray()[0],
            //};
            this._fileAppService.Upload(dto);

            return Ok();
        }

        [HttpPost("{fileKey}")]
        public ActionResult Create(string fileKey, IFormFile form)
        {
            this._fileAppService.Create(new FileCreateDto());
            throw new NotImplementedException();
        }

        [HttpGet("{fileKey}")]
        public ActionResult Get(string fileKey)
        {
            // cases:
            // return file content
            // redirect to s3
            // redirect to main file service
            // file not found
            try
            {
                var file = this._fileAppService.Get(new FileGetDto());
            }
            catch(FileKeyNotFoundException)
            {
                if (this._configService.GetBool("IsMainServer"))
                {
                    // response 404
                }
                else
                {
                    var url = this._configService.Get("MainServiceUrl");
                    // redirect to main file service
                }
            }
            throw new NotImplementedException();
        }

        [HttpDelete("{fileKey}")]
        public ActionResult Delete(string fileKey)
        {
            this._fileAppService.Delete(new FileDeleteDto());
            throw new NotImplementedException();
        }

        [HttpGet("version")]
        public ActionResult Version()
        {
            return Ok("File Service 001");
        }

        private byte[] StreamToBytes(System.IO.Stream s)
        {
            using(var ms = new System.IO.MemoryStream())
            {
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
