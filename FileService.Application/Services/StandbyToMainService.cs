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
                this._standbyToMainDomainService.MoveToMain();
                Thread.Sleep(1000);
            }
        }

        public void Start()
        {
            this._thread.Start();
        }
    }
}