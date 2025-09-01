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
    public class GetInfoscreenNodesStateCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetInfoscreenNodesStateCache> _logger;

        public GetInfoscreenNodesStateCache(ILogger<GetInfoscreenNodesStateCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetInfoscreenNodesStateCache";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/infoscreensState")] HttpRequestData req)
        {
            try
            {
                var api = eApi.Microservicebus;
                string nodeId = req.Query["nodeId"];
                var node = await BlobRepository.GetNodeConfigurationAsync(nodeId);

                var response = await BlobRepository.GetCachedDataAsync(api, node.BackendConfig.GetCachedFileName(api));

                var statuses = JsonConvert.DeserializeObject<IEnumerable<InfoscreenNodeStatusCached>>(response);
                
                return await HttpResponseHelper.JsonResponseAsync(req, statuses);
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
