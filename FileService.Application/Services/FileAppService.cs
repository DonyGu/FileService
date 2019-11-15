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

namespace FileService.Application.Services
{
    public class FileAppService : IFileAppService
    {
        private readonly IFileDomainService _fileDomainService;
        private readonly IFileAuthService _fileAuthService;
        private readonly IConfigService _configService;
        private readonly IFileRateLimitingService _rateLimitingService;

        public FileAppService(IFileDomainService fileDomainService,
            IFileAuthService authService,
            IConfigService configService,
            IFileRateLimitingService rateLimitingService)
        {
            this._fileDomainService = fileDomainService;
            this._configService = configService;
            this._fileAuthService = authService;
            this._rateLimitingService = rateLimitingService;
        }

        public FileDto Upload(FileUploadDto dto)
        {
            //this._rateLimitingService.CheckUpload(dto.Auth.IP);

            //var jwtResult = this._fileAuthService.VerifyJwt(dto.Auth);

            this._fileDomainService.Create(new FileCreateBo()
            {
                SiteId=0,
                Name=dto.Name,
                Content=dto.Content,
                AppId="hosted"
            });
            throw new NotImplementedException();
        }

        public FileDto Create(FileCreateDto dto)
        {
            //this._fileAuthService.VerifyComm100Platform(dto.Auth);

            this._fileDomainService.Create(new File());
            throw new NotImplementedException();
        }

        public FileDto Get(FileGetDto dto)
        {
            this._rateLimitingService.CheckGet(dto.FileKey, dto.IP);

            this._fileDomainService.Get(dto.FileKey);
            throw new NotImplementedException();
        }

        public void Delete(FileDeleteDto dto)
        {
            this._fileAuthService.VerifyComm100Platform(dto.Auth);

            this._fileDomainService.Delete(dto.FileKey);
            throw new NotImplementedException();
        }
    }
}
