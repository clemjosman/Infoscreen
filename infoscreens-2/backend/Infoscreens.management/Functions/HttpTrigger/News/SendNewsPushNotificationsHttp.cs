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
    public class SendNewsPushNotificationsHttp : BaseApiClass
    {
        #region Constructor / Dependency Injection

        private readonly INewsRepository _newsRepository;

        public SendNewsPushNotificationsHttp(
            ILogger<BaseApiClass> logger,
            INewsRepository newsRepository,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _newsRepository = newsRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "SendNewsPushNotificationsHttp";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/notifications/{tenantCode}/send")] HttpRequestData req, string tenantCode)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                {
                    Custom1 = tenantCode
                });

                #region Authentication and tenant permission check

                User user;
                Tenant tenant;
                (user, tenant) = await BasicApiCallPermissionCheckAsync(req, tenantCode);

                #endregion Authentication and tenant permission check

                await _newsRepository.NotifyNewsForTenantAsync(tenant);

                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = tenantCode
                });

                return await HttpResponseHelper.JsonResponseAsync(req, new { success = true});
            }
            catch (Exception exception)
            {
                _exceptionHelper.HandleException(FUNCTION_NAME, exception);
                return await _exceptionHelper.ExceptionToResponseAsync(req, exception);
            }
        }
    }
}
