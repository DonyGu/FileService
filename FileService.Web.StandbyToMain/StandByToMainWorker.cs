using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using FileService.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileService.Web.StandbyToMain
{
    public class StandByToMainWorker : BackgroundService
    {
        private IWindsorContainer _container = null;
        public StandByToMainWorker(IWindsorContainer container)
        {
            _container = container;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (_container.RequireScope())
                {
                    var standbyToMainDomainService = _container.Resolve<IStandbyToMainDomainService>();
                    await standbyToMainDomainService.MoveToMain();
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
