using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using FileService.Application.Interfaces;
using FileService.Application.Services;
using FileService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.Application
{
    public class ApplicationIocFacility : AbstractFacility
    {
        protected override void Init()
        {
            DomainIocInitializer.Init(Kernel);

            Kernel.Register(
               Component.For(typeof(IDbToS3Service)).ImplementedBy(typeof(DbToS3Service))
                        .LifestyleScoped(),
               Component.For(typeof(IDeleteExpriedFilesService)).ImplementedBy(typeof(DeleteExpiredFilesService))
                            .LifestyleScoped(),
               Component.For(typeof(IFileAppService)).ImplementedBy(typeof(FileAppService))
                            .LifestyleScoped(),
               Component.For(typeof(IFileAuthService)).ImplementedBy(typeof(FileAuthService))
                            .LifestyleScoped(),
               Component.For(typeof(IFileRateLimitingService)).ImplementedBy(typeof(FileRateLimitingService))
                            .LifestyleScoped(),
               Component.For(typeof(IStandbyToMainService)).ImplementedBy(typeof(StandbyToMainService))
                            .LifestyleScoped()
                );

        }
    }
}
