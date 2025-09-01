using Infoscreens.Common.Helpers;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetMSBLogLevel
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetMSBLogLevel> _logger;

        public GetMSBLogLevel(ILogger<GetMSBLogLevel> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetMSBLogLevel";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/MSBLogLevel")] HttpRequestData req)
        {
            try
            {
                string nodeId = req.Query["nodeId"];
                var node = await BlobRepository.GetNodeConfigurationAsync(nodeId);

                var result = node.FrontendConfig.MsbLogLevel.ToString();
                return await HttpResponseHelper.TextResponseAsync(req, result);
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
