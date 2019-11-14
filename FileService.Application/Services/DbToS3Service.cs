using System;
using FileService.Application.Interfaces;
using FileService.Domain.Entities;
using Comm100.Framework.Config;
using System.Threading.Tasks;
using System.Threading;
using FileService.Application.Dto;
using System.Linq;
using FileService.Domain.Interfaces;
using Comm100.Framework;

namespace FileService.Application.Services
{
    public class DbToS3Service : IDbToS3Service
    {
        private readonly IDbToS3DomainService _dbToS3DomainService;
        private readonly ThreadStartOnce _thread;

        public DbToS3Service(IDbToS3DomainService dbToS3DomainService)
        {
            this._dbToS3DomainService = dbToS3DomainService;
            this._thread = new ThreadStartOnce(new Thread(Run));
        }

        private void Run()
        {
            while (true)
            {
                this._dbToS3DomainService.MoveToS3();
                Thread.Sleep(1000);
            }
        }

        public void Start()
        {
            if (this._thread.Start())
            {
                // log start now
            }
            else
            {
                // log already started before
            }
        }
    }
}