using System;
using FileService.Application.Dto;
using FileService.Application.Interfaces;
using Comm100.Framework.Config;
using Comm100.Framework.Security;

namespace FileService.Application.Services
{
    public class FileAuthService : IFileAuthService
    {
        IConfigService _configService;

        public FileAuthService(IConfigService configService)
        {
            this._configService = configService;
        }

        public FileServiceJwt VerifyJwt(AuthJwt auth)
        {
            // check signature
            // check IP
            var publicKey = this._configService.Get("JWTPublicKey");
            throw new NotImplementedException();
        }

        public void VerifyComm100Platform(AuthComm100Platform auth)
        {
            VerifySharedSecret(auth.SharedSecret);
            VerifyIP(auth.IP);
        }

        private void VerifySharedSecret(string clientSharedSecret)
        {
            var sharedSecret = _configService.Get("SharedSecret");
            throw new NotImplementedException();
        }

        private void VerifyIP(string ip)
        {
            var whiteList = _configService.GetJson<IPRange[]>("IPWhiteList");
            throw new NotImplementedException();
        }

        class IPRange
        {
            public string From { get; set; }
            public string To { get; set; }
        }
    }
}