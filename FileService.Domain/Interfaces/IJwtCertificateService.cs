using Comm100.Framework.Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.Interfaces
{
    public interface IJwtCertificateService
    {
       Task<RsaWithThumbprint> GetPrivateKey();
    }
}
