using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.Infrastructure
{
    public static class IConfigurationConnectionStringExtension
    {
        public static string GetDatabaseConnectionString(this IConfiguration configuration, string name, string database)
        {
            var connectionString = configuration.GetConnectionString(name);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception($"Missing configuration for ConnectionString '{name}'");
            }
            var deploymentName = configuration["DeploymentName"];
            if (!string.IsNullOrEmpty(deploymentName) && !string.IsNullOrEmpty(database))
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                builder.InitialCatalog = $"{deploymentName}.{database}";
                return builder.ToString();
            }
            else
            {
                return connectionString;
            }
        }
    }
}
