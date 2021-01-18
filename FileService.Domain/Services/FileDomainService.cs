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
using System.Threading.Tasks;
using Comm100.Framework.Common;

namespace FileService.Domain.Services
{
    public class FileDomainService : IFileDomainService
    {
        private readonly IFileRepository _repository;
        private readonly IRepository<byte[], FileContent> _fileContentRepository;
        private readonly IS3Repository _s3Repository;
        private readonly IConfigService _configService;

        public FileDomainService(IFileRepository repository, IS3Repository s3Repository
            , IRepository<byte[], FileContent> fileContentRepository, IConfigService configService)
        {
            this._repository = repository;
            this._s3Repository = s3Repository;
            this._fileContentRepository = fileContentRepository;
            this._configService = configService;
            ;
        }

        public async Task<File> Create(File bo)
        {
            if (await _fileContentRepository.Exists(bo.Checksum))
            {
                bo.Content = null;
            }
            return await this._repository.Create(bo);
        }

        public async Task<File> Get(string fileKey)
        {
            var file = await this._repository.Get(fileKey);
            if (file == null)
            {
                throw new FileKeyNotFoundException();
            }
            file.Content = await this._fileContentRepository.Get((file.Checksum));
            // check expire, delete expire record and throw new FileKeyNotFoundException
            await DeleteOneExpiredFile(file);
            return file;
        }

        public async Task<bool> Exist(string fileKey)
        {
            return await this._repository.Exists(fileKey);
        }

        public IReadOnlyList<File> GetList(FileFilterSpecification spec)
        {
            return _repository.List(spec);
        }

        public async Task MoveToRemote(File file)
        {
            var s3Setting = await this._configService.GetJson<S3SettingsBo>("S3Settings");
            var s3FileBo = S3FileBoMapping(file, s3Setting);

            LogHelper.Debug($"begin post: {DateTime.UtcNow}({file.FileKey})");
            await this._s3Repository.Put(s3Setting, s3FileBo);
            LogHelper.Debug($"end post: {DateTime.UtcNow}({file.FileKey})");
            file.Content.StorageType = StorageType.S3;
            file.Content.Link = s3FileBo.Link;
            file.Content.Content = null;
            await this._fileContentRepository.Update(file.Content);
        }

        public async Task Delete(File file)
        {
            await this._repository.Delete(file);
            var count = await CountContentFiles(file.Checksum);
            if (count == 0)
            {
                var fileContent = file.Content;
                if (fileContent.StorageType == StorageType.S3)
                {
                    var s3Setting = await this._configService.GetJson<S3SettingsBo>("S3Settings");
                    await this._s3Repository.Delete(s3Setting, fileContent.Link);
                }
                await this._fileContentRepository.Delete(fileContent);
            }
        }
        public async Task Delete(string fileKey)
        {

            var file = await this._repository.Get(fileKey);
            await this._repository.Delete(file);
            var count = await CountContentFiles(file.Checksum);
            if (count == 0)
            {
                var fileContent = await this._fileContentRepository.Get(file.Checksum);
                if (fileContent.StorageType == StorageType.S3)
                {
                    var s3Setting = await this._configService.GetJson<S3SettingsBo>("S3Settings");
                    await this._s3Repository.Delete(s3Setting, fileContent.Link);
                }
                await this._fileContentRepository.Delete(fileContent);
            }
        }

        public async Task DeleteOneExpiredFile(File file)
        {
            var fileContent = file.Content;
            if (file.ExpireTime < DateTime.UtcNow)
            {
                await _repository.Delete(file);
                var count = await CountContentFiles(fileContent.Checksum);
                if (count == 0)
                {
                    if (fileContent.StorageType == StorageType.S3)
                    {
                        var s3Setting = await this._configService.GetJson<S3SettingsBo>("S3Settings");
                        await this._s3Repository.Delete(s3Setting, fileContent.Link);
                    }
                    await this._fileContentRepository.Delete(fileContent);
                }
                throw new FileKeyNotFoundException();
            }
        }

        public async Task<int> CountContentFiles(byte[] checksum)
        {
            return await _repository.Count(new FileFilterSpecification(checksum));
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

        public IEnumerable<File> GetTopInDb(int count)
        {
            return _repository.GetTopInDb(count);
        }
    }
}
