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
    public class PostAndPutVideo : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly IVideoRepository _videoRepository;

        public PostAndPutVideo(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            IVideoRepository videoRepository
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _videoRepository = videoRepository;
        }

        #endregion Constructor / Dependency Injection


        const string FUNCTION_NAME = "PostAndPutVideo";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = "v1/video/{tenantCode}")] HttpRequestData req, string tenantCode)
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


                // Get body of request
                var publishedVideo = await ExtractRequestBodyAsync<apiVideo_Publish>(req);

                // Create or update and persist video
                var video = await _videoRepository.CreateOrUpdateVideoAsync(publishedVideo, tenant, user);
                video = await _databaseRepository.GetVideoFromTenantAsync(tenant, video.Id);
                var apiVideo = await video.ToApiVideoAsync(_databaseRepository);


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiVideo != null ? JsonConvert.SerializeObject(apiVideo) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiVideo);
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
