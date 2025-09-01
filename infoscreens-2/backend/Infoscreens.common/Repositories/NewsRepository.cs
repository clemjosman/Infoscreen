using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vesact.common.file.Interfaces;
using vesact.common.file.Models;
using vesact.common.Log;
using vesact.common.message.v2;
using vesact.common.message.v2.Interfaces;
using vesact.common.message.v2.Models;
using vesact.common.translate;
using User = Infoscreens.Common.Models.EntityFramework.CMS.User;

namespace Infoscreens.Common.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly ILogger<NewsRepository> _logger;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly ITranslationRepository _translationRepository;
        private readonly IFileHelper _fileHelper;
        private readonly ILabelTranslationHelper _labelTranslationHelper;
        private readonly MessageService<IEmailProvider, FirebaseConfig_V1> _messageService;

        public NewsRepository(ILogger<NewsRepository> logger, IDatabaseRepository databaseRepository, ITranslationRepository translationRepository, IFileHelper fileHelper, ILabelTranslationHelper labelTranslationHelper, MessageService<IEmailProvider, FirebaseConfig_V1> messageService)
        {
            logger.LogDebug(new LogItem(1, "NewsRepository() Creating a new instance."));

            _logger = logger;
            _databaseRepository = databaseRepository;
            _translationRepository = translationRepository;
            _fileHelper = fileHelper;
            _labelTranslationHelper = labelTranslationHelper;
            _messageService = messageService;

            _logger.LogTrace(new LogItem(2, "NewsRepository() New instance has been created."));
        }



        #region Create or Update

        public async Task<News> CreateOrUpdateNewsAsync(apiNews_Publish publishedNews, Tenant tenant, User user)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "CreateOrUpdateNewsAsync() called.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom4 = publishedNews != null ? JsonConvert.SerializeObject(publishedNews) : "Parameter publishedNews is null."
                });

                #region Checks and Init

                // Checks
                if (publishedNews == null)
                    throw new ArgumentNullException(nameof(publishedNews));

                if (!publishedNews.CheckConsistancy())
                    throw new ArgumentException("Is not consistent.", nameof(publishedNews));

                // Init
                News news = null;
                var isNewNews = !publishedNews.Id.HasValue;

                if (!isNewNews)
                {
                    news = await _databaseRepository.GetNewsFromTenantAsync(tenant, publishedNews.Id.Value);

                    if (news == null)
                        throw new NewsNotFoundCustomException(publishedNews.Id.Value, $"News with id {publishedNews.Id.Value} has not been found in tenant with code {tenant.Code}.");
                }

                #endregion Checks and Init

                #region Attachment

                // Deleting old attachments
                if (!isNewNews && publishedNews.DeleteAttachment.HasValue && news.FileId == publishedNews.DeleteAttachment.Value)
                {
                    try
                    {
                        news.FileId = null;
                        news.File = null;
                        await _fileHelper.DeleteFileAsync(publishedNews.DeleteAttachment.Value);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(new LogItem(10, ex, "CreateOrUpdateNewsAsync() faced an exception while deleting an attachment.")
                        {
                            Custom1 = news != null ? news.ToString() : "News is new.",
                            Custom2 = publishedNews.DeleteAttachment.HasValue ? $"Id of the file to delete: #{publishedNews.DeleteAttachment.Value}" : "Id of the file used for delete request is null."
                        });
                    }
                }

                int? fileId = news?.FileId;
                // Adding new attachments
                if (publishedNews.Attachment != null && !string.IsNullOrEmpty(publishedNews.Attachment.Base64))
                {
                    try
                    {
                        var newFile = new NewFileWrapper(Convert.FromBase64String(publishedNews.Attachment.Base64),
                                                         publishedNews.Attachment.FileName,
                                                         publishedNews.Attachment.FileExtension,
                                                         user.Id.ToString(),
                                                         tenant.Id.ToString());
                        var fileMetaData = await _fileHelper.CreateFileAsync(newFile);
                        fileId = fileMetaData.FileId;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(new LogItem(10, ex, "CreateOrUpdateNewsAsync() faced an exception while creating an attachment.")
                        {
                            Custom1 = publishedNews.Attachment != null ? $"{publishedNews.Attachment}" : "Attachment is null.",
                            Custom2 = user != null ? $"{user}" : "User is null.",
                            Custom3 = tenant != null ? $"{tenant}" : "Tenant is null.",
                            Custom4 = publishedNews.Attachment != null ? $"{publishedNews.Attachment.Base64}" : "Attachment is null, therefore has no base64 value."
                        });
                    }
                }

                #endregion Attachment

                #region Creating or Updating translations
                var titleTranslation = await _translationRepository.GenerateOrUpdateTranslationAsync(news?.TitleTranslation, publishedNews.Title);
                var contentMarkdownTranslation = await _translationRepository.GenerateOrUpdateTranslationAsync(news?.ContentMarkdownTranslation, publishedNews.ContentMarkdown);
                var contentHTMLTranslation = await _translationRepository.GenerateOrUpdateTranslationAsync(news?.ContentHTMLTranslation, publishedNews.ContentHTML);


                var translatedTextsToDelete = titleTranslation.TranslatedTextsToDelete.Concat(contentMarkdownTranslation.TranslatedTextsToDelete).Concat(contentHTMLTranslation.TranslatedTextsToDelete);
                #endregion Creating or Updating translations

                #region Categories

                var categories = new List<Category>();
                var newsCategories = new List<NewsCategory>();
                var categoriesToDelete = new List<NewsCategory>();

                // Get or create assigned categories
                foreach (var categoryName in publishedNews.Categories)
                {
                    Category category = null;
                    NewsCategory newsCategory = null;

                    // First try to get the assignment from the video object
                    if (!isNewNews)
                        newsCategory = news.NewsCategories.FirstOrDefault(nc => string.Compare(nc.Category.Name, categoryName, true) == 0);

                    // Else try to get it from the DB
                    if (newsCategory == null && !isNewNews)
                        newsCategory = await _databaseRepository.GetNewsCategoryAsync(categoryName, news);

                    // If found, use the referenced category
                    if (newsCategory != null)
                        category = newsCategory.Category;


                    // Else query the DB for the category
                    category ??= await _databaseRepository.GetCategoryFromTenantAsync(tenant, categoryName);

                    // If not fount, create it
                    category ??= new Category(categoryName, tenant, user);

                    // And create assignment if not found already
                    newsCategory ??= new NewsCategory(news, category);

                    // Store references
                    categories.Add(category);
                    newsCategories.Add(newsCategory);
                }

                // Listing removed categories assignment to delete
                if (!isNewNews)
                    categoriesToDelete = news.NewsCategories.Where(nc => !newsCategories.Select(nc_ => nc_.Id).Contains(nc.Id)).ToList();


                #endregion

                #region Create or Update news

                // Create or Update news
                if (isNewNews)
                {
                    news = new News(publishedNews, tenant, titleTranslation, contentMarkdownTranslation, contentHTMLTranslation, categories, user, fileId);
                }
                else
                {
                    news = news.UpdateNews(publishedNews, titleTranslation, contentMarkdownTranslation, contentHTMLTranslation, newsCategories, user, fileId);
                }

                // Filter infoscreen assignment to have a list of assignments to create and one for the ones to delete
                var infoscreensNewsToCreate = new List<InfoscreenNews>();
                var infoscreensNewsToDelete = new List<InfoscreenNews>();

                if (isNewNews)
                {
                    infoscreensNewsToCreate.AddRange(publishedNews.AssignedToInfoscreenIds.Select(id => new InfoscreenNews() { InfoscreenId = id, News = news }));
                }
                else
                {
                    var existingInfoscreenNews = await _databaseRepository.GetInfoscreensNewsForNewsAsync(news.Id);
                    infoscreensNewsToDelete = existingInfoscreenNews.Where(e => !publishedNews.AssignedToInfoscreenIds.Contains(e.InfoscreenId)).ToList();
                    infoscreensNewsToCreate.AddRange(publishedNews.AssignedToInfoscreenIds.Where(id => !existingInfoscreenNews.Any(e => e.InfoscreenId == id)).Select(id => new InfoscreenNews() { InfoscreenId = id, News = news }));
                }



                // Persist the news in the database
                news = await _databaseRepository.CreateOrUpdateNewsAsync(news, translatedTextsToDelete, infoscreensNewsToCreate, infoscreensNewsToDelete, categoriesToDelete);

                #endregion Create or Update news

                _logger.LogDebug(new LogItem(11, "CreateOrUpdateNewsAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom4 = publishedNews != null ? JsonConvert.SerializeObject(publishedNews) : "Parameter publishedNews is null.",
                    Custom3 = news != null ? news.ToString() : "No news created or updated."
                });

                // Return the news
                return news;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "CreateOrUpdateNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Create or Update

        #region Get

        public async Task NotifyNewsForTenantAsync(Tenant tenant)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "NotifyNewsForTenant() called.") { 
                    Custom1 = tenant != null ? $"{tenant}" : "Parameter tenant is null"
                });

                // Ensure right Push Provider config is used
                var config = CommonConfigHelper.GetTenantMessageServiceConfig(tenant);
                _messageService.OverridePushConfig(config.PushProviderConfig);

                // Checks
                if (tenant == null)
                    throw new ArgumentNullException(nameof(tenant));

                // Get news to notify
                var news = (await _databaseRepository.GetNewsForNotificationAsync(tenant, DateTimeOffset.UtcNow.Subtract(CommonConfigHelper.NewsNotificationPeriod))).ToList();
                
                if(news == null || news.Count == 0)
                {
                    _logger.LogDebug(new LogItem(11, "NotifyNewsForTenant() finished.")
                    {
                        Custom1 = "No news to notify"
                    });
                    return;
                }

                // Matching all subscribed users with a list of news that must be notified
                var usersToNotifyForNews = new List<KeyValuePair<User, Tuple<PushToken, List<News>>>>();
                foreach(var singleNews in news)
                {
                    foreach(var infoscreenNews in singleNews.InfoscreensNews)
                    {
                        foreach(var subscription in infoscreenNews.Infoscreen.Subscriptions)
                        {
                            var index = usersToNotifyForNews.FindIndex(i => i.Key.Id == subscription.User.Id);
                            if (index == -1)
                            {
                                usersToNotifyForNews.Add(new KeyValuePair<User, Tuple<PushToken, List<News>>>(subscription.User, Tuple.Create(subscription.PushToken, new List<News>() { singleNews })));
                            }
                            else
                            {
                                if (!usersToNotifyForNews[index].Value.Item2.Any(n => n.Id == singleNews.Id))
                                {
                                    usersToNotifyForNews[index].Value.Item2.Add(singleNews);
                                }

                                // Re-register if previous PushToken was null and this one is defined
                                if(usersToNotifyForNews[index].Value.Item1 == null && subscription.PushToken != null)
                                {
                                    usersToNotifyForNews[index] = new KeyValuePair<User, Tuple<PushToken, List<News>>>(subscription.User, Tuple.Create(subscription.PushToken, usersToNotifyForNews[index].Value.Item2));
                                }
                            }
                        }
                    }
                }

                var languages = await _databaseRepository.GetAllLanguageAsync();

                var retryList = new List<Tuple<User, PushRequest_Fcm_V1>>();
                var noTokenList = new List<User>();

                foreach(var userAndNews in usersToNotifyForNews)
                {
                    if (userAndNews.Value.Item1 == null) {
                        noTokenList.Add(userAndNews.Key);
                        continue;
                    }

                    var language = languages.FirstOrDefault(l => l.Iso2 == userAndNews.Key.Iso2, languages.First());

                    string title, message;
                    // Single news to notify for this user
                    if (userAndNews.Value.Item2.Count == 1)
                    {
                        // Get the news title in the user's language
                        var translatedTitleTexts = userAndNews.Value.Item2.First().TitleTranslation.TranslatedTexts;
                        title = translatedTitleTexts.FirstOrDefault(t => t.LanguageId == language.Id)?.Text;
                        
                        // If user language could not be found, use the first language defined
                        title ??= translatedTitleTexts.First().Text;

                        message = await _labelTranslationHelper.GetTextCodeLabelAsync("push.localNews.label", language.Iso2);
                    }

                    // Multiple news to notify for this user
                    else
                    {
                        title = await _labelTranslationHelper.GetTextCodeLabelAsync("push.localNews.label", language.Iso2);
                        message = "";
                        bool firstNews = true;
                        foreach(var newsToNotify in userAndNews.Value.Item2)
                        {
                            // Get the news title in the infoscreen language
                            var translatedTitleTexts = newsToNotify.TitleTranslation.TranslatedTexts;
                            var newsTitle = translatedTitleTexts.FirstOrDefault(t => t.LanguageId == language.Id)?.Text;

                            // If Infoscreen language could not be found, use the first language defined
                            newsTitle ??= translatedTitleTexts.First().Text;

                            message += $"{(firstNews ? "" : "\n")}  - {newsTitle}";
                            firstNews = false;
                        }
                    }

                    var pushRequest = new PushRequest_Fcm_V1(null, title, message)
                    {
                        Token = userAndNews.Value.Item1.Token,
                        Icon = "fcm_push_icon",
                        CustomParameters = new Dictionary<string, string>() { { "action", "NEWS_UPDATE" } }
                    };

                    try {
                        _logger.LogInformation(new LogItem(115, $"NotifyNewsForTenant() sending to: {userAndNews.Key.Upn}"));
                        await _messageService.SendPushAsync(pushRequest);
                    }
                    catch (Exception ex)
                    {
                        retryList.Add(Tuple.Create(userAndNews.Key, pushRequest));
                        _logger.LogError(new LogItem(300, ex, "NotifyNewsForTenant() has thrown an exception when sending a notification to {0}: {1}, ", userAndNews.Key.Upn, ex.Message));
                    }
                    Thread.Sleep(50);
                }

                // Some push are not send correctly right away, retry - See https://firebase.google.com/docs/cloud-messaging/scale-fcm#exponential_backoff
                if (retryList.Count > 0) {
                    int retryCount = 0;
                    const int MAX_RETRY = 2;
                    while (retryList.Count > 0 && retryCount < MAX_RETRY)
                    {
                        retryCount++;
                        Thread.Sleep(5000);
                        var failedRetry = new List<Tuple<User, PushRequest_Fcm_V1>>();
                        foreach (var userAndNotification in retryList)
                        {
                            try
                            {
                                _logger.LogInformation(new LogItem(115, $"NotifyNewsForTenant() retry sending to: {userAndNotification.Item1.Upn}"));
                                await _messageService.SendPushAsync(userAndNotification.Item2);
                            }
                            catch (Exception ex)
                            {
                                failedRetry.Add(userAndNotification);
                                _logger.LogError(new LogItem(300, ex, "NotifyNewsForTenant() has thrown an exception when retrying to send a notification to {0}: {1}, ", userAndNotification.Item1.Upn, ex.Message));
                            }
                            Thread.Sleep(50);
                        }
                        retryList = failedRetry;
                    }
                }

                if (retryList.Count > 0)
                {
                    _logger.LogError(new LogItem(300, $"NotifyNewsForTenant() Failed to notify {retryList.Count} of the {usersToNotifyForNews.Count - noTokenList.Count} users for {tenant.DisplayName}: {string.Join(", ", retryList.Select(i => i.Item1.Upn))}"));
                }

                _logger.LogInformation(new LogItem(300, $"NotifyNewsForTenant() Successfully notified {usersToNotifyForNews.Count - retryList.Count - noTokenList.Count} users for {tenant.DisplayName}. There are {noTokenList.Count} users who refused notifications. Failed to notify {retryList.Count} users."));


                await MarkNewsAsNotifiedAsync(news.Select(n => n.Id).Distinct().ToList());

                // Reset to default config
                _messageService.OverridePushConfig(null);

                _logger.LogDebug(new LogItem(11, "NotifyNewsForTenant() finished.")
                {
                    Custom1 = $"Notified {news.Count} news."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "NotifyNewsForTenant() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }



        #endregion

        #region Delete

        public async Task DeleteNewsAsync(News news)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "DeleteNewsAsync() called.")
                {
                    Custom1 = news != null ? news.ToString() : "Parameter news is null."
                });

                // Checks
                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                int? fileId = news.FileId;

                // Delete the news in the database
                await _databaseRepository.DeleteNewsAsync(news);

                try
                {
                    // Delete attachment
                    if (fileId.HasValue)
                        await _fileHelper.DeleteFileAsync(fileId.Value);
                }
                catch(Exception ex)
                {
                    _logger.LogError(new LogItem(300, ex, "DeleteNewsAsync() has thrown an exception while deleting file #" + fileId.Value + " attached to a news that has just been deleted. Exception message: {0}", ex.Message));
                }

                _logger.LogDebug(new LogItem(11, "DeleteNewsAsync() finished.")
                {
                    Custom1 = news != null ? news.ToString() : "Deleted news is null."
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task DeleteNewsAsync(List<News> news)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "DeleteNewsAsync() called.")
                {
                    Custom1 = news != null ? $"Received {news.Count} news to delete" : "Parameter news is null."
                });

                // Checks
                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                List<int?> fileIds = news.Select(n => n.FileId).ToList();

                // Delete the news in the database
                await _databaseRepository.DeleteNewsAsync(news);

                foreach (var fileId in fileIds)
                {
                    try
                    {
                        // Delete attachment
                        if (fileId.HasValue)
                            await _fileHelper.DeleteFileAsync(fileId.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(new LogItem(300, ex, "DeleteNewsAsync() has thrown an exception while deleting file #" + fileId.Value + " attached to a news that has just been deleted. Exception message: {0}", ex.Message));
                    }
                }
                

                _logger.LogDebug(new LogItem(11, "DeleteNewsAsync() finished.")
                {
                    Custom1 = news != null ? $"Deleted {news.Count} news" : "Deleted news is null."
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Delete

        #region Notification

        public async Task<News> MarkNewsAsNotifiedAsync(News news)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "MarkNewsAsNotified() called.")
                {
                    Custom1 = news != null ? news.ToString() : "Parameter news is null."
                });

                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                news.UsersNotified = DateTimeOffset.UtcNow;
                await _databaseRepository.CreateOrUpdateNewsAsync(news, null, null, null, null);

                _logger.LogDebug(new LogItem(11, "MarkNewsAsNotified() finished.")
                {
                    Custom1 = news != null ? news.ToString() : "Parameter news is null."
                });

                return news;

            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "MarkNewsAsNotified() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<News>> MarkNewsAsNotifiedAsync(List<int> newsIds)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "MarkNewsAsNotified() called.")
                {
                    Custom1 = newsIds != null ? $"{newsIds.Count} news ids provided." : "Parameter newsIds is null."
                });

                if (newsIds == null)
                    throw new ArgumentNullException(nameof(newsIds));

                var news = await this._databaseRepository.GetNewsAsync(newsIds);

                news = news.Select(n => { n.UsersNotified = DateTimeOffset.UtcNow; return n; }).ToList();
                news = (await _databaseRepository.UpdateNewsAsync(news)).ToList();

                _logger.LogDebug(new LogItem(11, "MarkNewsAsNotified() finished.")
                {
                    Custom1 = news != null ? $"{news.Count} news updated." : "Parameter news is null."
                });

                return news;

            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "MarkNewsAsNotified() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<News> ResetNewsNotificationAsync(News news, User user)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "ResetNewsNotificationAsync() called.")
                {
                    Custom1 = news != null ? news.ToString() : "Parameter news is null.",
                    Custom2 = user != null ? user.ToString() : "Parameter user is null."
                });

                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                news.UsersNotified = null;
                news.LastEditDate = DateTimeOffset.UtcNow;
                news.LastEditedBy = user.Id;
                await _databaseRepository.CreateOrUpdateNewsAsync(news, null, null, null, null);

                _logger.LogDebug(new LogItem(11, "ResetNewsNotificationAsync() finished.")
                {
                    Custom1 = news != null ? news.ToString() : "Parameter news is null.",
                    Custom2 = user != null ? user.ToString() : "Parameter user is null."
                });

                return news;

            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "ResetNewsNotificationAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion

        #region Translate

        public async Task<apiNews_Translated> TranslateNewsAsync(apiNews_Translate apiNews_Translate)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "TranslateNewsAsync() called.")
                {
                    Custom4 = apiNews_Translate != null ? JsonConvert.SerializeObject(apiNews_Translate) : "Parameter apiNews_Translate is null."
                });

                if (apiNews_Translate == null)
                    throw new ArgumentNullException(nameof(apiNews_Translate));

                var translationHelper = new TranslationHelper_Microsoft(CommonConfigHelper.TransationApiKey_Microsoft);

                var fromLanguage = await _databaseRepository.GetLanguageAsync(apiNews_Translate.From);
                var toLanguage = await _databaseRepository.GetLanguageAsync(apiNews_Translate.To);

                if (fromLanguage == null)
                    throw new ArgumentException($"Language {apiNews_Translate.From} is not supported.");

                if (toLanguage == null)
                    throw new ArgumentException($"Language {apiNews_Translate.To} is not supported.");

                var translatedTitle = await translationHelper.TranslateTextAsync(toLanguage.Iso2, apiNews_Translate.Title);
                var translatedContent = await translationHelper.TranslateTextAsync(toLanguage.Iso2, apiNews_Translate.Content);

                var apiNews_Translated = new apiNews_Translated(toLanguage.ToApiLanguage_Light(), translatedTitle, translatedContent);

                _logger.LogDebug(new LogItem(11, "TranslateNewsAsync() finished.")
                {
                    Custom4 = apiNews_Translated != null ? JsonConvert.SerializeObject(apiNews_Translated) : "Translated result apiNews_Translated is null."
                });

                return apiNews_Translated;

            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "TranslateNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Translate
    }
}
