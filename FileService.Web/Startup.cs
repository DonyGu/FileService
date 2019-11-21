using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.AspNetCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FileService.Application.Interfaces;
using FileService.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Comm100.Web.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
               // options.Filters.Add(typeof(Comm100ExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(token =>
            {
                token.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("SecretKey")),
                    ValidateIssuer = true,
                    ValidIssuers = new string[] { "http://localhost:45092/" },
                    ValidateAudience = true,
                    ValidAudience = "http://localhost:45092/",
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

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
