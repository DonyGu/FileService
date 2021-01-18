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
using Comm100.Framework.Exceptions;
using System.Threading.Tasks;
using Comm100.Framework.Security;

namespace FileService.Application.Services
{
    public class FileAppService : IFileAppService
    {
        private readonly IFileDomainService _fileDomainService;
        private readonly IFileLimitDomainService _fileLimitDomainService;
        private readonly IFileAuthService _fileAuthService;
        private const string salt1 = "ADA4CDE7-C963-4FBC-BC5D-B13A85AF4A1D";
        private const string salt2 = "89439DB7-7548-4E4C-9816-34D3D1829190";

        public FileAppService(IFileDomainService fileDomainService, IFileAuthService authService, IFileLimitDomainService fileLimitDomainService)
        {
            this._fileDomainService = fileDomainService;
            this._fileAuthService = authService;
            this._fileLimitDomainService = fileLimitDomainService;
        }

        public async Task<FileDto> Upload(FileUploadDto dto)
        {
            var jwtResult = await this._fileAuthService.VerifyJwt(dto.Auth);
            await this._fileLimitDomainService.Check(new CheckFileLimitBo() { AppId = jwtResult.AppId, Name = dto.Name, Content = dto.Content });
            var file = FileMapping(dto, jwtResult);
            var result = await this._fileDomainService.Create(file);
            return FileDtoMapping(result);
        }

        public async Task<FileDto> Create(FileCreateDto dto)
        {
            await this._fileAuthService.VerifyComm100Platform(dto.Auth);
            bool ifExists = await _fileDomainService.Exist(dto.File.FileKey);
            if (ifExists)
            {
                throw new FileKeyExistsException();
            }
            var file = FileMapping(dto);
            var result = await this._fileDomainService.Create(file);
            return FileDtoMapping(result);
        }

        public async Task<FileDto> Get(string fileKey)
        {
            var result = await this._fileDomainService.Get(fileKey);
            return FileDtoMapping(result);
        }

        public async Task Delete(FileDeleteDto dto)
        {
            await this._fileAuthService.VerifyComm100Platform(dto.Auth);

            await this._fileDomainService.Delete(dto.FileKey);
        }

        public async Task<List<FileDto>> Monitor(AuthJwt authJwt, int count)
        {
            var jwtResult = await this._fileAuthService.VerifyJwt(authJwt);
            List<FileDto> result = new List<FileDto>();
            var spec = new FileFilterSpecification(StorageType.Db);
            spec.ApplyPaging(1, count);
            var list = this._fileDomainService.GetList(spec);
            foreach (var item in list)
            {
                result.Add(new FileDto { FileKey = item.FileKey, SiteId = item.SiteId, CreationTime = item.CreationTime, ExpireTime = item.ExpireTime });
            }
            return result;
        }

        private File FileMapping(FileCreateDto dto)
        {
            var checksum = CalCulateChecksum(dto.File.Name, dto.File.Content);
            return new File()
            {
                FileKey = dto.File.FileKey,
                SiteId = dto.File.SiteId,
                Checksum = checksum,
                ExpireTime = dto.File.ExpireTime,
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
        private File FileMapping(FileUploadDto dto, FileServiceJwt fileServiceJwt)
        {
            var fileKey = CreateFileKey(dto.Name);
            var checksum = CalCulateChecksum(dto.Name, dto.Content);
            return new File()
            {
                FileKey = fileKey,
                SiteId = fileServiceJwt.SiteId,
                Checksum = checksum,
                ExpireTime = fileServiceJwt.ExpireInDays == 0 ? DateTime.MaxValue : DateTime.UtcNow.AddDays(fileServiceJwt.ExpireInDays),
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
                Content = file.Content.Content,
                StorageType = file.Content.StorageType,
                Link = file.Content.Link,
                CreationTime = file.CreationTime,
                ExpireTime = file.ExpireTime
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
