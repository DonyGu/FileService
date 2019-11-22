using System;
using System.Collections.Generic;
using System.Text;
// using Comm100.Extension;
using FileService.Application.Dto;
using FileService.Domain.Entities;
using FileService.Domain.Interfaces;
using FileService.Domain.Bo;
using FileService.Domain.Specifications;
using Comm100.Framework.Config;
using FileService.Application.Interfaces;
using System.Security.Cryptography;
using System.Linq;

namespace FileService.Application.Services
{
    public class FileAppService : IFileAppService
    {
        private readonly IFileDomainService _fileDomainService;
        private readonly IFileAuthService _fileAuthService;
        private readonly IConfigService _configService;
        private const string salt1 = "ADA4CDE7-C963-4FBC-BC5D-B13A85AF4A1D";
        private const string salt2 = "89439DB7-7548-4E4C-9816-34D3D1829190";

        public FileAppService(IFileDomainService fileDomainService,
            IFileAuthService authService,
            IConfigService configService)
        {
            this._fileDomainService = fileDomainService;
            this._configService = configService;
            this._fileAuthService = authService;
        }

        public FileDto Upload(FileUploadDto dto)
        {
            //var jwtResult = this._fileAuthService.VerifyJwt(dto.Auth);

            var file = FileMapping(dto);
            var result = this._fileDomainService.Create(file);
            return FileDtoMapping(result);
        }

        public FileDto Create(FileCreateDto dto)
        {
            //this._fileAuthService.VerifyComm100Platform(dto.Auth);
            if (_fileDomainService.Exist(dto.File.FileKey))
            {
                throw new Exception("FileKey exist");
            }
            var file = FileMapping(dto);
            var result = this._fileDomainService.Create(file);
            return FileDtoMapping(result);
        }

        public FileDto Get(string fileKey)
        {
            var result = this._fileDomainService.Get(fileKey);
            return FileDtoMapping(result);
        }

        public void Delete(FileDeleteDto dto)
        {
            this._fileAuthService.VerifyComm100Platform(dto.Auth);

            this._fileDomainService.Delete(dto.FileKey);
            throw new NotImplementedException();
        }

        private File FileMapping(FileCreateDto dto)
        {
            var checksum = CalCulateChecksum(dto.File.Name, dto.File.Content);
            return new File()
            {
                FileKey = dto.File.FileKey,
                SiteId = 0,
                Checksum = checksum,
                Content = new FileContent()
                {
                    Checksum = checksum,
                    Name = dto.File.Name,
                    Content = dto.File.Content,
                    StorageType = StorageType.Db,
                    Link = ""
                },
                CreationTime = DateTime.UtcNow,
            };
        }
        private File FileMapping(FileUploadDto dto)
        {
            var fileKey = CreateFileKey(dto.Name);
            var checksum = CalCulateChecksum(dto.Name, dto.Content);
            return new File()
            {
                FileKey = fileKey,
                SiteId = 0,
                Checksum = checksum,
                Content = new FileContent()
                {
                    Checksum = checksum,
                    Name = dto.Name,
                    Content = dto.Content,
                    StorageType = StorageType.Db,
                    Link = ""
                },
                CreationTime = DateTime.UtcNow,
            };
        }
        private FileDto FileDtoMapping(File file)
        {
            return new FileDto
            {
                FileKey = file.FileKey,
                SiteId = file.SiteId,
                Name = file.Content.Name,
                Content = file.Content.Content
            };
        }
        private string CreateFileKey(string name)
        {
            using (SHA512 shahash = SHA512.Create())
            {
                Random ran = new Random();

                byte[] filekeyBytes1 = shahash.ComputeHash(Encoding.UTF8.GetBytes(name + Guid.NewGuid().ToString() + salt1 + ran.Next(1, 10000).ToString()));
                byte[] filekeyBytes2 = shahash.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString() + salt2 + ran.Next(1, 10000).ToString()));
                string fileKey = (Convert.ToBase64String(filekeyBytes1, Base64FormattingOptions.None).TrimEnd('=') + Convert.ToBase64String(filekeyBytes2, Base64FormattingOptions.None).TrimEnd('=')).Replace('+', '-').Replace('/', '_');
                string temp1 = name + Guid.NewGuid().ToString() + salt1 + ran.Next(1, 10000).ToString();
                string temp2 = Guid.NewGuid().ToString() + salt2 + ran.Next(1, 10000).ToString();
                return fileKey;
            }

        }
        private byte[] CalCulateChecksum(string name, byte[] content)
        {
            using (var md5Hash = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
            {
                md5Hash.AppendData(content);
                md5Hash.AppendData(Encoding.UTF8.GetBytes(name));
                byte[] result1 = md5Hash.GetHashAndReset();
                md5Hash.AppendData(new byte[] { 0 });
                byte[] result2 = md5Hash.GetHashAndReset();
                return result1.Concat(result2).ToArray();
            }
        }
    }
}
