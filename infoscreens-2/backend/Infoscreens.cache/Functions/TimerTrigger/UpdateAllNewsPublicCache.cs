using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class UpdateAllNewsPublicCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<UpdateAllNewsPublicCache> _logger;

        public UpdateAllNewsPublicCache(ILogger<UpdateAllNewsPublicCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        // Trigger: Every 30 minutes from 05:00 to 18:00 UTC
        const string FUNCTION_NAME = "UpdateAllNewsPublicCache";
        [Function(FUNCTION_NAME)]
        public async Task RunAsync([TimerTrigger("0 0,30 5-18 * * *", RunOnStartup = false)] TimerInfo timer)
        {
            try { 
                var api = eApi.NewsPublic;
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
