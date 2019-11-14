using System;
using FileService.Domain.Bo;

namespace FileService.Domain.Interfaces
{
    // use AWS SDK in side
    public interface IS3Repository
    {
        void Delete(S3SettingsBo settings, string fileKey);
        void Put(S3SettingsBo settings, S3FileBo file);
    }
}