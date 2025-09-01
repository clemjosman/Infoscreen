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
using vesact.common.file.Interfaces;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{
    public class PostAndPutNews : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly INewsRepository _newsRepository;
        private readonly IFileHelper _fileHelper;

        public PostAndPutNews(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            INewsRepository newsRepository,
            IFileHelper fileHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _newsRepository = newsRepository;
            _fileHelper = fileHelper;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "PostAndPutNews";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = "v1/news/{tenantCode}")] HttpRequestData req, string tenantCode)
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
                var publishedNews = await ExtractRequestBodyAsync<apiNews_Publish>(req);

                // Create or update and persist news
                var news = await _newsRepository.CreateOrUpdateNewsAsync(publishedNews, tenant, user);
                news = await _databaseRepository.GetNewsFromTenantAsync(tenant, news.Id);
                var apiNews = await news.ToApiNewsAsync(_databaseRepository, _fileHelper, CommonConfigHelper.AttachmentFileSasExpiry_CMS);


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = apiNews != null ? JsonConvert.SerializeObject(apiNews) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, apiNews);
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
