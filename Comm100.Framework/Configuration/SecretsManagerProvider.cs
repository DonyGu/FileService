using Comm100.Framework.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Comm100.Framework.Configuration
{
    public class SecretsManagerProvider : ConfigurationProvider
    {
        private readonly IConfiguration _configuration;


        public SecretsManagerProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string>();
            try
            {
                //LogHelper.Info($"_configuration[{SecretsManagerConstants.CONNECTION_CONFIGURATION_SECRETNAME_KEY}]: {_configuration[SecretsManagerConstants.CONNECTION_CONFIGURATION_SECRETNAME_KEY]}");
                var configurationValue = _configuration[SecretsManagerConstants.CONNECTION_CONFIGURATION_SECRETNAME_KEY];
                if (configurationValue != null && configurationValue.StartsWith("@aws-sm:"))
                {
                    var configurationConnectionStr = SecretsManager.GetSecret(configurationValue.Replace("@aws-sm:", ""));
                    //LogHelper.Info(configurationConnectionStr);
                    Data.Add(SecretsManagerConstants.CONNECTION_CONFIGURATION_SECRETNAME_KEY, configurationConnectionStr);
                }
            }
            catch (Exception e)
            {
                LogHelper.Info(e.Message);
            }

        }
    }

    public class SecretsManagerConstants
    {
        public const string CONNECTION_CONFIGURATION_SECRETNAME_KEY = "ConnectionStrings:Configuration";
        public const string CONNECTION_REPORTING_SECRETNAME_KEY = "ConnectionStrings:Reporting";
    }
}