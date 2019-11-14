using System;
using FileService.Domain.Bo;
using FileService.Domain.Interfaces;

namespace FileService.Infrastructure.Repositories
{
    public class S3Repository : IS3Repository
    {
        public void Delete(S3SettingsBo settings, string fileKey)
        {
            throw new NotImplementedException();
        }

        public void Put(S3SettingsBo settings, S3FileBo file)
        {
            throw new NotImplementedException();
        }
    }
}