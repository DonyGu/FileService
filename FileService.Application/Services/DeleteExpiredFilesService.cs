using System.Threading;
using Comm100.Framework;
using FileService.Application.Interfaces;
using FileService.Domain.Interfaces;

namespace FileService.Application.Services
{
    public class DeleteExpiredFilesService : IDeleteExpriedFilesService
    {
        private readonly IDeleteExpiredFilesDomainService _deleteExpiredFilesDomainService;
        private readonly ThreadStartOnce _thread;

        public DeleteExpiredFilesService(IDeleteExpiredFilesDomainService deleteExpiredFilesDomainService)
        {
            this._deleteExpiredFilesDomainService = deleteExpiredFilesDomainService;
            this._thread = new ThreadStartOnce(new Thread(Run));
        }

        private void Run()
        {
            while (true)
            {
                this._deleteExpiredFilesDomainService.DeleteAnExpiredFile();
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