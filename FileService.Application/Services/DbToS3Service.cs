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
using Comm100.Framework.Common;

namespace FileService.Application.Services
{
    public class DbToS3Service //: IDbToS3Service
    {
        //private readonly IDbToS3DomainService _dbToS3DomainService;
        //private readonly IDeleteExpiredFilesDomainService _deleteExpiredFilesDomainService;
        //private readonly ThreadStartOnce _thread;

        //public DbToS3Service(IDbToS3DomainService dbToS3DomainService, IDeleteExpiredFilesDomainService deleteExpiredFilesDomainService)
        //{
        //    this._dbToS3DomainService = dbToS3DomainService;
        //    _deleteExpiredFilesDomainService = deleteExpiredFilesDomainService;
        //    this._thread = new ThreadStartOnce(new Thread(Run));
        //}

        //private void Run()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            this._dbToS3DomainService.MoveToS3().Wait();
        //            this._deleteExpiredFilesDomainService.DeleteAnExpiredFile().Wait();
        //        }
        //        catch (Exception ex)
        //        {
        //            LogHelper.ErrorLog(ex.Message, ex);
        //        }
        //        Thread.Sleep(1000);
        //    }
        //}

        //public void Start()
        //{
        //    if (this._thread.Start())
        //    {
        //        LogHelper.ErrorLog("DbToS3 start.");
        //    }
        //    else
        //    {
        //        LogHelper.ErrorLog("DbToS3 already started before.");
        //    }
        //}
    }
}