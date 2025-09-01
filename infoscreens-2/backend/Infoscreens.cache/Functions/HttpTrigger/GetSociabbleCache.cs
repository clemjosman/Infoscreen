using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Models.CachedData;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetSociabbleCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetSociabbleCache> _logger;

        public GetSociabbleCache(ILogger<GetSociabbleCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetSociabbleCache";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/sociabble")] HttpRequestData req)
        {
            try
            {
                var api = eApi.Sociabble;
                string nodeId = req.Query["nodeId"];
                var node = await BlobRepository.GetNodeConfigurationAsync(nodeId);

                var cachedFileNames = node.BackendConfig.GetCachedFileNames(api);

                var cachedSociabble = new List<SociabblePostCached>();
                foreach (var cachedFileName in cachedFileNames)
                {
                    var sociabbleCached = await BlobRepository.GetCachedDataAsync(api, cachedFileName);
                    cachedSociabble.AddRange(JsonConvert.DeserializeObject<List<SociabblePostCached>>(sociabbleCached));
                }

                return await HttpResponseHelper.JsonResponseAsync(req, cachedSociabble);
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
