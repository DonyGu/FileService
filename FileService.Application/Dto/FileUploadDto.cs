using Comm100.Framework.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileService.Application.Dto
{
    public class FileUploadDto
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }

        public AuthJwt Auth { get; set; }
    }
}
