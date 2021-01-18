using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrastructure.EntityConfigurations
{
    public class FileEntityTypeConfigurations : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.ToTable("t_fileService_file");
            builder.HasKey(a => a.FileKey);
            builder.Property(a => a.FileKey).HasColumnType("varchar(172)").IsFixedLength();
        }
    }
}