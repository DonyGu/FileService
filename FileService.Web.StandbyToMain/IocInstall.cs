using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Comm100.Framework.Config;
using Comm100.Framework.Infrastructure;
using FileService.Application;
using FileService.Infrastructure;

namespace FileService.Web.StandbyToMain
{
    public class IocInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<InfrastructureIocFacility>();
            container.AddFacility<EFIocFacility>();
            container.AddFacility<ConfigIocFacility>();
            container.AddFacility<ApplicationIocFacility>();
        }
    }
}
