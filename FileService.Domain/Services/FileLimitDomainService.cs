using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task Check(CheckFileLimitBo bo)
        {
            await CheckSize(bo);
            await CheckBlackList(bo);
        }

        private async Task CheckSize(CheckFileLimitBo bo)
        {
            var fileLimit = await this._repository.Get(bo.AppId);

            if (bo.Content.Length > fileLimit.MaxSize)
            {
                throw new FileTooLargeException(fileLimit.MaxSize);
            }
        }

        private async Task CheckBlackList(CheckFileLimitBo bo)
        {
            var blackList =await _configService.GetJson<string[]>("FileBlackList");
            var files = ExpandFile(bo);
            FileLegitimacyChecker fileLegitimacyChecker = new FileLegitimacyChecker(blackList);
             
            // throw exception if not all files pass test
            if (!files.All(f => fileLegitimacyChecker.FileLegitimacyCheck(f.name, f.content)))
            {
                throw new FileNotAllowedException();
            }
            return ;

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