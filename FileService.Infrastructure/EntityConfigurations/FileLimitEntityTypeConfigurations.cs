using FileService.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

public class FileLimitEntityTypeConfigurations : EntityTypeConfiguration<FileLimit>
{
    public void Configure(EntityTypeConfiguration<FileLimit> builder)
    {
        builder.ToTable("t_fileService_fileLimit");
        builder.HasKey(a => a.AppId);
    }
}