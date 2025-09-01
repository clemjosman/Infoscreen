using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.Mobile;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;
using vesact.common.message.v2.Models;

namespace Infoscreens.Common.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ILogger<SubscriptionRepository> _logger;
        private readonly IDatabaseRepository _databaseRepository;

        public SubscriptionRepository(ILogger<SubscriptionRepository> logger, IDatabaseRepository databaseRepository)
        {
            logger.LogDebug(new LogItem(1, "SubscriptionRepository() Creating a new instance."));

            _logger = logger;
            _databaseRepository = databaseRepository;

            _logger.LogTrace(new LogItem(2, "SubscriptionRepository() New instance has been created."));
        }

        #region Get

        public async Task<List<Infoscreen>> GetSubscribedInfoscreens(User user, Tenant tenant)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "GetSubscribedInfoscreens() called.")
                {
                    Custom1 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom2 = tenant != null ? tenant.ToString() : "Parameter tenant is null."
                });

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (tenant == null)
                    throw new ArgumentNullException(nameof(tenant));
                
                var existingSubscriptions = await _databaseRepository.GetAllSubscriptionsForUserAsync(user.Id);
                var subscribedInfoscreens = existingSubscriptions.Where(s => s.Infoscreen.InfoscreenGroup.TenantId == tenant.Id).Select(s => s.Infoscreen).ToList();
                
                _logger.LogDebug(new LogItem(11, "GetSubscribedInfoscreens() finished.")
                {
                    Custom1 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom2 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom3 = $"Returning {subscribedInfoscreens.Count} subscriptions."
                });

                return subscribedInfoscreens;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetSubscribedInfoscreens() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Get

        #region Create/Update

        public async Task<List<Subscription>> CreateOrUpdateSubscriptionsAsync(User user, PushToken pushToken, apiSubscribeDevice_Mobile subscribe)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "CreateOrUpdateSubscriptionsAsync() called.")
                {
                    Custom1 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom2 = pushToken != null ? pushToken.ToString() : "Parameter pushToken is null.",
                    Custom4 = subscribe != null ? JsonConvert.SerializeObject(subscribe) : "Parameter subscribe is null."
                });

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (subscribe == null)
                    throw new ArgumentNullException(nameof(subscribe));

                // Delete and then create subscriptions
                // First remove all existing subscriptions
                var existingSubscriptions = await _databaseRepository.GetAllSubscriptionsForUserAsync(user.Id);
                foreach (var subscription in existingSubscriptions)
                {
                    await _databaseRepository.DeleteSubscriptionAsync(subscription);
                }

                // Then create all user selected subscriptions
                List<Subscription> result = [];
                foreach (var requestedInfoscreenId in subscribe.InfoscreenIds)
                {
                    // Create new subscription
                    var newSub = new Subscription(user, pushToken, requestedInfoscreenId);
                    // Add it to the result list
                    result.Add(await _databaseRepository.CreateOrUpdateSubscriptionAsync(newSub));
                }

                _logger.LogDebug(new LogItem(11, "CreateOrUpdateSubscriptionsAsync() finished.")
                {
                    Custom1 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom2 = pushToken != null ? pushToken.ToString() : "Parameter pushToken is null.",
                    Custom3 = subscribe != null && subscribe.InfoscreenIds.Count != 0 ? $"[{string.Join(", ", subscribe.InfoscreenIds)}]" : "Subscription is null.",
                    Custom4 = subscribe != null ? JsonConvert.SerializeObject(subscribe) : "Parameter subscribe is null."
                });

                // Return modified infoscreen
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "CreateOrUpdateSubscriptionsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Create/Update

    }
}
