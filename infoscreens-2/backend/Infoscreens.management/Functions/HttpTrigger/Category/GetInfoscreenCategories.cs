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
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{
    public class GetInfoscreenCategories : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public GetInfoscreenCategories(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        { }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetInfoscreenCategories";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/category/{tenantCode}/infoscreen/{infoscreenId}")] HttpRequestData req, string tenantCode, int infoscreenId)
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


                // Get the infoscreen
                var infoscreen = await _databaseRepository.GetInfoscreenByIdAsync(infoscreenId);

                // Check if infoscreen part of requested tenant
                if (infoscreen.InfoscreenGroup.TenantId != tenant.Id)
                {
                    throw new InfoscreenNotFoundCustomException(infoscreenId.ToString());
                }

                // Getting categories for various content type
                var newsCategories = await _databaseRepository.GetNewsCategoriesOfInfoscreenAsync(infoscreen);
                var videoCategories = await _databaseRepository.GetVideoCategoriesOfInfoscreenAsync(infoscreen);

                // Preparing API resposne model
                var apiInfoscreenCategories = new apiInfoscreenCategories(
                    newsCategories: newsCategories.Select(c => c.ToApiCategory()).ToList(),
                    videoCategories: videoCategories.Select(c => c.ToApiCategory()).ToList()
                );
               
                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiInfoscreenCategories != null ? JsonConvert.SerializeObject(apiInfoscreenCategories) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiInfoscreenCategories);
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
