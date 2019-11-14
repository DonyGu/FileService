using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FileLimitEntityTypeConfigurations : IEntityTypeConfiguration<FileLimit>
{
    public void Configure(EntityTypeBuilder<FileLimit> builder)
    {
        builder.ToTable("t_fileService_fileLimit");
        builder.HasKey(a => a.AppId);

        throw new System.NotImplementedException();
    }
}