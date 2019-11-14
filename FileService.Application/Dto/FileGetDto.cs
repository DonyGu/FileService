namespace FileService.Application.Dto
{
    public class FileGetDto
    {
        public string FileKey { get; set; }
        // for rate limiting
        public string IP { get; set; }
    }
}