using Comm100.Framework.Config;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Text;

namespace FileService.Infrastructure.EntityConfigurations
{
    class ConfigEntityTypeConfigurations : EntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeConfiguration<Config> builder)
        {
            builder.ToTable("t_fileService_config");
            builder.HasKey(a => a.Key);
        }
    }
}
