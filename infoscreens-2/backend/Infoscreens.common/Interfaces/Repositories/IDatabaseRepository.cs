using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.API.Mobile;
using Infoscreens.Common.Models.EntityFramework.CMS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoscreens.Common.Interfaces
{
    public partial interface IDatabaseRepository
    {
        #region Category

        Task<List<Category>> GetCategoriesFromTenantAsync(Tenant tenant);

        Task<Category> GetCategoryFromTenantAsync(Tenant tenant, string category);

        Task<List<Category>> GetUnassociatedCategoriesAsync();

        Task DeleteCategoriesAsync(List<Category> category);

        #endregion Category

        #region Infoscreen

        Task<IEnumerable<Infoscreen>> GetInfoscreensAsync();

        Task<IEnumerable<Infoscreen>> GetInfoscreensFromTenantAsync(Tenant tenant);

        Task<Infoscreen> GetInfoscreenByNodeIdAsync(string nodeId);

        Task<Infoscreen> GetInfoscreenByIdAsync(int id);

        Task<List<Infoscreen>> GetInfoscreensByIdsAsync(IEnumerable<int> infoscreensIds);

        Task<List<Infoscreen>> GetInfoscreensByNewsIdAsync(int newsId);

        Task UpdateInfoscreenAsync(Infoscreen infoscreen);

        #endregion Infoscreen

        #region Infoscreen Groups

        Task<IEnumerable<InfoscreenGroup>> GetInfoscreenGroupsFromTenantAsync(Tenant tenant);

        #endregion Infoscreen Groups

        #region InfoscreensNews

        Task<List<InfoscreenNews>> GetInfoscreensNewsForInfoscreenAsync(int infoscreenId);
        Task<List<InfoscreenNews>> GetInfoscreensNewsForNewsAsync(int newsId);
        Task<List<InfoscreenNews>> GetInfoscreensNewsForNewsAsync(List<int> newsIds);

        #endregion InfoscreensNews

        #region InfoscreensVideos

        Task<List<InfoscreenVideo>> GetInfoscreensVideosForVideoAsync(int videoId);
        Task<List<InfoscreenVideo>> GetInfoscreensVideosForVideoAsync(List<int> videoIds);

        #endregion InfoscreensVideos

        #region Language

        Task<IEnumerable<Language>> GetAllLanguageAsync();

        Task<Language> GetLanguageAsync(apiLanguage_Light language);

        Task<Language> GetLanguageAsync(int languageId);

        Task<Language> GetLanguageAsync(string iso2);

        #endregion Language

        #region News

        Task<IEnumerable<News>> GetAllNewsAsync();
        Task<List<News>> GetNewsAsync(List<int> newsIds);

        Task<IEnumerable<News>> GetAllNewsFromTenantAsync(Tenant tenant, string search = "", IEnumerable<int> infoscreenIds = null, IEnumerable<int> categoryIds = null);
        Task<IEnumerable<News>> GetNewsForNotificationAsync(Tenant tenant, DateTimeOffset publishedSinceAtLeast);

        Task<News> GetNewsFromTenantAsync(Tenant tenant, int newsId);
        Task<List<News>> GetNewsFromTenantAsync(Tenant tenant, List<int> newsIds);

        Task<List<News>> GetPublishedNewsForInfoscreenAsync(int infoscreenId, DateTimeOffset? before = null, DateTimeOffset? after = null, int amount = 0, bool mustBeAssignedToInfoscreens = false, bool mustBeAssignedToApp = false);

        Task<News> CreateOrUpdateNewsAsync(News news, IEnumerable<TranslatedText> translatedTextsToDelete, IEnumerable<InfoscreenNews> infoscreensNewsToCreate, IEnumerable<InfoscreenNews> infoscreensNewsToDelete, IEnumerable<NewsCategory> newsCategoriesToDelete);

        Task<IEnumerable<News>> UpdateNewsAsync(IEnumerable<News> news);

        Task DeleteNewsAsync(News news);
        Task DeleteNewsAsync(List<News> news);

        #endregion News

        #region NewsCategories

        Task<List<NewsCategory>> GetNewsCategoriesOfNewsAsync(News news);

        Task<NewsCategory> GetNewsCategoryAsync(string category, News news);

        Task DeleteNewsCategoriesAsync(List<NewsCategory> newsCategories);

        Task<List<Category>> GetNewsCategoriesOfInfoscreenAsync(Infoscreen infoscreen);

        #endregion

        #region Subscription

        Task<List<Subscription>> GetAllSubscriptionsForUserAsync(int userId);
        Task<Subscription> CreateOrUpdateSubscriptionAsync(Subscription subscription);
        Task DeleteSubscriptionAsync(Subscription subscription);

        #endregion Subscription

        #region Tenant

        Task<IEnumerable<Tenant>> GetAllTenantsAsync();

        Task<Tenant> GetTenantByCodeAsync(string tenantCode);

        Task<Tenant> GetTenantByIdAsync(int tenantId);

        #endregion Tenant

        #region TranslatedText

       Task<TranslatedText> GetTranslatedTextAsync(Translation translation, Language language);

        Task<List<TranslatedText>> GetTranslatedTextsOfTranslationAsync(Translation translation);

        Task DeleteTranslatedTextsAsync(IEnumerable<TranslatedText> translatedTexts);

        #endregion TranslatedText

        #region Translation

        Task<Translation> GetTranslationtByIdAsync(int translationId);

        #endregion Translation

        #region User

        Task<User> GetUserByIdAsync(int userId);

        Task<User> GetUserByObjectIdAsync(string objectId);

        Task<User> GetUserByEmailAsync(string email, bool isUsernameMatchEnough = false);

        Task UpdateUserAsync(User user);

        Task<User> CreateUser_Mobile(apiRegisterUser_Mobile register);

        #endregion User

        #region UserTenant

        Task<UserTenant> GetUserTenantAsync(int userId, int tenantId);

        Task<List<UserTenant>> GetUserTenantsFromUserAsync(int userId);

        #endregion UserTenant

        #region Video

        Task<IEnumerable<Video>> GetAllVideosFromTenantAsync(Tenant tenant, string search = "", IEnumerable<int> infoscreenIds = null, IEnumerable<int> categoryIds = null);

        Task<Video> GetVideoFromTenantAsync(Tenant tenant, int videoId);
        Task<List<Video>> GetVideoFromTenantAsync(Tenant tenant, List<int> videoIds);

        Task<List<Video>> GetPublishedVideosForInfoscreenAsync(int infoscreenId, int amount = 0, bool mustBeAssignedToInfoscreens = false, bool mustBeAssignedToApp = false);

        Task<List<Video>> GetPublishedVideosForInfoscreensAsync(List<int> infoscreenIds, int amount = 0, bool mustBeAssignedToInfoscreens = false, bool mustBeAssignedToApp = false);

        Task<Video> CreateOrUpdateVideoAsync(Video video, IEnumerable<TranslatedText> translatedTextsToDelete, IEnumerable<InfoscreenVideo> infoscreensVideosToCreate, IEnumerable<InfoscreenVideo> infoscreensVideosToDelete, IEnumerable<VideoCategory> videoCategoriesToDelete);

        Task DeleteVideoAsync(Video video);
        Task DeleteVideoAsync(List<Video> videos);

        #endregion Video

        #region VideoCategories

        Task<List<VideoCategory>> GetVideoCategoriesOfVideoAsync(Video video);

        Task<VideoCategory> GetVideoCategoryAsync(string category, Video video);

        Task DeleteVideoCategoriesAsync(List<VideoCategory> videoCategories);

        Task<List<Category>> GetVideoCategoriesOfInfoscreenAsync(Infoscreen infoscreen);

        #endregion
    }
}
