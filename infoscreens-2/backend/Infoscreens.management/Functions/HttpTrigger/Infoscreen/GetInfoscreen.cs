using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
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

    public class GetInfoscreen : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public GetInfoscreen(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        { }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetInfoscreen";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/infoscreen/{infoscreenId}")] HttpRequestData req, int infoscreenId)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication and tenant permission check

                User user = await BasicApiCallPermissionCheckAsync(req);

                #endregion Authentication and tenant permission check


                // Get the infoscreen
                var infoscreen = await _databaseRepository.GetInfoscreenByIdAsync(infoscreenId);

                // Check if user has access to requested infoscreen's tenant
                var tenant = await _databaseRepository.GetTenantByIdAsync(infoscreen.InfoscreenGroup.TenantId);
                if (!(await PermissionHelper.HasUserAccessToTenantAsync(_databaseRepository, user, tenant)))
                {
                    throw new InfoscreenNotFoundCustomException(infoscreenId.ToString());
                }


                var apiInfoscreen_Light = infoscreen.ToApiInfoscreen_Light(_databaseRepository);


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiInfoscreen_Light != null ? JsonConvert.SerializeObject(apiInfoscreen_Light) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiInfoscreen_Light);
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
