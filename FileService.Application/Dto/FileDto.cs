using System;
using FileService.Domain.Entities;

namespace FileService.Application.Dto
{
    public class FileDto
    {
        public string FileKey { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ExpireTime { get; set; }
        public byte[] Content{ get; set; }
        public StorageType StorageType { get; set; }
        public string Link { get; set; }
    }
}