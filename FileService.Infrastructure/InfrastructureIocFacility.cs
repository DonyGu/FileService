using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using FileService.Domain.Interfaces;
using FileService.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace FileService.Infrastructure
{
    public class InfrastructureIocFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Register(
               Component.For(typeof(DbContext))
                        .ImplementedBy(typeof(FileDbContext))
                        .LifestyleScoped(),
               Component.For(typeof(IS3Repository))
                        .ImplementedBy(typeof(S3Repository))
                        .LifestyleScoped()
                );
        }
    }
}
