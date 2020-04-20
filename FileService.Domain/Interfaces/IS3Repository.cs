using System;
using System.Threading.Tasks;
using FileService.Domain.Bo;

namespace FileService.Domain.Interfaces
{
    // use AWS SDK in side
    public interface IS3Repository
    {
        Task Delete(S3SettingsBo settings, string fileKey);
        Task Put(S3SettingsBo settings, S3FileBo file);
    }
}