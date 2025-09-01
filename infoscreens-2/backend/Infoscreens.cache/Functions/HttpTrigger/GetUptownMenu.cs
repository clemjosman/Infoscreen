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
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetUptownMenu
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetUptownMenu> _logger;

        public GetUptownMenu(ILogger<GetUptownMenu> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetUptownMenu";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/uptownmenu")] HttpRequestData req)
        {
            try
            {
                var blob = BlobRepository.GetBlockBlob(CommonConfigHelper.UptownMenuFile);
                var data = await BlobRepository.ReadBlockBlobContentAsync(blob);
                JObject response = JsonConvert.DeserializeObject<JObject>(data);
                return await HttpResponseHelper.JsonResponseAsync(req, response);
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
