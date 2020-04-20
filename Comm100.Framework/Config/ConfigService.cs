using Comm100.Framework.Domain.Repository;
using System;

namespace Comm100.Framework.Config
{
    public class ConfigService : IConfigService
    {
        private IRepository<string, Config> _repository;
        public ConfigService(IRepository<string, Config> repository)
        {
            this._repository = repository;
        }
        public string Get(string key)
        {
            return _repository.Get(key).Result.Value;
        }
    }
}