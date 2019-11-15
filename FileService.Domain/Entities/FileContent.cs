using System;

namespace FileService.Domain.Entities
{
    public class FileContent
    {
        public byte[] Checksum { get; set; }
        public string Name { get; set; }
        public DateTime ExpireTime { get; set; }
        public string Link { get; set; }
        public StorageType StorageType { get; set; }
        // read from t_fileService_file
        public string FileKey { get; set; }
    }

    public enum StorageType
    {
        Db = 0,
        S3 = 1
    }
}