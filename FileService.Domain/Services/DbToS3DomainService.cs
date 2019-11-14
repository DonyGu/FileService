using System;
using FileService.Domain.Bo;
using FileService.Domain.Entities;
using FileService.Domain.Interfaces;
using Comm100.Framework.Config;
using System.Threading.Tasks;
using FileService.Domain.Specifications;
using System.Linq;
using Comm100.Framework.Domain.Repository;

namespace FileService.Domain.Services
{
    public class DbToS3DomainService : IDbToS3DomainService
    {
        IConfigService _configService;
        IFileDomainService _fileDomainService;
        IS3Repository _s3Repository;
        IRepository<byte[], FileContent> _linkRepository;

        public DbToS3DomainService(
            IConfigService configService,
            IFileDomainService fileDomainService,
            IS3Repository s3Repository,
            IRepository<byte[], FileContent> linkRepository)
        {
            this._configService = configService;
            this._fileDomainService = fileDomainService;
            this._s3Repository = s3Repository;
            this._linkRepository = linkRepository;
        }

        public void MoveToS3()
        {
            var n = this._configService.GetInt("DbToS3WorkerNum");
            var files = this._fileDomainService.GetFileContentList(new FileContentFilterSpecification(n, StorageType.Db));
            var tasks = files.Select((f) =>
                Task.Run(() =>
                {
                    if (f.ExpireTime <= DateTime.UtcNow)
                    {
                        this._fileDomainService.Delete(f.FileKey);
                        return;
                    }
                    this._fileDomainService.MoveToRemote(f.FileKey);
                })).ToArray();

            // wait until all finish
            Task.WaitAll(tasks);

            throw new NotImplementedException();
        }
    }
}