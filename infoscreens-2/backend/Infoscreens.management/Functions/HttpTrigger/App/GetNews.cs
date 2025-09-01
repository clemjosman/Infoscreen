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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using vesact.common.file.Interfaces;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{
    public class GetNews_App : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly IFileHelper _fileHelper;

        public GetNews_App(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            IFileHelper fileHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _fileHelper = fileHelper;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetNews_App";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/app/news")] HttpRequestData req)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication check

                User user = await BasicApiCallPermissionCheckAsync(req, eApplication.MOBILE_APP);

                #endregion Authentication check

                var subscriptions = await _databaseRepository.GetAllSubscriptionsForUserAsync(user.Id) ?? throw new UnauthorizedAccessException();

                DateTimeOffset? after = null;
                DateTimeOffset? before = null;
                var query = HttpUtility.ParseQueryString(req.Url.Query);
                if (query["after"] != null)
                {
                    string afterParam = req.Query["after"];
                    after = DateTimeOffset.Parse(afterParam);
                }
                if (query["before"] != null)
                {
                    string beforeParam = req.Query["before"];
                    before = DateTimeOffset.Parse(beforeParam);
                }

                List<News> listOfNews = [];
                foreach (var subscription in subscriptions)
                {
                    var newsArticles = await _databaseRepository.GetPublishedNewsForInfoscreenAsync(subscription.InfoscreenId, before, after, mustBeAssignedToApp: true);

                    foreach (var news in newsArticles)
                    {
                        // One article can be reused for multiple screens
                        // so we need to make sure each article is only added once
                        if (!listOfNews.Any(e => e.Id == news.Id))
                        {
                            listOfNews.Add(news);
                        }
                    }
                }
                
                var response = listOfNews.Select(async n => await n.ToApiNews_MobileAsync(_databaseRepository, _fileHelper, CommonConfigHelper.AttachmentFileSasExpiry_CMS))
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
