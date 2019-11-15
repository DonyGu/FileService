using System;
using System.Data.Entity.ModelConfiguration;
using FileService.Domain.Entities;

namespace FileService.Infrastructure.EntityConfigurations
{
    public class FileContentEntityTypeConfigurations : EntityTypeConfiguration<FileContent>
    {
        public void Configure(EntityTypeConfiguration<FileContent> builder)
        {
            builder.ToTable("t_fileService_fileContent");
            builder.HasKey(a => a.Checksum);
        }
    }
}