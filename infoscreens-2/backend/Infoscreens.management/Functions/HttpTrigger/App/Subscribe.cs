using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.Mobile;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Infoscreens.Management.Enumerations;
using Infoscreens.Management.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;
using vesact.common.message.v2;
using vesact.common.message.v2.Interfaces;
using vesact.common.message.v2.Models;

namespace Infoscreens.Management.Functions
{
    public class Subscribe_App : BaseApiClass
    {
        #region Constructor / Dependency Injection

        readonly MessageService<IEmailProvider, FirebaseConfig_V1> _messageService;
        readonly ISubscriptionRepository _subscriptionRepository;

        public Subscribe_App(
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

        const string FUNCTION_NAME = "Subscribe_App";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/app/subscribe/{tenantCode}")] HttpRequestData req)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication check

                User user = await BasicApiCallPermissionCheckAsync(req, eApplication.MOBILE_APP);

                #endregion Authentication check

                // Get body of request
                var subscribe = await ExtractRequestBodyAsync<apiSubscribeDevice_Mobile>(req);

                // Store pushToken if token is available, else update it (for web usage)
                PushToken pushToken;
                if (!string.IsNullOrWhiteSpace(subscribe.Token))
                    pushToken = await _messageService.RegisterDeviceAsync(user.Id.ToString(), subscribe);
                else
                    pushToken = await _messageService.GetTokenByUserIdAsync(user.Id.ToString());

                // Create/Update subscriptions
                var subscription = await _subscriptionRepository.CreateOrUpdateSubscriptionsAsync(user, pushToken, subscribe);

                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {});

                return await HttpResponseHelper.TextResponseAsync(req, "");
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
