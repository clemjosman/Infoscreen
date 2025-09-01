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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{

    public class GetInfoscreenGroups : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public GetInfoscreenGroups(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        { }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetInfoscreenGroups";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/infoscreens/{tenantCode}/groups")] HttpRequestData req, string tenantCode)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication and tenant permission check

                User user;
                Tenant tenant;
                (user, tenant) = await BasicApiCallPermissionCheckAsync(req, tenantCode);

                #endregion Authentication and tenant permission check


                // Get the infoscreens and format for api
                var apiInfoscreenGroups = new List<apiInfoscreenGroup>();
                var infoscreenGroupsFromTenant = (await _databaseRepository.GetInfoscreenGroupsFromTenantAsync(tenant));

                if(infoscreenGroupsFromTenant.Any())
                {
                    apiInfoscreenGroups = infoscreenGroupsFromTenant.Select(i => i.ToApiInfoscreenGroup())
                                                                    .ToList();
                }

                // Sort by group name
                apiInfoscreenGroups = apiInfoscreenGroups.OrderBy(ig => ig.Name.ToLowerInvariant()).ToList();


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiInfoscreenGroups != null ? JsonConvert.SerializeObject(apiInfoscreenGroups) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiInfoscreenGroups);
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
