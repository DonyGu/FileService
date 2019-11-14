using System;

namespace Comm100.Framework.Config
{
    public interface IConfigService
    {
        string Get(string key);
    }

    public static class ConfigServiceExtension
    {
        public static bool GetBool(this IConfigService config, string key)
        {
            return Convert.ToBoolean(config.Get(key));
        }

        public static int GetInt(this IConfigService config, string key)
        {
            throw new NotImplementedException();
        }

        public static DateTime GetDateTime(this IConfigService config, string key)
        {
            throw new NotImplementedException();
        }

        public static T GetJson<T>(this IConfigService config, string key)
        {
            throw new NotImplementedException();
        }
    }
}