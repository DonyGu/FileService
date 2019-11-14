using FileService.Application.Dto;
using Comm100.Framework.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileService.Application.Interfaces
{
    public interface IFileAuthService
    {
        // throw AuthorizationException when jwt not signed correctly
        FileServiceJwt VerifyJwt(AuthJwt auth);

        // throw AuthorizationException when SharedSecret is not match or IP not in white list
        void VerifyComm100Platform(AuthComm100Platform auth);
    }

    public class FileServiceJwt
    {
        public int SiteId { get; set; }
        public int ExpireInDays { get; set; }
        public int AppId { get; set; }
    }
}