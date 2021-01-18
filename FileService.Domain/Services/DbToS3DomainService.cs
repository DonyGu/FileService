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
        IFileDomainService _fileDomainService;
        IWindsorContainer _container;

        public DbToS3DomainService(
            IFileDomainService fileDomainService,
            IWindsorContainer container)
        {
            this._fileDomainService = fileDomainService;
            this._container = container;
        }

        public async Task MoveToS3(int count)
        {
            //var spec = new FileFilterSpecification(StorageType.Db);
            //spec.ApplyPaging(1, count);
            //spec.AddContentInclude();
            var files = this._fileDomainService.GetTopInDb(count);
            LogHelper.Debug($"task begin: {DateTime.UtcNow}");
            var tasks = files.Select((f) =>
                Task.Run(async () =>
                {
                    using (_container.BeginScope())
                    {
                        try
                        {
                            LogHelper.Debug($"move file: {DateTime.UtcNow} ({f.FileKey})");
                            var fileDomainService = _container.Resolve<IFileDomainService>();
                            if (f.ExpireTime <= DateTime.UtcNow)
                            {
                                await fileDomainService.Delete(f);
                                return;
                            }
                            await fileDomainService.MoveToRemote(f);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(ex,ex.Message);
                        }
                    }

                })).ToArray();
            // wait until all finish
            Task.WaitAll(tasks);
            LogHelper.Debug($"task end: {DateTime.UtcNow}");

        }

    }
}