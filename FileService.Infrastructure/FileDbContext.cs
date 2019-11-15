using Comm100.Framework.Config;
using FileService.Domain.Entities;
using FileService.Infrastructure.EntityConfigurations;
using System.Configuration;
using System.Data.Entity;

namespace FileService.Infrastructure
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(string nameOrConnectionString) :base(nameOrConnectionString)
        {
            //Database.SetInitializer<FileDbContext>(null);
        }

        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<FileContent> FileContent { get; set; }
        public virtual DbSet<Config> Config { get; set; }
        public virtual DbSet<FileLimit> FileLimit { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ConfigEntityTypeConfigurations());
            modelBuilder.Configurations.Add(new FileContentEntityTypeConfigurations());
            modelBuilder.Configurations.Add(new FileEntityTypeConfigurations());
            modelBuilder.Configurations.Add(new FileLimitEntityTypeConfigurations());
        }
    }
}