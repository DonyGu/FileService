using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comm100.Framework.Configuration
{
    public static class ConfigurationBuilderExtension
    {
        public static IConfigurationBuilder AddSecretsManagerConfiguration(
            this IConfigurationBuilder builder
            )
        {
            var configuration = builder.Build();
            return builder.Add(new SecretsManagerConfigurationSource(configuration));
        }
    }
}
