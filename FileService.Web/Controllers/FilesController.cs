using System;
using FileService.Application.Dto;
using FileService.Application.Interfaces;
using Comm100.Framework.Config;
using Comm100.Framework.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using FileService.Domain.Entities;
using Comm100.Framework.Security;
using System.Web;
using Comm100.Framework.Common;
using System.Linq;
using System.Text.Encodings.Web;

namespace FileService.Web.Controllers
{
    [Route("v1/files")]
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
        public ActionResult<string> Upload(IFormFile file)
        {
            var dto = new FileUploadDto();
            dto.Name = file.FileName;
            dto.Content = StreamToBytes(file.OpenReadStream());
            dto.Auth = new AuthJwt
            {
                IP = this.HttpContext.GetRemoteIPAddress().MapToIPv4().ToString(),
                Jwt = this.Request.Headers["Authorization"].ToArray().FirstOrDefault(a => a.StartsWith("Bearer "))?.Replace("Bearer ", string.Empty),
            };
            var fileDTO = this._fileAppService.Upload(dto);
            return Ok(fileDTO.FileKey);
        }

        [HttpPost("{fileKey}")]
        public ActionResult Create(string fileKey,IFormFile file)
        {
            if (fileKey.Length != 172)
            {
                throw new FileKeyNotFoundException();
            }
            this._fileAppService.Create(new FileCreateDto()
            {
                File = new FileDto
                {
                    FileKey = fileKey,
                    Content = StreamToBytes(file.OpenReadStream()),
                    Name = file.FileName
                },
                Auth=new AuthComm100Platform
                {
                    SharedSecret = Request.Headers["Authorization"].ToArray().FirstOrDefault(a => a.StartsWith("Bearer "))?.Replace("Bearer ", string.Empty),
                    IP = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                }
            });
            return Ok();
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
                var file = this._fileAppService.Get(fileKey);
                if (file.StorageType == StorageType.S3)
                {
                    return new RedirectResult(file.Link);
                }
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = HttpUtility.UrlEncode(file.Name),
                    Inline = ContentTypeHelper.FileIsInline(file.Name)  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");
                Response.Headers.Add("cache-control", "max-age=31536000");
                return File(file.Content, ContentTypeHelper.GetMimeType(file.Name), DateTimeOffset.Now.AddSeconds(1), new EntityTagHeaderValue(new Microsoft.Extensions.Primitives.StringSegment("\"inline\"")));
            }
            catch (FileKeyNotFoundException ex)
            {
                if (this._configService.GetBool("IsMainServer"))
                {
                    throw ex;
                }
                else
                {
                    var url = this._configService.Get("MainServiceUrl");
                    return new RedirectResult($"{url}/{fileKey}");
                    // redirect to main file service
                }
            }
        }

        [HttpDelete("{fileKey}")]
        public ActionResult Delete(string fileKey)
        {
            var dto = new FileDeleteDto()
            {
                FileKey=fileKey,
                Auth=new AuthComm100Platform
                {
                    SharedSecret = Request.Headers["Authorization"].ToArray().FirstOrDefault(a=>a.StartsWith("Bearer "))?.Replace("Bearer ", string.Empty),
                    IP = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                }
        };
            this._fileAppService.Delete(dto);
            return Ok();
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
