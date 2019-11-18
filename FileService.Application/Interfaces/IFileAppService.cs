using FileService.Application.Dto;
using FileService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileService.Application.Interfaces
{
    public interface IFileAppService
    {
        // upload file from browser 
        FileDto Upload(FileUploadDto dto);

        // for moving back from standby file service
        FileDto Create(FileCreateDto dto);

        // browser download file
        FileDto Get(string fileKey);

        void Delete(FileDeleteDto dto);
    }
}
