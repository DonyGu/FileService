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
                        .LifestyleSingleton(),
               Component.For(typeof(IDeleteExpriedFilesService)).ImplementedBy(typeof(DeleteExpiredFilesService))
                            .LifestyleSingleton(),
               Component.For(typeof(IFileAppService)).ImplementedBy(typeof(FileAppService))
                            .LifestyleScoped(),
               Component.For(typeof(IFileAuthService)).ImplementedBy(typeof(FileAuthService))
                            .LifestyleScoped(),
               Component.For(typeof(IStandbyToMainService)).ImplementedBy(typeof(StandbyToMainService))
                            .LifestyleSingleton()
                );
        }
    }
}
