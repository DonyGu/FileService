using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comm100.Framework.Config
{
    public interface IConfigService
    {
        Task<string> Get(string key);
        IReadOnlyList<Config> List();
        Task Set(string key, string value);

        Task<string> TryGet(string key, string defaultValue);
    }

    public static class ConfigServiceExtension
    {
        public async static Task<bool> GetBool(this IConfigService config, string key)
        {
            return Convert.ToBoolean(await config.Get(key));
        }

        public async static Task<int> GetInt(this IConfigService config, string key)
        {
            return Convert.ToInt32(await config.Get(key));
        }

        public async static Task<DateTime> GetDateTime(this IConfigService config, string key)
        {
            return Convert.ToDateTime(await config.Get(key));
        }

        public async static Task<T> GetJson<T>(this IConfigService config, string key)
        {
            var value = await config.Get(key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public async static Task<byte[]> GetFile(this IConfigService config, string key)
        {
            var value = await config.Get(key);
            return System.IO.File.ReadAllBytes(value);
        }
    }
}