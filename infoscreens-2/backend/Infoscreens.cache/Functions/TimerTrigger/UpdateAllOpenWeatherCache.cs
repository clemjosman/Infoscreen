using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class UpdateAllOpenWeatherCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<UpdateAllOpenWeatherCache> _logger;

        public UpdateAllOpenWeatherCache(ILogger<UpdateAllOpenWeatherCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        // Trigger: every hour from 05:00 to 18:00 UTC
        const string FUNCTION_NAME = "UpdateAllOpenWeatherCache";
        [Function(FUNCTION_NAME)]
        public async Task RunAsync([TimerTrigger("0 0 5-18 * * *", RunOnStartup = true)] TimerInfo timer)
        {
            try
            {
                var api = eApi.OpenWeather;
                var apiConfig = await BlobRepository.GetApiRequestsConfigAsync(api);
                CacheRepository.UpdateApiCache(api, apiConfig);
            }
            catch(Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
            }
        }
    }
}
