using Comm100.Framework.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comm100.Framework.Config
{
    public class ConfigService : IConfigService
    {
        private IRepository<string, Config> _repository;
        public ConfigService(IRepository<string, Config> repository)
        {
            this._repository = repository;
        }
        public async Task<string> Get(string key)
        {
            var result= await _repository.Get(key);
            return result.Value;
        }

        public async Task Set(string key, string value)
        {
            var config= await _repository.Get(key);
            config.Value = value;
            await _repository.Update(config);
        }

        public IReadOnlyList<Config> List()
        {
            var result= _repository.ListAll();
            return result;
        }


        public async Task<string> TryGet(string key, string defaultValue)
        {
            var result = await _repository.Get(key);
            if (result == null)
            {
                result = await _repository.Create(new Config { Key = key, Value = defaultValue });
            }
            return result?.Value;
        }
    }
}