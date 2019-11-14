using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.Domain.Entities
{
    public class File
    {
        public string FileKey { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ExpireTime { get; set; }
        public byte[] Content { get; set; }
        // read from t_fileService_content
        public string Link { get; set; }
    }
}
