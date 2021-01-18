using Comm100.Framework.Security;
using FileService.Application.Dto;
using FileService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Interfaces
{
    public interface IFileAppService
    {
        // upload file from browser 
        Task<FileDto> Upload(FileUploadDto dto);

        // for moving back from standby file service
        Task<FileDto> Create(FileCreateDto dto);

        // browser download file
        Task<FileDto> Get(string fileKey);

        Task Delete(FileDeleteDto dto);

        Task<List<FileDto>> Monitor(AuthJwt authJwt, int count);
    }
}
