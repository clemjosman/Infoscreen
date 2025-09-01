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
    public class GetAllVideos : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public GetAllVideos(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        { }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetAllVideos";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/video/{tenantCode}")] HttpRequestData req, string tenantCode)
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

                string searchText = req.Query["search"];
                var infoscreenIds = UrlHelper.ParseIdArray(req.Query["infoscreens"]);
                var categoryIds = UrlHelper.ParseIdArray(req.Query["categories"]);

                // Get videos
                var videos = await _databaseRepository.GetAllVideosFromTenantAsync(tenant, search: searchText, infoscreenIds: infoscreenIds, categoryIds: categoryIds);
                var apiVideos = new List<apiVideo>();

                if (videos.Any())
                    apiVideos = videos.Select(async n => await n.ToApiVideoAsync(_databaseRepository)).Select(t => t.Result).ToList();



                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiVideos != null ? JsonConvert.SerializeObject(apiVideos) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiVideos);
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
