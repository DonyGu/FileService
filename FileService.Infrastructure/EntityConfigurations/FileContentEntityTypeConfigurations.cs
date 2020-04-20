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
            builder.Property(c => c.StorageType).HasColumnType("smallint");
            builder.HasKey(c => c.Checksum);
            builder.HasMany(c => c.Files).WithOne(f => f.Content).HasPrincipalKey(c => c.Checksum).HasForeignKey(f => f.Checksum);
        }
    }
}