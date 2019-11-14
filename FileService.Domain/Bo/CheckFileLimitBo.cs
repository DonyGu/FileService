namespace FileService.Domain.Bo
{
    public class CheckFileLimitBo
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}