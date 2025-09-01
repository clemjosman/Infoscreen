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
using System.Threading.Tasks;
using vesact.common.Log;
using vesact.common.translate;

namespace Infoscreens.Common.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ILogger<VideoRepository> _logger;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly ITranslationRepository _translationRepository;

        public VideoRepository(ILogger<VideoRepository> logger, IDatabaseRepository databaseRepository, ITranslationRepository translationRepository)
        {
            logger.LogDebug(new LogItem(1, "VideoRepository() Creating a new instance."));

            _logger = logger;
            _databaseRepository = databaseRepository;
            _translationRepository = translationRepository;

            _logger.LogTrace(new LogItem(2, "VideoRepository() New instance has been created."));
        }

        #region Create or Update

        public async Task<Video> CreateOrUpdateVideoAsync(apiVideo_Publish publishedVideo, Tenant tenant, User user)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "CreateOrUpdateVideoAsync() called.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom4 = publishedVideo != null ? JsonConvert.SerializeObject(publishedVideo) : "Parameter publishedVideo is null."
                });

                #region Checks and Init

                // Checks
                if (publishedVideo == null)
                    throw new ArgumentNullException(nameof(publishedVideo));

                if(!publishedVideo.CheckConsistancy())
                    throw new ArgumentException("Is not consistent.", nameof(publishedVideo));

                // Init
                Video video = null;
                var isNewVideo = !publishedVideo.Id.HasValue;

                if(!isNewVideo)
                {
                    video = await _databaseRepository.GetVideoFromTenantAsync(tenant, publishedVideo.Id.Value);

                    if (video == null)
                        throw new VideoNotFoundCustomException(publishedVideo.Id.Value, $"Video with id {publishedVideo.Id.Value} has not been found in tenant with code {tenant.Code}.");
                }

                #endregion Checks and Init

                #region Creating or Updating translations

                var titleTranslation = await _translationRepository.GenerateOrUpdateTranslationAsync(video?.TitleTranslation, publishedVideo.Title);
                var translatedTextsToDelete = titleTranslation.TranslatedTextsToDelete;

                #endregion Creating or Updating translations

                #region Categories

                var categories = new List<Category>();
                var videoCategories = new List<VideoCategory>();
                var categoriesToDelete = new List<VideoCategory>();

                // Get or create assigned categories
                foreach (var categoryName in publishedVideo.Categories)
                {
                    Category category = null;
                    VideoCategory videoCategory = null;

                    // First try to get the assignment from the video object
                    if (!isNewVideo)
                        videoCategory = video.VideoCategories.FirstOrDefault(vc => string.Compare(vc.Category.Name, categoryName, true) == 0);

                    // Else try to get it from the DB
                    if (videoCategory == null && !isNewVideo)
                        videoCategory = await _databaseRepository.GetVideoCategoryAsync(categoryName, video);

                    // If found, use the referenced category
                    if (videoCategory != null)
                        category = videoCategory.Category;


                    // Else query the DB for the category
                    category ??= await _databaseRepository.GetCategoryFromTenantAsync(tenant, categoryName);
                    
                    // If not fount, create it
                    category ??= new Category(categoryName, tenant, user);

                    // And create assignment if not found already
                    videoCategory ??= new VideoCategory(video, category);

                    // Store references
                    categories.Add(category);
                    videoCategories.Add(videoCategory);
                }

                // Listing removed categories assignment to delete
                if (!isNewVideo)
                    categoriesToDelete = video.VideoCategories.Where(vc => !videoCategories.Select(vc_ => vc_.Id).Contains(vc.Id)).ToList();

                #endregion

                #region Create or Update video

                // Create or Update video
                if (isNewVideo)
                {
                    video = new Video(publishedVideo, tenant, titleTranslation, categories, user);
                }
                else
                {
                    video = video.UpdateVideo(publishedVideo, titleTranslation, videoCategories, user);
                }

                // Filter infoscreen assignment to have a list of assignments to create and one for the ones to delete
                var infoscreensVideosToCreate = new List<InfoscreenVideo>();
                var infoscreensVideosToDelete = new List<InfoscreenVideo>();

                if (isNewVideo)
                {
                    infoscreensVideosToCreate.AddRange(publishedVideo.AssignedToInfoscreenIds.Select(id => new InfoscreenVideo() { InfoscreenId = id, Video = video }));
                }
                else
                {
                    var existingInfoscreenVideos = await _databaseRepository.GetInfoscreensVideosForVideoAsync(video.Id);
                    infoscreensVideosToDelete = existingInfoscreenVideos.Where(e => !publishedVideo.AssignedToInfoscreenIds.Contains(e.InfoscreenId)).ToList();
                    infoscreensVideosToCreate.AddRange(publishedVideo.AssignedToInfoscreenIds.Where(id => !existingInfoscreenVideos.Any(e => e.InfoscreenId == id)).Select(id => new InfoscreenVideo() { InfoscreenId = id, Video = video }));
                }



                // Persist the video in the database
                video = await _databaseRepository.CreateOrUpdateVideoAsync(video, translatedTextsToDelete, infoscreensVideosToCreate, infoscreensVideosToDelete, categoriesToDelete);

                #endregion Create or Update video

                _logger.LogDebug(new LogItem(11, "CreateOrUpdateVideoAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom4 = publishedVideo != null ? JsonConvert.SerializeObject(publishedVideo) : "Parameter publishedVideo is null.",
                    Custom3 = video != null ? video.ToString() : "No video created or updated."
                });

                // Return the video
                return video;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "CreateOrUpdateVideoAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Create or Update

        #region Delete

        public async Task DeleteVideoAsync(Video video)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "DeleteVideoAsync() called.")
                {
                    Custom1 = video != null ? video.ToString() : "Parameter video is null."
                });

                // Checks
                if (video == null)
                    throw new ArgumentNullException(nameof(video));


                // Delete the video in the database
                await _databaseRepository.DeleteVideoAsync(video);

                _logger.LogDebug(new LogItem(11, "DeleteVideoAsync() finished.")
                {
                    Custom1 = video != null ? video.ToString() : "Deleted video is null."
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteVideoAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task DeleteVideoAsync(List<Video> video)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "DeleteVideoAsync() called.")
                {
                    Custom1 = video != null ? $"Received {video.Count} videos to delete" : "Parameter news is null."
                });

                // Checks
                if (video == null)
                    throw new ArgumentNullException(nameof(video));


                // Delete the video in the database
                await _databaseRepository.DeleteVideoAsync(video);

                _logger.LogDebug(new LogItem(11, "DeleteVideoAsync() finished.")
                {
                    Custom1 = video != null ? $"Deleted {video.Count} videos" : "Deleted video is null."
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteVideoAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Delete

        #region Translate

        public async Task<apiVideo_Translated> TranslateVideoAsync(apiVideo_Translate apiVideo_Translate)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "TranslateVideoAsync() called.")
                {
                    Custom4 = apiVideo_Translate != null ? JsonConvert.SerializeObject(apiVideo_Translate) : "Parameter apiVideo_Translate is null."
                });

                if (apiVideo_Translate == null)
                    throw new ArgumentNullException(nameof(apiVideo_Translate));

                var translationHelper = new TranslationHelper_Microsoft(CommonConfigHelper.TransationApiKey_Microsoft);

                var fromLanguage = await _databaseRepository.GetLanguageAsync(apiVideo_Translate.From);
                var toLanguage = await _databaseRepository.GetLanguageAsync(apiVideo_Translate.To);

                if(fromLanguage == null)
                    throw new ArgumentNullException($"Language {apiVideo_Translate.From} is not supported.");

                if (toLanguage == null)
                    throw new ArgumentNullException($"Language {apiVideo_Translate.To} is not supported.");

                var translatedTitle = await translationHelper.TranslateTextAsync(toLanguage.Iso2, apiVideo_Translate.Title);

                var apiVideo_Translated = new apiVideo_Translated(toLanguage.ToApiLanguage_Light(), translatedTitle);

                _logger.LogDebug(new LogItem(11, "TranslateVideoAsync() finished.")
                {
                    Custom4 = apiVideo_Translated != null ? JsonConvert.SerializeObject(apiVideo_Translated) : "Translated result apiVideo_Translated is null."
                });

                return apiVideo_Translated;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "TranslateVideoAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Translate
    }
}
