using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Facilities.AspNetCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Comm100.Framework.Common;
using Comm100.Framework.Exceptions;
using FileService.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileService.Web.StandbyToMain
{
    public class Startup
    {
        private static readonly WindsorContainer Container = new WindsorContainer();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Container.AddFacility<AspNetCoreFacility>(f => f.CrossWiresInto(services));
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add(typeof(GlobalExceptions));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSingleton<IWindsorContainer>(Container);
            RegisterApplicationComponents(services);
            services.AddHostedService<StandByToMainWorker>();
            return services.AddWindsor(Container,
                opts => opts.UseEntryAssembly(this.GetType().Assembly));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use((context, next) => {
                context.Request.PathBase = "/fileservicestandbytomain";
                return next();
            });

            app.UsePathBase("/fileservicestandbytomain");
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
            Container.Register(Component.For<DbContext>().ImplementedBy<FileDbContext>().LifestyleScoped());
            Container.Install(new IocInstaller());
        }
    }
}
