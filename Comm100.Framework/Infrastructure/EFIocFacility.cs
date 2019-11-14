using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Comm100.Framework.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Infrastructure
{
    public class EFIocFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Register(
                Component.For(typeof(IRepository<,>))
                         .ImplementedBy(typeof(EFRepository<,>))
                         .LifestyleScoped()
                         );
        }
    }
}
