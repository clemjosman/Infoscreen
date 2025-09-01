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
using vesact.common.message.v2;
using vesact.common.message.v2.Interfaces;
using vesact.common.message.v2.Models;

namespace Infoscreens.Management.Functions
{
    public class GetSubscriptions_App : BaseApiClass
    {
        #region Constructor / Dependency Injection

        readonly MessageService<IEmailProvider, FirebaseConfig_V1> _messageService;
        readonly ISubscriptionRepository _subscriptionRepository;

        public GetSubscriptions_App(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper,
            MessageService<IEmailProvider, FirebaseConfig_V1> messageService,
            ISubscriptionRepository subscriptionRepository
        ) : base(logger, databaseRepository, exceptionHelper)
        {
            _messageService = messageService;
            _subscriptionRepository = subscriptionRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetSubscriptions_App";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/app/subscribe/{tenantCode}")] HttpRequestData req, string tenantCode)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication check

                (User user, Tenant tenant) = await BasicApiCallPermissionCheckAsync(req, tenantCode, eApplication.MOBILE_APP);

                #endregion Authentication check

                var subscribedInfoscreens = await _subscriptionRepository.GetSubscribedInfoscreens(user, tenant);

                // Avoid endless serialization loop
                foreach (var i in subscribedInfoscreens)
                {
                    i.InfoscreenGroup = null;
                    i.Subscriptions = null;
                }

                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {});

                return await HttpResponseHelper.JsonResponseAsync(req, subscribedInfoscreens);
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
