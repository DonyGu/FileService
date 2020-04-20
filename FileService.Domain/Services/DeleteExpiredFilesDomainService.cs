using System;
using System.Linq;
using System.Threading.Tasks;
using Comm100.Framework.Common;
using FileService.Domain.Interfaces;
using FileService.Domain.Specifications;

namespace FileService.Domain.Services
{
    public class DeleteExpiredFilesDomainService : IDeleteExpiredFilesDomainService
    {
        IFileDomainService _fileDomainService;
        
        public DeleteExpiredFilesDomainService(IFileDomainService fileDomainService)
        {
            this._fileDomainService = fileDomainService;
        }
        public async Task DeleteAnExpiredFile()
        {
            var spec = new FileFilterSpecification(DateTime.UtcNow);
            spec.ApplyPaging(1, 1);
            spec.AddContentInclude();
            var files = this._fileDomainService.GetList(spec);
            if (files.Count>0)
            {
                foreach (var item in files)
                {
                    try
                    {
                        this._fileDomainService.Delete(item.FileKey);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.ErrorLog(ex.Message, ex);
                    }
                }
            }
        }
    }
}