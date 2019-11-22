using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// using Comm100.Domain.Repository;
// using Comm100.Extension;
// using Comm100.Runtime.Exception;
using FileService.Domain.Entities;
using FileService.Domain.Interfaces;
using FileService.Domain.Bo;
using FileService.Domain.Specifications;
using Comm100.Framework.Config;
using Comm100.Framework.Exceptions;
using Comm100.Framework.Domain.Repository;
using System.Security.Cryptography;

namespace FileService.Domain.Services
{
    public class FileDomainService : IFileDomainService
    {
        private readonly IRepository<string, File> _repository;
        private readonly IRepository<byte[], FileContent> _fileContentRepository;
        private readonly IS3Repository _s3Repository;

        public FileDomainService(IRepository<string, File> repository, IS3Repository s3Repository, IRepository<byte[], FileContent> fileContentRepository)
        {
            this._repository = repository;
            this._s3Repository = s3Repository;
            this._fileContentRepository = fileContentRepository;
        }

        public File Create(File bo)
        {
            return this._repository.Create(bo);
        }

        public File Get(string fileKey)
        {
            // check expire, delete expire record and throw new FileKeyNotFoundException

            var file = this._repository.Get(fileKey);
            file.Content = this._fileContentRepository.Get((file.Checksum));
            return file;
        }

        public bool Exist(string fileKey)
        {
            return this._repository.Exists(fileKey);
        }

        public IReadOnlyList<File> GetList(FileFilterSpecification spec)
        {

            throw new NotImplementedException();
        }

        public void MoveToRemote(string fileKey)
        {
            this._repository.Update(new File());
            this._s3Repository.Put(new S3SettingsBo(), new S3FileBo());

            throw new NotImplementedException();
        }

        public void Delete(string fileKey)
        {
            this._repository.Delete(this._repository.Get(fileKey));
            this._s3Repository.Delete(new S3SettingsBo(), fileKey);
            throw new NotImplementedException();
        }

        public void DeleteOneExpiredFile()
        {
            throw new NotImplementedException();
        }



    }
}
