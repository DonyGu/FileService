using FileService.Domain.Entities;
using System.Collections.Generic;
using FileService.Domain.Bo;
using FileService.Domain.Specifications;

namespace FileService.Domain.Interfaces
{
    public interface IFileDomainService
    {
        // create fileKey and insert to db
        File Create(FileCreateBo file);
        // insert file to db
        File Create(File file);
        File Get(string fileKey);

        // for move files back from standby file service
        IReadOnlyList<File> GetList(int count);

        // for move files to S3
        IReadOnlyList<FileContent> GetFileContentList(FileContentFilterSpecification spec);

        // move file to s3 and update db
        void MoveToRemote(string fileKey);

        // delete record from t_fileService_file and s3
        void Delete(string fileKey);
    }
}
