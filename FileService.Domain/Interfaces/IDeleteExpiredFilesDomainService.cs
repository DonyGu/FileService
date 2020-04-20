using System.Threading.Tasks;

namespace FileService.Domain.Interfaces
{
    public interface IDeleteExpiredFilesDomainService
    {
        Task DeleteAnExpiredFile();
    }
}