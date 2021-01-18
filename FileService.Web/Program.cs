using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Comm100.Framework.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace FileService.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args, null).Build().Run();
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args, Action<IConfigurationBuilder> configure) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddSecretsManagerConfiguration();
                configure?.Invoke(config);
            })
            .UseNLog()
            .UseStartup<Startup>();
    }
}
