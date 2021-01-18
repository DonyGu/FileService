using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using FileService.Domain;
using FileService.Domain.Interfaces;
using FileService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.Infrastructure
{
    public class InfrastructureIocFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Register(
               //Component.For(typeof(DbContext))
               //         .ImplementedBy(typeof(FileDbContext))
               //         .LifestylePerThread(),
               Component.For(typeof(IS3Repository))
                        .ImplementedBy(typeof(S3Repository))
                        .LifestyleScoped(),
               Component.For(typeof(IFileRepository))
                    .ImplementedBy(typeof(FileRepository))
                        .LifestyleScoped()
                );
        }
    }
}
