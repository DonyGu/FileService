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

namespace FileService.Domain.Services
{
    public class FileDomainService : IFileDomainService
    {
        private readonly IConfigService _configService;
        private readonly IFileLimitDomainService _fileLimitDomainService;
        private readonly IRepository<string, File> _repository;
        private readonly IRepository<string, FileContent> _fileContentRepository;
        private readonly IS3Repository _s3Repository;

        public FileDomainService(IConfigService configService,
            IFileLimitDomainService fileLimitDomainService,
            IRepository<string, File> repository,
            IS3Repository s3Repository,
            IRepository<string, FileContent> fileContentRepository)
        {
            this._configService = configService;
            this._fileLimitDomainService = fileLimitDomainService;
            this._repository = repository;
            this._s3Repository = s3Repository;
            this._fileContentRepository = fileContentRepository;
        }

        public File Create(FileCreateBo bo)
        {
            this._fileLimitDomainService.Check(new CheckFileLimitBo());

            var fileKey = CreateFileKey(bo);

            this._repository.Create(new File());

            throw new NotImplementedException();
        }

        public File Create(File bo)
        {
            this._repository.Create(bo);

            throw new NotImplementedException();
        }

        public File Get(string fileKey)
        {
            // check expire, delete expire record and throw new FileKeyNotFoundException
            return this._repository.Get(fileKey);

            throw new NotImplementedException();
        }

        public IReadOnlyList<File> GetList(int n)
        {
            return this._repository.ListAll().Take(n).ToList();

            throw new NotImplementedException();
        }

        public IReadOnlyList<FileContent> GetFileContentList(FileContentFilterSpecification spec)
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

        private string CreateFileKey(FileCreateBo bo)
        {
            throw new NotImplementedException();
        }
    }
}
