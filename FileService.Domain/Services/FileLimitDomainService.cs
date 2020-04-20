using System;
using System.Linq;
using Comm100.Framework.Common;
using Comm100.Framework.Config;
using Comm100.Framework.Domain.Repository;
using Comm100.Framework.Exceptions;
using FileService.Domain.Bo;
using FileService.Domain.Entities;
using FileService.Domain.Interfaces;

namespace FileService.Domain.Services
{
    public class FileLimitDomainService : IFileLimitDomainService
    {
        private readonly IConfigService _configService;
        private readonly IRepository<string, FileLimit> _repository;
        public FileLimitDomainService(IConfigService configService, IRepository<string, FileLimit> repository)
        {
            this._configService = configService;
            this._repository = repository;
        }

        public void Check(CheckFileLimitBo bo)
        {
            CheckSize(bo);
            CheckBlackList(bo);
        }

        private void CheckSize(CheckFileLimitBo bo)
        {
            var maxSize = this._repository.Get(bo.AppId).Result.MaxSize;

            if (bo.Content.Length > maxSize)
            {
                throw new FileTooLargeException(maxSize);
            }
        }

        private void CheckBlackList(CheckFileLimitBo bo)
        {
            var blackList = _configService.GetJson<string[]>("FileBlackList");
            var files = ExpandFile(bo);

            // throw exception if not all files pass test
            if (!files.All(f => FileHelper.CheckFileNameLegitimacy(f.name, f.content, blackList)))
            {
                throw new FileNotAllowedException();
            }
        }

        private NameAndContent[] ExpandFile(CheckFileLimitBo bo)
        {
            // how deep?
            //throw new NotImplementedException();
            var result = new NameAndContent();
            result.name = bo.Name;
            result.content = bo.Content;
            return new NameAndContent[] { result };
        }

        class NameAndContent
        {
#pragma warning disable 0649
            public string name;
            public byte[] content;
#pragma warning restore 0649
        }
    }
}