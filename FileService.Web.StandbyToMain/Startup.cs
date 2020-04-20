using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Facilities.AspNetCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Comm100.Framework.Common;
using Comm100.Framework.Exceptions;
using log4net;
using log4net.Config;
using log4net.Repository;
// using Comm100.Web.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileService.Web.StandbyToMain
{
    public class Startup
    {
        private static readonly WindsorContainer Container = new WindsorContainer();
        public static ILoggerRepository loggerRepository { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            loggerRepository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.ConfigureAndWatch(loggerRepository, new FileInfo("log4net.config"));
            InitRepository.LoggerRepository = loggerRepository;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Container.AddFacility<AspNetCoreFacility>(f => f.CrossWiresInto(services));
            Container.Register(Component.For<IWindsorContainer>().Instance(Container));

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add(typeof(GlobalExceptions));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            RegisterApplicationComponents(services);

            return services.AddWindsor(Container,
                opts => opts.UseEntryAssembly(this.GetType().Assembly));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
        private void RegisterApplicationComponents(IServiceCollection services)
        {
            // Application components
            Container.Register(Component.For<IHttpContextAccessor>().ImplementedBy<HttpContextAccessor>());
            Container.Install(new IocInstaller());
        }
    }
}
