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
using Comm100.Framework.DTO;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Text.Encodings.Web.Utf8;
using System.Text;

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
        public async Task<ActionResult<string>> Upload(IFormFile file)
        {
            var dto = new FileUploadDto();
            if (file==null)
            {
                throw new Exception("File must not be empty.");
            }
            dto.Name = file.FileName;
            dto.Content = StreamToBytes(file.OpenReadStream());
            dto.Auth = new AuthJwt
            {
                IP = this.HttpContext.GetRemoteIPAddress().MapToIPv4().ToString(),
                Jwt = this.Request.Headers["Authorization"].ToArray().FirstOrDefault(a => a.StartsWith("Bearer "))?.Replace("Bearer ", string.Empty),
            };
            var fileDTO = await this._fileAppService.Upload(dto);
            return Ok(fileDTO.FileKey);
        }

        [HttpPost("{fileKey}")]
        public async Task<ActionResult> Create(string fileKey, IFormFile file, [FromForm]int siteId, [FromForm]DateTime? CreationTime = null, [FromForm]DateTime? expireTime = null)
        {
            if (fileKey.Length != 172)
            {
                throw new FileKeyNotFoundException();
            }

            LogHelper.Info($"sync request: {fileKey}");
            await this._fileAppService.Create(new FileCreateDto()
            {
                File = new FileDto
                {
                    FileKey = fileKey,
                    Content = StreamToBytes(file.OpenReadStream()),
                    Name = file.FileName,
                    CreationTime = CreationTime ?? DateTime.UtcNow,
                    ExpireTime = expireTime ?? DateTime.MaxValue,
                    SiteId = siteId
                },
                Auth = new AuthComm100Platform
                {
                    SharedSecret = Request.Headers["Authorization"].ToArray().FirstOrDefault(a => a.StartsWith("Bearer "))?.Replace("Bearer ", string.Empty),
                    IP = this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                }
            });
            return Ok();
        }

        [HttpHead("{fileKey}")]
        [HttpGet("{fileKey}")]
        public async Task<ActionResult> Get(string fileKey)
        {
            // cases:
            // return file content
            // redirect to s3
            // redirect to main file service
            // file not found
            try
            {
                var file = await this._fileAppService.Get(fileKey);
                if (file.StorageType == StorageType.S3)
                {
                    return new RedirectResult(file.Link);
                }
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = HttpUtility.UrlEncode(file.Name, Encoding.UTF8).Replace("+"," ").Replace("%2b","+"),
                    Inline = ContentTypeHelper.FileIsInline(file.Name)  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");
                Response.Headers.Add("cache-control", "max-age=31536000");
                return File(file.Content, ContentTypeHelper.GetMimeType(file.Name), DateTimeOffset.Now.AddSeconds(1), new EntityTagHeaderValue(new Microsoft.Extensions.Primitives.StringSegment("\"inline\"")));
            }
            catch (FileKeyNotFoundException ex)
            {
                if (await this._configService.GetBool("IsMainServer"))
                {
                    throw ex;
                }
                else
                {
                    var url = await this._configService.Get("MainServiceUrl");
                    return new RedirectResult($"{url}/{fileKey}");
                    // redirect to main file service
                }
            }
        }

        [HttpDelete("{fileKey}")]
        public async Task<ActionResult> Delete(string fileKey)
        {
            var dto = new FileDeleteDto()
            {
                FileKey = fileKey,
                Auth = new AuthComm100Platform
                {
                    SharedSecret = Request.Headers["Authorization"].ToArray().FirstOrDefault(a => a.StartsWith("Bearer "))?.Replace("Bearer ", string.Empty),
                    IP = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                }
            };
            await this._fileAppService.Delete(dto);
            return Ok();
        }

        [HttpGet("version")]
        public ActionResult Version()
        {
            var fileAssembly = Assembly.GetEntryAssembly();
            var version = fileAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            var lastWriteTime = System.IO.File.GetLastWriteTime(fileAssembly.Location);
            return Ok($"File Service: { version ?? "unknow version"},{lastWriteTime}");
        }

        [HttpGet("monitor")]
        public async Task<ActionResult<string>> Monitor(int count = 20)
        {
            var auth = new AuthJwt
            {
                IP = this.HttpContext.GetRemoteIPAddress().MapToIPv4().ToString(),
                Jwt = this.Request.Headers["Authorization"].ToArray().FirstOrDefault(a => a.StartsWith("Bearer "))?.Replace("Bearer ", string.Empty),
            };
            var fileDTO = await this._fileAppService.Monitor(auth, count);
            return Ok(fileDTO);
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
