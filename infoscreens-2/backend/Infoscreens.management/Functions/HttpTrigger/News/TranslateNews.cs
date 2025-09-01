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
    public class TranslateText : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly INewsRepository _newsRepository;

        public TranslateText(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            INewsRepository newsRepository
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _newsRepository = newsRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "TranslateText";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/news/{tenantCode}/translate")] HttpRequestData req)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication check

                User user = await BasicApiCallPermissionCheckAsync(req);

                #endregion Authentication check


                // Get body of request
                var apiNews_translate = await ExtractRequestBodyAsync<apiNews_Translate>(req);

                // Translate
                var apiNews_Translated = await _newsRepository.TranslateNewsAsync(apiNews_translate);


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiNews_Translated != null ? JsonConvert.SerializeObject(apiNews_Translated) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiNews_Translated);
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
