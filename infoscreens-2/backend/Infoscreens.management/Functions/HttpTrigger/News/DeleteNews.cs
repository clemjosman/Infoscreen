using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Infoscreens.Management.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{
    public class DeleteNews : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly INewsRepository _newsRepository;

        public DeleteNews(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            INewsRepository newsRepository
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _newsRepository = newsRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "DeleteNews";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/news/{tenantCode}/{newsId}")] HttpRequestData req, string tenantCode, int newsId)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication and tenant permission check

                Tenant tenant;
                (_, tenant) = await BasicApiCallPermissionCheckAsync(req, tenantCode);

                #endregion Authentication and tenant permission check


                // Delete news
                var news = await _databaseRepository.GetNewsFromTenantAsync(tenant, newsId)
                           ?? throw new NewsNotFoundCustomException(newsId, $"News with id {newsId} has not been found in tenant with code {tenantCode}.");
                
                await _newsRepository.DeleteNewsAsync(news);


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = "Returning nothing."
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
