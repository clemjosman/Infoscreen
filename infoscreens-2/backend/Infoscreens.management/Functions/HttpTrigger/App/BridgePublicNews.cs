using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Repositories;
using Infoscreens.Management.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{

    public class BridgePublicNews_App : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public BridgePublicNews_App(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        {}

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "BridgePublicNews_App";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/app/news/public")] HttpRequestData req)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                var response = await HttpRepository.GetAsync($"https://actemium.ch/wp-json/wp/v2/newspost{req.Url.Query}");

                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {});

                return await HttpResponseHelper.JsonResponseAsync(req, response);
            }
            catch (CustomExceptionBaseClass customException)
            {
                _exceptionHelper.HandleCustomException(FUNCTION_NAME, customException);
                return await customException.ToApiResponseAsync(req);
            }
            catch (Exception exception)
            {
                _exceptionHelper.HandleException(FUNCTION_NAME, exception);
                return await _exceptionHelper.ExceptionToResponseAsync(req, exception);
            }
        }
    }
}
