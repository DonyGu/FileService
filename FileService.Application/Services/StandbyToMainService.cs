using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Comm100.Framework.Config;
using Comm100.Framework.SitePlatform;
using FileService.Application.Interfaces;
using FileService.Domain.Entities;
using FileService.Domain.Interfaces;
using FileService.Domain.Specifications;
using System.Threading;
using Comm100.Framework;
using Comm100.Framework.Common;

namespace FileService.Application.Services
{
    public class StandbyToMainService : IStandbyToMainService
    {
        private readonly IStandbyToMainDomainService _standbyToMainDomainService;
        private ThreadStartOnce _thread;

        public StandbyToMainService(IStandbyToMainDomainService standbyToMainDomainService)
        {
            this._standbyToMainDomainService = standbyToMainDomainService;
            this._thread = new ThreadStartOnce(new Thread(Run));
        }

        private void Run()
        {
            while (true)
            {
                try
                {
                    this._standbyToMainDomainService.MoveToMain().Wait();
                }
                catch (Exception ex)
                {

                    LogHelper.Error(ex, ex.Message);
                }
                Thread.Sleep(1000);
            }
        }

        public void Start()
        {
            if (this._thread.Start())
            {
                LogHelper.Info("DbToStandBy start.");
            }
            else
            {
                LogHelper.Info("DbToStandBy already started before.");
            }
        }
    }
}