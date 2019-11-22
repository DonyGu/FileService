using FileService.Domain.Entities;
using System.Collections.Generic;
using FileService.Domain.Bo;
using FileService.Domain.Specifications;

namespace FileService.Domain.Interfaces
{
    public interface IFileDomainService
    {
        // insert file to db
        File Create(File file);
        File Get(string fileKey);
        bool Exist(string fileKey);
        IReadOnlyList<File> GetList(FileFilterSpecification spec);
        // move file to s3 and update db
        void MoveToRemote(string fileKey);
        // delete record from t_fileService_file and s3
        void Delete(string fileKey);
    }
}
