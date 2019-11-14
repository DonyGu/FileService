using FileService.Domain.Entities;
using FileService.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FileService.Infrastructure
{
    public class FileDbContext : DbContext
    {
        public string connectString { get; set; }
        public FileDbContext(IConfiguration configuration)
        {
            this.connectString = configuration.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectString);
        }

        public virtual DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FileEntityTypeConfigurations());
        }
    }
}