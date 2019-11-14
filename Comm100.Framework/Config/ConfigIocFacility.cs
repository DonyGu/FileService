using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Config
{
    public class ConfigIocFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Register(
                Component.For(typeof(IConfigService))
                         .ImplementedBy(typeof(ConfigService))
                         .LifestyleScoped()
                         );
        }
    }
}
