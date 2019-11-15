using FileService.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace FileService.Infrastructure.EntityConfigurations
{
    public class FileEntityTypeConfigurations : EntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeConfiguration<File> builder)
        {
            
            builder.ToTable("t_fileService_file");
            builder.HasKey(a => a.FileKey);
        }
    }
}