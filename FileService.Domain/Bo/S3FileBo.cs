namespace FileService.Domain.Bo
{
    public class S3FileBo
    {
        // prefix part of s3 url, create from siteId and fileKey
        // "/{siteId}/{first 2 characters of fileKey}/{rest characters of fileKey}"
        public string Link { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}