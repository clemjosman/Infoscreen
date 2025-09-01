using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Infoscreens.Management.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{
    public class DeleteMultipleVideos : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly IVideoRepository _videoRepository;

        public DeleteMultipleVideos(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            IVideoRepository videoRepository
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _videoRepository = videoRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "DeleteMultipleVideos";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/video/{tenantCode}")] HttpRequestData req, string tenantCode)
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

                // Get list of ids
                string videoIdsQuery = req.Query["videoIds"];
                if (string.IsNullOrEmpty(videoIdsQuery))
                    throw new MissingOrBadQueryParameterCustomException();

                List<int> videoIds = videoIdsQuery.Split(",").Select(i => int.Parse(i)).ToList();
                if (videoIds.Count == 0 || videoIds.Any(i => i <= 0))
                    throw new MissingOrBadQueryParameterCustomException();


                // Delete video
                var videoList = await _databaseRepository.GetVideoFromTenantAsync(tenant, videoIds);

                if (videoList == null || videoList.Count == 0 || videoList.Count != videoIds.Count)
                    throw new VideoListNotFoundCustomException(videoIds.Count);

                
                await _videoRepository.DeleteVideoAsync(videoList);


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = "Returning null."
                });

                return HttpResponseHelper.EmptyResponse(req);
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
