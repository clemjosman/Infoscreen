using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Models.CachedData;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetTwitterCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetTwitterCache> _logger;

        public GetTwitterCache(ILogger<GetTwitterCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetTwitterCache";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/twitter")] HttpRequestData req)
        {
            try
            {
                var api = eApi.Twitter;
                string nodeId = req.Query["nodeId"];
                var node = await BlobRepository.GetNodeConfigurationAsync(nodeId);

                var fileNames = node.BackendConfig.GetCachedFileNames(api);

                var tweets = new List<TweetCached>();
                foreach (var fileName in fileNames)
                {
                    var tweetsFromFile = await BlobRepository.GetCachedDataAsync(api, fileName);
                    tweets.AddRange(JsonConvert.DeserializeObject<List<TweetCached>>(tweetsFromFile));
                }

                return await HttpResponseHelper.JsonResponseAsync(req, tweets);
            }
            catch(FileNotFoundException ex)
            {
                return await HttpResponseHelper.TextResponseAsync(req, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
                throw;
            }
        }
    }
}
