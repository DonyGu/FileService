using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Text;
using FileService.Domain.Interfaces;
using FileService.Domain.Services;

namespace FileService.Domain
{
    public class DomainIocInitializer
    {
        public static void Init(IKernel kernel)
        {
            kernel.Register(
               Component.For(typeof(IDbToS3DomainService)).ImplementedBy(typeof(DbToS3DomainService))
                        .LifestyleScoped(),
               Component.For(typeof(IDeleteExpiredFilesDomainService)).ImplementedBy(typeof(DeleteExpiredFilesDomainService))
                            .LifestyleScoped(),
               Component.For(typeof(IFileDomainService)).ImplementedBy(typeof(FileDomainService))
                            .LifestyleScoped(),
               Component.For(typeof(IFileLimitDomainService)).ImplementedBy(typeof(FileLimitDomainService))
                            .LifestyleScoped(),
               Component.For(typeof(IStandbyToMainDomainService)).ImplementedBy(typeof(StandbyToMainDomainService))
                            .LifestyleScoped(),
               Component.For(typeof(IJwtCertificateService)).ImplementedBy(typeof(JwtCertificateService))
                            .LifestyleScoped()
                );
        }
    }
}
