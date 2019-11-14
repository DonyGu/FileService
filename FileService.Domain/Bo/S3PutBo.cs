namespace FileService.Domain.Bo
{
    public class S3PutBo
    {
        public string FileKey { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}