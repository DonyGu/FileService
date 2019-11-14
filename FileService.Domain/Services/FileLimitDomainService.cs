using System;
using System.Linq;
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
            throw new System.NotImplementedException();
        }

        private void CheckSize(CheckFileLimitBo bo)
        {
            var maxSize = this._repository.Get(bo.AppId).MaxSize;
            throw new NotImplementedException();
        }

        private void CheckBlackList(CheckFileLimitBo bo)
        {
            var blackList = _configService.GetJson<string[]>("FileBlackList");
            var files = ExpandFile(bo);

            // throw exception if not all files pass test
            if (!files.All(f => TestFile(f, blackList)))
            {
                throw new FileNotAllowedException();
            }
        }

        private NameAndContent[] ExpandFile(CheckFileLimitBo bo)
        {
            // how deep?
            throw new NotImplementedException();
        }

        private bool TestFile(NameAndContent f, string[] blackList)
        {
            if (blackList.Any(blob => IsMatch(f.name, blob)))
            {
                return false;
            }

            if (IsExe(f.content))
            {
                return false;
            }

            return true;
        }

        private bool IsMatch(string name, string blob)
        {
            throw new NotImplementedException();
        }

        private bool IsExe(byte[] content)
        {
            throw new NotImplementedException();
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