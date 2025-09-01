using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Infoscreens.Management.Enumerations;
using Infoscreens.Management.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{

    public class GetInfoscreens_App : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public GetInfoscreens_App(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        { }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetInfoscreens_App";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/app/infoscreens/{tenantCode}")] HttpRequestData req, string tenantCode)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication and tenant permission check

                User user;
                Tenant tenant;
                (user, tenant) = await BasicApiCallPermissionCheckAsync(req, tenantCode, eApplication.MOBILE_APP);

                #endregion Authentication and tenant permission check


                // Get the infoscreens and format for api
                var apiInfoscreens_Light = new List<apiInfoscreen_Light>();
                var infoscreensFromTenant = (await _databaseRepository.GetInfoscreensFromTenantAsync(tenant));

                if(infoscreensFromTenant.Any())
                {
                    apiInfoscreens_Light = infoscreensFromTenant.Select(i => i.ToApiInfoscreen_Light(_databaseRepository))
                                                                .OrderBy(i => i.DisplayName.ToLowerInvariant())
                                                                .ToList();
                }


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiInfoscreens_Light != null ? JsonConvert.SerializeObject(apiInfoscreens_Light) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiInfoscreens_Light);
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
