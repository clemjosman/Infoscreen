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
    public class UpdateSelectedLanguage : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly IUserRepository _userRepository;

        public UpdateSelectedLanguage(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            IUserRepository userRepository
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _userRepository = userRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "UpdateSelectedLanguage";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/user/selectedLanguage")] HttpRequestData req)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication check

                User user = await BasicApiCallPermissionCheckAsync(req);

                #endregion Authentication check


                // Get the body of the request
                var newSelectedLanguage = await ExtractRequestBodyAsync<apiUser_UpdateSelectedLanguage>(req);

                // Update and persist data
                user = await _userRepository.UpdateSelectedLanguageAsync(user, newSelectedLanguage);
                var apiUser_Me = await user.ToApiUser_MeAsync(_databaseRepository);



                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiUser_Me != null ? JsonConvert.SerializeObject(apiUser_Me) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiUser_Me);
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
