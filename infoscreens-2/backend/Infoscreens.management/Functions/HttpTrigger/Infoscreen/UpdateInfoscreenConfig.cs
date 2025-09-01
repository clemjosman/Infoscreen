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
    public class UpdateInfoscreenConfig : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly IInfoscreenRepository _infoscreenRepository;

        public UpdateInfoscreenConfig(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            IInfoscreenRepository infoscreenRepository
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _infoscreenRepository = infoscreenRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "UpdateInfoscreenConfig";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/infoscreen/{infoscreenId}/config")] HttpRequestData req, int infoscreenId)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication check

                User user = await BasicApiCallPermissionCheckAsync(req);

                #endregion Authentication check


                // Get infoscreen
                var infoscreen = await _databaseRepository.GetInfoscreenByIdAsync(infoscreenId)
                                 ?? throw new InfoscreenNotFoundCustomException(infoscreenId.ToString());

                // Check if user has access to requested infoscreen's tenant
                var tenant = await _databaseRepository.GetTenantByIdAsync(infoscreen.InfoscreenGroup.TenantId);
                if (!(await PermissionHelper.HasUserAccessToTenantAsync(_databaseRepository, user, tenant)))
                {
                    throw new InfoscreenNotFoundCustomException(infoscreenId.ToString());
                }


                // Get the body of the request
                var configUpdate = await ExtractRequestBodyAsync<apiInfoscreen_ConfigUpdate>(req);

                // Update and persist data
                var newConfig = await _infoscreenRepository.UpdateInfoscreenConfigAsync(infoscreen, configUpdate);



                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = newConfig != null ? JsonConvert.SerializeObject(newConfig) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, newConfig);
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
