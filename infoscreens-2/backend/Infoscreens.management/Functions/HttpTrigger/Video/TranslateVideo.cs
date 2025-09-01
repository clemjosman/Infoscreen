using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Infoscreens.Management.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{
    public class TranslateVideo : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly IVideoRepository _videoRepository;

        public TranslateVideo(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            IVideoRepository videoRepository
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _videoRepository = videoRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "TranslateVideo";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/video/{tenantCode}/translate")] HttpRequestData req)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication and tenant permission check

                User user = await BasicApiCallPermissionCheckAsync(req);

                #endregion Authentication and tenant permission check


                // Get body of request
                var apiVideo_Translate = await ExtractRequestBodyAsync<apiVideo_Translate>(req);

                // Translate
                var apiVideo_Translated = await _videoRepository.TranslateVideoAsync(apiVideo_Translate);


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiVideo_Translated != null ? JsonConvert.SerializeObject(apiVideo_Translated) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiVideo_Translated);
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
