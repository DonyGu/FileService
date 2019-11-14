using System;

namespace FileService.Domain.Bo
{
    public class FileCreateBo
    {
        public string SiteId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public DateTime? ExpireTime { get; set; }

        // different app may have different size limit
        public string AppId { get; set; }
    }
}