using System;
using FileService.Application.Dto;
using FileService.Application.Interfaces;
using Comm100.Framework.Config;
using Comm100.Framework.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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
            dto.Name = file.FileName;
            dto.Content = StreamToBytes(file.OpenReadStream());
            //dto.Auth = new AuthJwt
            //{
            //    IP = this.HttpContext.Connection.RemoteIpAddress.ToString(),
            //    Jwt = this.Request.Headers["Authorization"].ToArray()[0],
            //};
            var fileDTO = this._fileAppService.Upload(dto);

            return Ok(fileDTO.FileKey);
        }

        [HttpPost("{fileKey}")]
        public ActionResult Create(string fileKey, [FromForm]IFormFile file)
        {
            if (fileKey.Length != 172)
            {
                throw new Exception("lenth");
            }
            this._fileAppService.Create(new FileCreateDto()
            {
                File = new FileDto
                {
                    FileKey = fileKey,
                    Content = StreamToBytes(file.OpenReadStream()),
                    Name = file.FileName
                }
            });
            return Ok();
        }

        [HttpGet("{fileKey}")]
        public FileContentResult Get(string fileKey)
        {
            // cases:
            // return file content
            // redirect to s3
            // redirect to main file service
            // file not found
            try
            {
                var file = this._fileAppService.Get(fileKey);
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.Name,
                    Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");
                return File(file.Content, "application/octet-stream", DateTimeOffset.Now.AddSeconds(1), new EntityTagHeaderValue(new Microsoft.Extensions.Primitives.StringSegment("\"inline\"")));
            }
            catch (FileKeyNotFoundException)
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
                throw new NotImplementedException();
            }
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
            using (var ms = new System.IO.MemoryStream())
            {
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
