using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using Comm100.Framework.Common;
using Comm100.Framework.Config;
using FileService.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileService.Web.DbToS3
{
    public class DbToS3Worker : BackgroundService
    {
        private IWindsorContainer _container = null;
        public DbToS3Worker(IWindsorContainer container)
        {
            _container = container;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int count = 0, interval = 0;
            try
            {
                using (_container.RequireScope())
                {
                    IConfigService _configService = _container.Resolve<IConfigService>();
                    count = await _configService.GetInt("DbToS3WorkerNum");
                    interval = await _configService.GetInt("DbToS3WorkerInterval");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, ex.Message);
                throw ex;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (_container.RequireScope())
                    {
                        var dbToS3DomainService = _container.Resolve<IDbToS3DomainService>();
                        await dbToS3DomainService.MoveToS3(count);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, ex.Message);
                }
                await Task.Delay(interval, stoppingToken);

            }
        }
    }
}
