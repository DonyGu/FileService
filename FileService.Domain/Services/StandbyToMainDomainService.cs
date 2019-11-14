using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Comm100.Framework.Config;
using Comm100.Framework.SitePlatform;
using FileService.Domain.Entities;
using FileService.Domain.Interfaces;
using FileService.Domain.Specifications;

namespace FileService.Domain.Services
{
    public class StandbyToMainDomainService : IStandbyToMainDomainService
    {
        IFileDomainService _fileDomainService;
        ISitePlatformDomainService _sitePlatformDomainService;
        IConfigService _configService;
        HttpClient _client = new HttpClient();

        public StandbyToMainDomainService(
            IFileDomainService fileDomainService,
            ISitePlatformDomainService sitePlatformDomainService,
            IConfigService configService)
        {
            this._fileDomainService = fileDomainService;
            this._sitePlatformDomainService = sitePlatformDomainService;
            this._configService = configService;
        }

        public void MoveToMain()
        {
            var n = _configService.GetInt("StandbyToMainWorkerNum");
            var files = this._fileDomainService.GetList(n);
            var tasks = files.Select((f) =>
                Task.Run(() =>
                {
                    // should have try-catch
                    MoveFie(f);
                    _fileDomainService.Delete(f.FileKey);
                })).ToArray();

            // wait until all finish
            Task.WaitAll(tasks);

            throw new NotImplementedException();
        }

        private void MoveFie(File file)
        {
            var url = this._sitePlatformDomainService.Get(file.SiteId).MainFileServiceUrl;
            var sharedSecret = this._configService.Get("SharedSecret");
            throw new NotImplementedException();
        }
    }
}