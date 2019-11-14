using System;
using System.Threading;
using FileService.Application.Interfaces;
using Comm100.Framework.Security;
using Comm100.Framework.Config;
using Comm100.Framework.Exceptions;

namespace FileService.Application.Services
{
    public class FileRateLimitingService : IFileRateLimitingService
    {
        IConfigService _configService;

        RequestsCounter _createRate;
        RequestsCounter _getPerFileKeyRate;
        RequestsCounter _getPerIpRate;

        Timer _clearTimer;

        public FileRateLimitingService(IConfigService configService)
        {
            this._configService = configService;
            var rateLimitingConfig = configService.GetJson<RateLimitingConfig>("RateLimiting");
            this._createRate = new RequestsCounter(rateLimitingConfig.SavePerIP.DurationTime);
            this._getPerIpRate = new RequestsCounter(rateLimitingConfig.GetPerIP.DurationTime);
            this._getPerFileKeyRate = new RequestsCounter(rateLimitingConfig.GetPerFileKey.DurationTime);

            _clearTimer = new Timer((_) => OnClearTimerTick(), null, 10 * 1000, 10 * 1000);
        }

        public void CheckUpload(string ip)
        {
            var rateLimitingConfig = this._configService.GetJson<RateLimitingConfig>("RateLimiting");

            if (this._createRate.Increase(ip) > rateLimitingConfig.SavePerIP.Limit)
            {
                throw new RateLimitingException();
            }
        }

        public void CheckGet(string ip, string fileKey)
        {
            var rateLimitingConfig = this._configService.GetJson<RateLimitingConfig>("RateLimiting");

            if (this._getPerIpRate.Increase(ip) > rateLimitingConfig.GetPerIP.Limit)
            {
                throw new RateLimitingException();
            }

            if (this._getPerFileKeyRate.Increase(fileKey) > rateLimitingConfig.GetPerFileKey.Limit)
            {
                throw new RateLimitingException();
            }
        }

        private void OnClearTimerTick()
        {
            this._createRate.Clear();
            this._getPerFileKeyRate.Clear();
            this._getPerIpRate.Clear();
        }

        class RateLimitingConfig
        {
            public RateLimitingConfigItem GetPerIP { get; set; }
            public RateLimitingConfigItem GetPerFileKey { get; set; }
            public RateLimitingConfigItem SavePerIP { get; set; }
        }

        class RateLimitingConfigItem
        {
            public string Duration { get; set; }
            public int Limit { get; set; }
            public TimeSpan DurationTime { get { return new TimeSpan(0,Convert.ToInt32(Duration.Split(":")[0]),Convert.ToInt16(Duration.Split(":")[1]),Convert.ToInt32(Duration.Split(":")[2])); } }
        }
    }
}