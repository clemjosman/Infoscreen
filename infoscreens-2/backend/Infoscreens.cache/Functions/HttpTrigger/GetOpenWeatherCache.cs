using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetOpenWeatherCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetOpenWeatherCache> _logger;

        public GetOpenWeatherCache(ILogger<GetOpenWeatherCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetOpenWeatherCache";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/openweather")] HttpRequestData req)
        {
            try
            {
                var api = eApi.OpenWeather;
                string nodeId = req.Query["nodeId"];
                var node = await BlobRepository.GetNodeConfigurationAsync(nodeId);

                var data = await BlobRepository.GetCachedDataAsync(api, node.BackendConfig.GetCachedFileName(api));
                JObject response= JsonConvert.DeserializeObject<JObject>(data);

                return await HttpResponseHelper.JsonResponseAsync(req, response);
            }
            catch(FileNotFoundException ex)
            {
                return await HttpResponseHelper.TextResponseAsync(req, ex.Message, HttpStatusCode.BadRequest);
            }
            catch(Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
                throw;
            }
        }
    }
}
