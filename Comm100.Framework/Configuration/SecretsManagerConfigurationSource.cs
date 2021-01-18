using Microsoft.Extensions.Configuration;

namespace Comm100.Framework.Configuration
{
    public class SecretsManagerConfigurationSource : IConfigurationSource
    {
        private readonly IConfiguration _configuration;
        public SecretsManagerConfigurationSource(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SecretsManagerProvider(_configuration);
        }
    }
}