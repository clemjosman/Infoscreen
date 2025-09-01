using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class UpdateAllJobOffersCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<UpdateAllJobOffersCache> _logger;

        public UpdateAllJobOffersCache(ILogger<UpdateAllJobOffersCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        // Trigger: every hour from 05:00 to 18:00 UTC
        const string FUNCTION_NAME = "UpdateAllJobOffersCache";
        const string FUNCTION_NAME_TIMER = FUNCTION_NAME+"Timer";
        [Function(FUNCTION_NAME_TIMER)]
        public async Task RunTimerAsync([TimerTrigger("0 0 8-18 * * *", RunOnStartup = false)] TimerInfo timer)
        {
            try { 
                var api = eApi.JobOffers;
                var apiConfig = await BlobRepository.GetApiRequestsConfigAsync(api);
                CacheRepository.UpdateApiCache(api, apiConfig);
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME_TIMER + "() has thrown an exception: {0}", exception.Message));
            }
        }

        const string FUNCTION_NAME_HTTP = FUNCTION_NAME + "Http";
        [Function("UpdateAllJobOffersCacheHttp")]
        public async Task RunHttpAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/jobOffersUpdate")] HttpRequestData req)
        {
            try
            {
                var api = eApi.JobOffers;
                var apiConfig = await BlobRepository.GetApiRequestsConfigAsync(api);
                CacheRepository.UpdateApiCache(api, apiConfig);
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME_HTTP + "() has thrown an exception: {0}", exception.Message));
            }
        }
    }
}
