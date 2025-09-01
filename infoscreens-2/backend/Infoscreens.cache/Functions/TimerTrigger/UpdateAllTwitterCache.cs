using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class UpdateAllTwitterCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<UpdateAllTwitterCache> _logger;

        public UpdateAllTwitterCache(ILogger<UpdateAllTwitterCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        // Trigger: Every 15 minutes
        // Disabled due to #8228
        //const string FUNCTION_NAME = "UpdateAllTwitterCache";
        //[Function(FUNCTION_NAME)]
        //public async Task RunAsync([TimerTrigger("0 0,15,30,45 * * * *", RunOnStartup = false)] TimerInfo timer)
        //{
        //    try { 
        //        var api = eApi.Twitter;
        //        var apiConfig = await BlobRepository.GetApiRequestsConfigAsync(api);
        //        CacheRepository.UpdateApiCache(api, apiConfig);
        //    }
        //    catch (Exception exception)
        //    {
        //        _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
        //    }
        //}
    }
}
