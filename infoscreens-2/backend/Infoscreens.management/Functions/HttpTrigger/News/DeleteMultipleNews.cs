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
    public class DeleteMultipleNews : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly INewsRepository _newsRepository;

        public DeleteMultipleNews(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            INewsRepository newsRepository
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _newsRepository = newsRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "DeleteMultipleNews";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/news/{tenantCode}")] HttpRequestData req, string tenantCode)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication and tenant permission check

                Tenant tenant;
                (_, tenant) = await BasicApiCallPermissionCheckAsync(req, tenantCode);

                #endregion Authentication and tenant permission check

                // Get list of ids
                string newsIdsQuery = req.Query["newsIds"];
                if (string.IsNullOrEmpty(newsIdsQuery))
                    throw new MissingOrBadQueryParameterCustomException();

                List<int> newsIds = newsIdsQuery.Split(",").Select(i => int.Parse(i)).ToList();
                if (newsIds.Count == 0 || newsIds.Any(i => i <= 0))
                    throw new MissingOrBadQueryParameterCustomException();


                // Delete news
                var newsList = await _databaseRepository.GetNewsFromTenantAsync(tenant, newsIds);

                if (newsList == null || newsList.Count == 0 || newsList.Count != newsIds.Count)
                    throw new NewsListNotFoundCustomException(newsIds.Count);

                await _newsRepository.DeleteNewsAsync(newsList);


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
