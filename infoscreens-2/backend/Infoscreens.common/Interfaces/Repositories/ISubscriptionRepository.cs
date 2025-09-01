using Infoscreens.Common.Models.API.Mobile;
using Infoscreens.Common.Models.EntityFramework.CMS;
using System.Collections.Generic;
using System.Threading.Tasks;
using vesact.common.message.v2.Models;

namespace Infoscreens.Common.Interfaces
{
    public interface ISubscriptionRepository
    {
        #region Get

        Task<List<Infoscreen>> GetSubscribedInfoscreens(User user, Tenant tenant);
        

        #endregion Get

        #region Create/Update

        Task<List<Subscription>> CreateOrUpdateSubscriptionsAsync(User user, PushToken pushToken, apiSubscribeDevice_Mobile subscribe);

        #endregion Create/Update
    }
}
