using Newtonsoft.Json;
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
            return Convert.ToInt32(config.Get(key));
        }

        public static DateTime GetDateTime(this IConfigService config, string key)
        {
            return Convert.ToDateTime(config.Get(key));
        }

        public static T GetJson<T>(this IConfigService config, string key)
        {
            var value = config.Get(key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static byte[] GetFile(this IConfigService config, string key)
        {
            var value = config.Get(key);
            return System.IO.File.ReadAllBytes(value);
        }
    }
}