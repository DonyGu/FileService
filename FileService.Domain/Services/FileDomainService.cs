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
        private readonly IConfigService _configService;

        public FileDomainService(IRepository<string, File> repository, IS3Repository s3Repository
            , IRepository<byte[], FileContent> fileContentRepository, IConfigService configService)
        {
            this._repository = repository;
            this._s3Repository = s3Repository;
            this._fileContentRepository = fileContentRepository;
            this._configService = configService;
            ;
        }

        public File Create(File bo)
        {
            if (_fileContentRepository.Exists(bo.Checksum))
            {
                bo.Content = null;
            }
            return this._repository.Create(bo);
        }

        public File Get(string fileKey)
        {
            var file = this._repository.Get(fileKey).Result;
            if (file == null)
            {
                throw new FileKeyNotFoundException();
            }
            file.Content = this._fileContentRepository.Get((file.Checksum)).Result;
            // check expire, delete expire record and throw new FileKeyNotFoundException
            DeleteOneExpiredFile(file);
            return file;
        }

        public bool Exist(string fileKey)
        {
            return this._repository.Exists(fileKey);
        }

        public IReadOnlyList<File> GetList(FileFilterSpecification spec)
        {
            return _repository.List(spec);
        }

        public void MoveToRemote(File file)
        {
            var s3Setting = this._configService.GetJson<S3SettingsBo>("S3Settings");
            var s3FileBo = S3FileBoMapping(file, s3Setting);
            this._s3Repository.Put(s3Setting, s3FileBo);
            file.Content.StorageType = StorageType.S3;
            file.Content.Link = s3FileBo.Link;
            //file.Content.Content = null;
            this._repository.Update(file);
        }

        public void Delete(string fileKey)
        {
            var file = this._repository.Get(fileKey).Result;
            var fileContent = this._fileContentRepository.Get((file.Checksum)).Result;
            this._repository.Delete(file);
            if (CountContentFiles(fileContent.Checksum)==0)
            {
                if (fileContent.StorageType == StorageType.S3)
                {
                    var s3Setting = this._configService.GetJson<S3SettingsBo>("S3Settings");
                    this._s3Repository.Delete(s3Setting, fileContent.Link);
                }
                this._fileContentRepository.Delete(fileContent);
            }
         
        }

        public void DeleteOneExpiredFile(File file)
        {
            var fileContent = file.Content;
            if (file.ExpireTime < DateTime.UtcNow)
            {
                _repository.Delete(file);
                
                if (CountContentFiles(fileContent.Checksum) == 0)
                {
                    if (fileContent.StorageType == StorageType.S3)
                    {
                        var s3Setting = this._configService.GetJson<S3SettingsBo>("S3Settings");
                        this._s3Repository.Delete(s3Setting, fileContent.Link);
                    }
                    this._fileContentRepository.Delete(fileContent);
                }
                throw new FileKeyNotFoundException();
            }
        }

        public int CountContentFiles(byte[] checksum)
        {
            return _repository.Count(new FileFilterSpecification(checksum));
        }


        private S3FileBo S3FileBoMapping(File file, S3SettingsBo s3SettingsBo)
        {
            return new S3FileBo
            {
                Name = file.Content.Name,
                Link = $"{s3SettingsBo.Address}/{file.SiteId}/{file.FileKey.Substring(0, 2)}/{file.FileKey.Substring(2, file.FileKey.Length - 2)}",
                Content = file.Content.Content
            };
        }

    }
}
