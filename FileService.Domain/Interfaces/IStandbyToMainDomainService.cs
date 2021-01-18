using FileService.Domain.Entities;
using System.Threading.Tasks;

namespace FileService.Domain.Interfaces
{
    public interface IStandbyToMainDomainService
    {
        // move n files to main service, n equals to NumOfWorkers in config
        Task MoveToMain();
        Task MoveFile(File f, string url, string sharedSecret);
    }
}