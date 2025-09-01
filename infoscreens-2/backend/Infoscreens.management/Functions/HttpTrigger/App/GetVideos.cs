using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Infoscreens.Management.Enumerations;
using Infoscreens.Management.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{
    public class GetVideos_App : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public GetVideos_App(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        {}

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetVideos_App";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/app/videos")] HttpRequestData req)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication check

                User user = await BasicApiCallPermissionCheckAsync(req, eApplication.MOBILE_APP);

                #endregion Authentication check

                var subscriptions = await _databaseRepository.GetAllSubscriptionsForUserAsync(user.Id)
                                   ?? throw new UnauthorizedAccessException();

                var infoscreenIds = subscriptions.Select(s => s.InfoscreenId).ToList();
                var videos = await _databaseRepository.GetPublishedVideosForInfoscreensAsync(infoscreenIds, mustBeAssignedToApp: true);
                var response = videos.Select(async v => await v.ToApiVideo_MobileAsync(_databaseRepository))
                                   .Select(t => t.Result)
                                   .ToList();

                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {});

                return await HttpResponseHelper.JsonResponseAsync(req, response);
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
