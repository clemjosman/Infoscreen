using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class UpdateAllUptownArticlesCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<UpdateAllUptownArticlesCache> _logger;

        public UpdateAllUptownArticlesCache(ILogger<UpdateAllUptownArticlesCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        // Trigger: every hour from 05:00 to 18:00 UTC
        const string FUNCTION_NAME = "UpdateAllUptownArticlesCache";
        [Function(FUNCTION_NAME)]
        public async Task RunAsync([TimerTrigger("0 0 8-18 * * *", RunOnStartup = false)] TimerInfo timer)
        {
            try { 
                var api = eApi.UptownArticle;
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
