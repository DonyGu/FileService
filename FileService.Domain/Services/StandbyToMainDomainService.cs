using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using Comm100.Framework.Common;
using Comm100.Framework.Config;
using Comm100.Framework.DTO;
using Comm100.Framework.SitePlatform;
using FileService.Domain.Entities;
using FileService.Domain.Interfaces;
using FileService.Domain.Specifications;
using Newtonsoft.Json;

namespace FileService.Domain.Services
{
    public class StandbyToMainDomainService : IStandbyToMainDomainService
    {
        IFileDomainService _fileDomainService;
        IConfigService _configService;
        IWindsorContainer _container;

        public StandbyToMainDomainService(
            IFileDomainService fileDomainService,
            IConfigService configService,
            IWindsorContainer container)
        {
            this._fileDomainService = fileDomainService;
            this._configService = configService;
            this._container = container;
        }

        public async Task MoveToMain()
        {
            var count = await _configService.GetInt("StandbyToMainWorkerNum");
            var url = await this._configService.Get("MainServiceUrl");//this._sitePlatformDomainService.Get(file.SiteId).MainFileServiceUrl;
            var sharedSecret = await this._configService.Get("SharedSecret");
            var spec = new FileFilterSpecification(StorageType.Db);
            spec.ApplyPaging(1, count);
            spec.AddContentInclude();
            var files = this._fileDomainService.GetList(spec);

            if (files.Count > 0)
            {
                var tasks = files.Select(async (f) =>
                   await Task.Run(async () =>
                    {
                        LogHelper.Debug($"{DateTime.UtcNow} Begin Move: {f.FileKey}");
                        await MoveFile(f, url, sharedSecret);
                        LogHelper.Debug($"{DateTime.UtcNow} End Move: {f.FileKey}");
                    })).ToArray();
                // wait until all finish
                await Task.WhenAll(tasks);
            }
        }

        public async Task MoveFile(File file, string url, string sharedSecret)
        {
            var callUrl = $"{url}/{file.FileKey}";
            FileSyncDTO fileSyncDTO = new FileSyncDTO
            {
                creationTime = file.CreationTime.ToString(),
                expireTime = file.ExpireTime.ToString(),
                siteId = file.SiteId.ToString(),
            };
            try
            {
                LogHelper.Debug($"{DateTime.UtcNow} Begin call api: {file.FileKey}");
                var httpstatusCode = await CallApiHelper.UploadFile(callUrl, sharedSecret, file.Content.Name, file.Content.Content, fileSyncDTO);
                LogHelper.Debug($"{DateTime.UtcNow} End call api: {file.FileKey}");

                if (httpstatusCode == HttpStatusCode.OK)
                {
                    using (_container.BeginScope())
                    {
                        LogHelper.Debug($"{DateTime.UtcNow} Begin delete: {file.FileKey}");
                        IFileDomainService fileDomainService = _container.Resolve<IFileDomainService>();
                        await fileDomainService.Delete(file);
                        LogHelper.Debug($"{DateTime.UtcNow} End delete: {file.FileKey}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, ex.Message);
            }
        }
    }
}