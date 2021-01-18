using Comm100.Framework.Common;
using Comm100.Framework.Infrastructure;
using FileService.Domain.Entities;
using FileService.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FileService.Infrastructure
{
    public class FileDbContext : DbContext
    {
        public string ConnectionString { get; private set; }
        public FileDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetDatabaseConnectionString("Configuration", "File");
            this.ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
            //optionsBuilder.AddInterceptors(new HintInterceptor());
        }

        public virtual DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ConfigEntityTypeConfigurations());
            modelBuilder.ApplyConfiguration(new FileContentEntityTypeConfigurations());
            modelBuilder.ApplyConfiguration(new FileEntityTypeConfigurations());
            modelBuilder.ApplyConfiguration(new FileLimitEntityTypeConfigurations());
        }
    }
}