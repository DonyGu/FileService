using FileService.Application.Dto;
using Comm100.Framework.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Application.Interfaces
{
    public interface IFileAuthService
    {
        // throw AuthorizationException when jwt not signed correctly
        Task<FileServiceJwt> VerifyJwt(AuthJwt auth);

        // throw AuthorizationException when SharedSecret is not match or IP not in white list
        Task VerifyComm100Platform(AuthComm100Platform auth);

        Task<string> GenerateToken(JwtPayloadDto jwtPayloadDto);
    }

    public class FileServiceJwt
    {
        public int SiteId { get; set; }
        public int ExpireInDays { get; set; }
        public string AppId { get; set; }
    }
}