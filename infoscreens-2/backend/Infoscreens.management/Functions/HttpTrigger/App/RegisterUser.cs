using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.Mobile;
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
    public class RegisterUser_App : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public RegisterUser_App(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        { }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "RegisterUser_App";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/app/user/register")] HttpRequestData req)
        {
            try
            {

                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                // No authentication check here, oid from token will be compared with the one in the request
                var oid = GetObjectIdFromTokenWithoutVerification(req);

                // Get body of request
                var register = await ExtractRequestBodyAsync<apiRegisterUser_Mobile>(req);

                // Verify oid with the one from token
                if (oid != register.ObjectId)
                    throw new MissingOrBadQueryParameterCustomException();

                // Register user
                var user = await _databaseRepository.CreateUser_Mobile(register);
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
