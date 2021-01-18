using FileService.Domain.Entities;
using System.Collections.Generic;
using FileService.Domain.Bo;
using FileService.Domain.Specifications;
using System.Threading.Tasks;

namespace FileService.Domain.Interfaces
{
    public interface IFileDomainService
    {
        // insert file to db
        Task<File> Create(File file);
        Task<File> Get(string fileKey);
        Task<bool> Exist(string fileKey);
        IReadOnlyList<File> GetList(FileFilterSpecification spec);

        IEnumerable<File> GetTopInDb(int count);
        // move file to s3 and update db
        Task MoveToRemote(File file);
        // delete record from t_fileService_file and s3
        Task Delete(File file);
        Task Delete(string fileKey);
    }
}
