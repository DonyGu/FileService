using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Comm100.Framework.Common;
using Comm100.Framework.Config;
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
            var count = _configService.GetInt("StandbyToMainWorkerNum");
            var spec = new FileFilterSpecification(StorageType.Db);
            spec.ApplyPaging(1, 1);
            var files = this._fileDomainService.GetList(spec);
            var tasks = files.Select((f) =>
                Task.Run(() =>
                {
                    try
                    {
                        MoveFile(f);
                        _fileDomainService.Delete(f.FileKey);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                })).ToArray();
            // wait until all finish
            Task.WaitAll(tasks);
        }

        private void MoveFile(File file)
        {
            var url = this._sitePlatformDomainService.Get(file.SiteId).MainFileServiceUrl;
            var sharedSecret = this._configService.Get("SharedSecret");
            var statusCode = HttpStatusCode.OK;
            try
            {
                CallApiHelper.CallApi<string>(out statusCode, url, HttpMethod.Post, sharedSecret, JsonConvert.SerializeObject(file));
            }
            catch (Exception)
            {
                if (statusCode == HttpStatusCode.BadRequest)
                {

                }
                else
                {
                    throw;

                }
            }
        }
    }
}