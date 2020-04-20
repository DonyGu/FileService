using System;
using System.IO;
using System.Text;
using Castle.Facilities.AspNetCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Comm100.Framework.Common;
using Comm100.Framework.Exceptions;
using Comm100.Framework.Middleware;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Comm100.Web.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FileService.Web
{

    public class Startup
    {
        private static readonly WindsorContainer Container = new WindsorContainer();
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
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

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder
                    .AllowAnyMethod();
                });
            });

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            RegisterApplicationComponents(services);
            
            return services.AddWindsor(Container,
                opts => opts.UseEntryAssembly(this.GetType().Assembly));
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(MyAllowSpecificOrigins);
            //app.UseMiddleware<RequestSetOptionsMiddleware>();
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
