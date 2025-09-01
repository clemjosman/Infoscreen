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
using vesact.common.file.Interfaces;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{
    public class GetAllNews : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly IFileHelper _fileHelper;

        public GetAllNews(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            IFileHelper fileHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _fileHelper = fileHelper;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetAllNews";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/news/{tenantCode}")] HttpRequestData req, string tenantCode)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });


                #region Authentication and tenant permission check

                Tenant tenant;
                (_, tenant) = await BasicApiCallPermissionCheckAsync(req, tenantCode);

                #endregion Authentication and tenant permission check

                string searchText = req.Query["search"];
                var infoscreenIds = UrlHelper.ParseIdArray(req.Query["infoscreens"]);
                var categoryIds = UrlHelper.ParseIdArray(req.Query["categories"]);

                // Get news
                var news = await _databaseRepository.GetAllNewsFromTenantAsync(tenant, search: searchText, infoscreenIds: infoscreenIds, categoryIds: categoryIds);
                var apiNews = new List<apiNews>();

                if(news.Any())
                    apiNews = news.Select(async n => await n.ToApiNewsAsync(_databaseRepository, _fileHelper, CommonConfigHelper.AttachmentFileSasExpiry_CMS))
                                  .Select(t => t.Result)
                                  .ToList();


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
