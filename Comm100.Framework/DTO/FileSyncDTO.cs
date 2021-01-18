using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.DTO
{
    public class FileSyncDTO
    {
        public IFormFile file { get; set; }
        public string creationTime { get; set; } = DateTime.UtcNow.ToString();
        public string? expireTime { get; set; } = DateTime.MaxValue.ToString();
        public string siteId { get; set; }

    }
}
