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
        private readonly IConfigService _configService;
        private readonly IFileLimitDomainService _fileLimitDomainService;
        private readonly IRepository<string, File> _repository;
        private readonly IRepository<byte[], FileContent> _fileContentRepository;
        private readonly IS3Repository _s3Repository;
        private const string salt1 = "ADA4CDE7-C963-4FBC-BC5D-B13A85AF4A1D";
        private const string salt2 = "89439DB7-7548-4E4C-9816-34D3D1829190";

        public FileDomainService(IConfigService configService,
            IFileLimitDomainService fileLimitDomainService,
            IRepository<string, File> repository,
            IS3Repository s3Repository,
            IRepository<byte[], FileContent> fileContentRepository)
        {
            this._configService = configService;
            this._fileLimitDomainService = fileLimitDomainService;
            this._repository = repository;
            this._s3Repository = s3Repository;
            this._fileContentRepository = fileContentRepository;
        }

        public File Create(FileCreateBo bo)
        {
            //this._fileLimitDomainService.Check(new CheckFileLimitBo());

            var fileKey = CreateFileKey(bo);
            using (MD5 md5Hash = MD5.Create())
            {
                var file = new File()
                {
                    FileKey = fileKey,
                    SiteId = bo.SiteId,
                    Checksum = Encoding.UTF8.GetBytes(bo.Name),
                    Content = new FileContent()
                    {
                        Checksum = Encoding.UTF8.GetBytes(bo.Name),
                        Name = bo.Name,
                        Content = bo.Content,
                        StorageType = StorageType.Db
                    },
                    CreationTime = DateTime.UtcNow,
                    //Content=bo.Content,
                };
                return _repository.Create(file);
            }
        }

        public File Create(File bo)
        {
            this._repository.Create(bo);

            throw new NotImplementedException();
        }

        public File Get(string fileKey)
        {
            // check expire, delete expire record and throw new FileKeyNotFoundException

            var file = this._repository.Get(fileKey);
            file.Content = this._fileContentRepository.Get((file.Checksum));
            return file;
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

        private string CreateFileKey(FileCreateBo bo)
        {
            using (SHA512 shahash = SHA512.Create())
            {
                Random ran = new Random();

                byte[] filekeyBytes1 = shahash.ComputeHash(Encoding.UTF8.GetBytes(bo.Name + Guid.NewGuid().ToString() + salt1 + ran.Next(1, 10000).ToString()));
                byte[] filekeyBytes2 = shahash.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString() + salt2 + ran.Next(1, 10000).ToString()));
                string fileKey = (Convert.ToBase64String(filekeyBytes1, Base64FormattingOptions.None).TrimEnd('=') + Convert.ToBase64String(filekeyBytes2, Base64FormattingOptions.None).TrimEnd('=')).Replace('+', '-').Replace('/', '_');
                string temp1 = bo.Name + Guid.NewGuid().ToString() + salt1 + ran.Next(1, 10000).ToString();
                string temp2 = Guid.NewGuid().ToString() + salt2 + ran.Next(1, 10000).ToString();
                return fileKey;
            }

        }
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }
}
