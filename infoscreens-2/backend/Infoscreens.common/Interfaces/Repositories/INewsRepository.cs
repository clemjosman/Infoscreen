using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.EntityFramework.CMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoscreens.Common.Interfaces
{
    public interface INewsRepository
    {
        #region Create or Update

        Task<News> CreateOrUpdateNewsAsync(apiNews_Publish publishedNews, Tenant tenant, User user);

        #endregion Create or Update

        #region Get

        Task NotifyNewsForTenantAsync(Tenant tenant);

        #endregion

        #region Delete

        Task DeleteNewsAsync(News news);
        Task DeleteNewsAsync(List<News> news);

        #endregion Delete

        #region Notification

        Task<News> MarkNewsAsNotifiedAsync(News news);

        Task<News> ResetNewsNotificationAsync(News news, User user);

        #endregion Notification

        #region Translate

        Task<apiNews_Translated> TranslateNewsAsync(apiNews_Translate apiNews_Translate);

        #endregion Translate
    }
}
