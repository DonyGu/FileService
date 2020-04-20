using Comm100.Framework.Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FileService.Domain.Interfaces
{
    public interface IJwtCertificateService
    {
        RSA GetPrivateKey();
    }
}
