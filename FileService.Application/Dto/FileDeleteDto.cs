using Comm100.Framework.Security;

namespace FileService.Application.Dto
{
    public class FileDeleteDto
    {
        public string FileKey { get; set; }
        public AuthComm100Platform Auth { get; set; }
    }
}