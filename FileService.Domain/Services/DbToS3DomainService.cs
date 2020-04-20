using System;
using FileService.Domain.Bo;
using FileService.Domain.Entities;
using FileService.Domain.Interfaces;
using Comm100.Framework.Config;
using System.Threading.Tasks;
using FileService.Domain.Specifications;
using System.Linq;
using Comm100.Framework.Domain.Repository;
using Comm100.Framework.Common;
using Castle.Windsor;
using Castle.MicroKernel.Lifestyle;

namespace FileService.Domain.Services
{
    public class DbToS3DomainService : IDbToS3DomainService
    {
        IConfigService _configService;
        IFileDomainService _fileDomainService;
        IWindsorContainer _container;

        public DbToS3DomainService(
            IConfigService configService,
            IFileDomainService fileDomainService,
            IWindsorContainer container)
        {
            this._configService = configService;
            this._fileDomainService = fileDomainService;
            this._container = container;
        }

        public void MoveToS3()
        {
            var count = this._configService.GetInt("DbToS3WorkerNum");
            var spec = new FileFilterSpecification(StorageType.Db);
            spec.ApplyPaging(1, count);
            spec.AddContentInclude();
            var files = this._fileDomainService.GetList(spec);
            var tasks = files.Select((f) =>
                Task.Run(() =>
                {
                    try
                    {
                        if (f.ExpireTime <= DateTime.UtcNow)
                        {
                            this._fileDomainService.Delete(f.FileKey);
                            return;
                        }
                        this._fileDomainService.MoveToRemote(f);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.ErrorLog(ex.Message, ex);
                    }

                })).ToArray();

            // wait until all finish
            Task.WaitAll(tasks);
        }

    }
}