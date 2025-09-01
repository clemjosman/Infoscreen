using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class UpdateAllTwentyMinCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<UpdateAllTwentyMinCache> _logger;

        public UpdateAllTwentyMinCache(ILogger<UpdateAllTwentyMinCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        // Trigger: Every 30 minutes from 03:00 to 18:00 UTC
        const string FUNCTION_NAME = "UpdateAllTwentyMinCache";
        [Function(FUNCTION_NAME)]
        public async Task RunAsync([TimerTrigger("0 0,30 3-18 * * *", RunOnStartup = false)] TimerInfo timer)
        {
            try { 
                var api = eApi.TwentyMin;
                var apiConfig = await BlobRepository.GetApiRequestsConfigAsync(api);
                CacheRepository.UpdateApiCache(api, apiConfig);
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
            }
        }
    }
}
