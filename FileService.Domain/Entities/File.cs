using System;

namespace FileService.Domain.Entities
{
    public class File
    {
        public string FileKey { get; set; }
        public int SiteId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ExpireTime { get; set; }
        public byte[] Checksum { get; set; }
        public virtual FileContent Content { get; set; }
    }
}
