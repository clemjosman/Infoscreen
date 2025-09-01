using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class UpdateAllPublicTransportCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<UpdateAllPublicTransportCache> _logger;

        public UpdateAllPublicTransportCache(ILogger<UpdateAllPublicTransportCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        // Trigger: Every 15 minutes from 05:00 to 18:00 UTC
        const string FUNCTION_NAME = "UpdateAllPublicTransportCache";
        [Function(FUNCTION_NAME)]
        public async Task RunAsync([TimerTrigger("0 0/15 5-18 * * *", RunOnStartup = false)] TimerInfo timer)
        {
            try { 
                var api = eApi.PublicTransport;
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
