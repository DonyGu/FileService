using System;
using System.Threading.Tasks;
using FileService.Domain.Bo;

namespace FileService.Domain.Interfaces
{
    public interface IDbToS3DomainService
    {
        Task MoveToS3(int count);
    }
}