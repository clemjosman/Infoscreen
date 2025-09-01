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
    public class GetCategories : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public GetCategories(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        { }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetCategories";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/category/{tenantCode}")] HttpRequestData req, string tenantCode)
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

                var categories = await _databaseRepository.GetCategoriesFromTenantAsync(tenant);
                var apiCategories = new List<apiCategory>();

                if(categories.Count > 0)
                    apiCategories = categories.Select(c => c.ToApiCategory()).ToList();


               
                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiCategories != null ? JsonConvert.SerializeObject(apiCategories) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiCategories);
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
