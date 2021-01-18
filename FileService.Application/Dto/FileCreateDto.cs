using Comm100.Framework.Security;

namespace FileService.Application.Dto
{
    public class FileCreateDto
    {
        public FileDto File { get; set; }
        public AuthComm100Platform Auth { get; set; }
    }

}
