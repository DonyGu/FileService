using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using FileService.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileService.Web.DbToS3
{
    public class DeletedExpiredFilesWorker : BackgroundService
    {
        private IWindsorContainer _container = null;
        public DeletedExpiredFilesWorker(IWindsorContainer container)
        {
            _container = container;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (_container.RequireScope())
                {
                    var deleteExpiredFilesDomainService = _container.Resolve<IDeleteExpiredFilesDomainService>();
                    await deleteExpiredFilesDomainService.DeleteAnExpiredFile();
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
