using System;
using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrastructure.EntityConfigurations
{
    public class FileContentEntityTypeConfigurations : IEntityTypeConfiguration<FileContent>
    {
        public void Configure(EntityTypeBuilder<FileContent> builder)
        {
            builder.ToTable("t_fileService_fileContent");
            builder.HasKey(a => a.Checksum);
        }
    }
}