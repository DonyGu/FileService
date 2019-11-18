using Comm100.Framework.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.Infrastructure.EntityConfigurations
{
    class ConfigEntityTypeConfigurations : IEntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeBuilder<Config> builder)
        {
            builder.ToTable("t_fileService_config");
            builder.HasKey(a => a.Key);
        }
    }
}
