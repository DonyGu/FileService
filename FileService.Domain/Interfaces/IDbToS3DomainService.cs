using System;
using FileService.Domain.Bo;

namespace FileService.Domain.Interfaces
{
    public interface IDbToS3DomainService
    {
        void MoveToS3();
    }
}