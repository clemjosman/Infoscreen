using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.API.Mobile;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;
using Z.EntityFramework.Plus;

namespace Infoscreens.Common.Repositories
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly ILogger<DatabaseRepository> _logger;
        public DatabaseRepository(ILogger<DatabaseRepository> logger)
        {
            logger.LogDebug(new LogItem(1, "DatabaseRepository() Creating a new instance."));

            _logger = logger;

            _logger.LogTrace(new LogItem(2, "DatabaseRepository() New instance has been created."));
        }


        #region Category

        public async Task<List<Category>> GetCategoriesFromTenantAsync(Tenant tenant)
        {
            _logger.LogDebug(new LogItem(10, "GetCategoriesFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null."
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Categories
                                          .Where(c => c.TenantId == tenant.Id)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetCategoriesFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = result != null ? result.ToString() : "No video found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetCategoriesFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Category> GetCategoryFromTenantAsync(Tenant tenant, string category)
        {
            _logger.LogDebug(new LogItem(10, "GetCategoryFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                Custom2 = category
            });

            var categoryLower = category.ToLower();

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Categories
                                          .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryLower && c.TenantId == tenant.Id);

                _logger.LogDebug(new LogItem(11, "GetVideoFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = category,
                    Custom3 = result != null ? result.ToString() : "No video found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetVideoFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<Category>> GetUnassociatedCategoriesAsync()
        {
            _logger.LogDebug(new LogItem(10, "GetUnassociatedCategoriesAsync() called."));

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Categories
                                          .IncludeOptimized(c => c.NewsCategories)
                                          .IncludeOptimized(c => c.VideosCategories)
                                          .Where(c => c.NewsCategories.Count == 0 && c.VideosCategories.Count == 0)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetUnassociatedCategoriesAsync() finished.")
                {
                    Custom3 = $"Returned {result.Count} categories."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetUnassociatedCategoriesAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task DeleteCategoriesAsync(List<Category> categories)
        {
            _logger.LogDebug(new LogItem(10, "DeleteCategoriesAsync() called.") { 
                Custom1 = $"Received {categories.Count} categories to delete"
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                context.Categories.RemoveRange(categories);
                await context.SaveChangesAsync();

                _logger.LogDebug(new LogItem(11, "DeleteCategoriesAsync() finished.")
                {
                    Custom3 = $"Deleted {categories.Count} categories."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteCategoriesAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }


        #endregion Category

        #region Infoscreen

        public async Task<IEnumerable<Infoscreen>> GetInfoscreensAsync()
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreensAsync() called."));

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Infoscreens
                                          .IncludeOptimized(i => i.InfoscreenGroup)
                                          .IncludeOptimized(i => i.InfoscreenGroup.Tenant)
                                          .IncludeOptimized(i => i.DefaultContentLanguage)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetInfoscreensFromTenantAsync() finished.")
                {
                    Custom2 = $"Returning list of {result.Count} Infoscreen(s)."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreensAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<IEnumerable<Infoscreen>> GetInfoscreensFromTenantAsync(Tenant tenant)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreensFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null."
            });

            try
            {
                if (tenant == null)
                    throw new ArgumentNullException(nameof(tenant));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Infoscreens
                                          .IncludeOptimized(i => i.InfoscreenGroup)
                                          .IncludeOptimized(i => i.DefaultContentLanguage)
                                          .Where(i => i.InfoscreenGroup.TenantId == tenant.Id)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetInfoscreensFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = $"Returning list of {result.Count} Infoscreen(s)."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreensFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Infoscreen> GetInfoscreenByNodeIdAsync(string nodeId)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreenByNodeIdAsync() called.")
            {
                Custom1 = !string.IsNullOrWhiteSpace(nodeId) ? nodeId : "Parameter nodeId is null or empty."
            });

            try
            {
                if (string.IsNullOrWhiteSpace(nodeId))
                    throw new ArgumentNullException(nameof(nodeId));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Infoscreens
                                          .Include(i => i.InfoscreenGroup)
                                          .Include(i => i.DefaultContentLanguage)
                                          .FirstOrDefaultAsync(i => i.NodeId == nodeId);

                _logger.LogDebug(new LogItem(11, "GetInfoscreenByNodeIdAsync() finished.")
                {
                    Custom1 = !string.IsNullOrWhiteSpace(nodeId) ? nodeId : "Parameter nodeId is null or empty.",
                    Custom2 = result != null ? result.ToString() : "No infoscreen found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreenByNodeIdAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Infoscreen> GetInfoscreenByIdAsync(int infoscreenId)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreenByIdAsync() called.")
            {
                Custom1 = infoscreenId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Infoscreens
                                          .IncludeOptimized(i => i.InfoscreenGroup)
                                          .IncludeOptimized(i => i.DefaultContentLanguage)
                                          .FirstOrDefaultAsync(i => i.Id == infoscreenId);

                _logger.LogDebug(new LogItem(11, "GetInfoscreenByIdAsync() finished.")
                {
                    Custom1 = infoscreenId.ToString(),
                    Custom2 = $"Returning {result?.ToString()}"
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreenByIdAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<Infoscreen>> GetInfoscreensByIdsAsync(IEnumerable<int> infoscreensIds)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreensByIdsAsync() called.")
            {
                Custom1 = infoscreensIds != null ? string.Join(", ", infoscreensIds) : "Parameter infoscreensIds is null."
            });

            try
            {
                if (infoscreensIds == null)
                    throw new ArgumentNullException(nameof(infoscreensIds));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Infoscreens
                                          .IncludeOptimized(i => i.InfoscreenGroup)
                                          .Where(i => infoscreensIds.Contains(i.Id))
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetInfoscreensByIdsAsync() finished.")
                {
                    Custom1 = infoscreensIds != null ? string.Join(", ", infoscreensIds) : "Parameter infoscreensIds is null.",
                    Custom2 = $"Returning list of {result.Count} Infoscreen(s)."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreensByIdsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<Infoscreen>> GetInfoscreensByNewsIdAsync(int newsId)
        {
            _logger.LogDebug(new LogItem(10, $"{nameof(GetInfoscreensByNewsIdAsync)}() called.")
            {
                Custom1 = newsId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Infoscreens
                                          .IncludeOptimized(i => i.InfoscreenNews)
                                          .Where(i => i.InfoscreenNews.Any(n => n.NewsId == newsId))
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, $"{nameof(GetInfoscreensByNewsIdAsync)}() finished.")
                {
                    Custom1 = newsId.ToString(),
                    Custom2 = $"Returning list of {result.Count} Infoscreen(s)."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, $"{nameof(GetInfoscreensByNewsIdAsync)}() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task UpdateInfoscreenAsync(Infoscreen infoscreen)
        {
            _logger.LogDebug(new LogItem(10, "UpdateInfoscreenAsync() called.")
            {
                Custom1 = infoscreen != null ? infoscreen.ToString() : "Parameter infoscreen is null."
            });

            try
            {
                if (infoscreen == null)
                    throw new ArgumentNullException(nameof(infoscreen));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                context.Infoscreens.Update(infoscreen);
                await context.SaveChangesAsync();

                _logger.LogDebug(new LogItem(11, "UpdateInfoscreenAsync() finished.")
                {
                    Custom1 = infoscreen != null ? infoscreen.ToString() : "Parameter infoscreen is null."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "UpdateInfoscreenAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Infoscreen

        #region Infoscreen Groups

        public async Task<IEnumerable<InfoscreenGroup>> GetInfoscreenGroupsFromTenantAsync(Tenant tenant)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreenGroupsFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null."
            });

            try
            {
                if (tenant == null)
                    throw new ArgumentNullException(nameof(tenant));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.InfoscreensGroups
                                          .Where(ig => ig.TenantId == tenant.Id)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetInfoscreenGroupsFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = $"Returning list of {result.Count} Infoscreen group(s)."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreenGroupsFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Infoscreen Groups

        #region InfoscreensNews

        public async Task<List<InfoscreenNews>> GetInfoscreensNewsForInfoscreenAsync(int infoscreenId)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreensNewsForNewsAsync() called.")
            {
                Custom1 = infoscreenId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                List<InfoscreenNews> result;

                result = await context.InfoscreensNews
                                      .IncludeOptimized(i => i.News)
                                      .Where(e => e.InfoscreenId == infoscreenId)
                                      .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetInfoscreensNewsForNewsAsync() finished.")
                {
                    Custom1 = infoscreenId.ToString(),
                    Custom2 = $"Returning list of {result.Count} InfoscreenNews relations."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreensNewsForNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<InfoscreenNews>> GetInfoscreensNewsForNewsAsync(int newsId)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreensNewsForNewsAsync() called.")
            {
                Custom1 = newsId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                List<InfoscreenNews> result;

                result = await context.InfoscreensNews
                                        .Where(e => e.NewsId == newsId)
                                        .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetInfoscreensNewsForNewsAsync() finished.")
                {
                    Custom1 = newsId.ToString(),
                    Custom2 = $"Returning list of {result.Count} InfoscreenNews relations."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreensNewsForNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<InfoscreenNews>> GetInfoscreensNewsForNewsAsync(List<int> newsIds)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreensNewsForNewsAsync() called.")
            {
                Custom1 = string.Join(",",newsIds)
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                List<InfoscreenNews> result;

                result = await context.InfoscreensNews
                                        .Where(e => newsIds.Contains(e.NewsId))
                                        .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetInfoscreensNewsForNewsAsync() finished.")
                {
                    Custom1 = string.Join(",", newsIds),
                    Custom2 = $"Returning list of {result.Count} InfoscreenNews relations."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreensNewsForNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }


        #endregion InfoscreensNews

        #region InfoscreensVideos

        public async Task<List<InfoscreenVideo>> GetInfoscreensVideosForVideoAsync(int videoId)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreensVideosForVideoAsync() called.")
            {
                Custom1 = videoId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.InfoscreensVideos
                                          .Where(e => e.VideoId == videoId)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetInfoscreensVideosForVideoAsync() finished.")
                {
                    Custom1 = videoId.ToString(),
                    Custom2 = $"Returning list of {result.Count} InfoscreenVideos relations."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreensVideosForVideoAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }
        
        public async Task<List<InfoscreenVideo>> GetInfoscreensVideosForVideoAsync(List<int> videoIds)
        {
            _logger.LogDebug(new LogItem(10, "GetInfoscreensVideosForVideoAsync() called.")
            {
                Custom1 = string.Join(",", videoIds)
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.InfoscreensVideos
                                          .Where(e => videoIds.Contains(e.VideoId))
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetInfoscreensVideosForVideoAsync() finished.")
                {
                    Custom1 = string.Join(",", videoIds),
                    Custom2 = $"Returning list of {result.Count} InfoscreenVideos relations."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetInfoscreensVideosForVideoAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion InfoscreensVideos

        #region Language

        public async Task<IEnumerable<Language>> GetAllLanguageAsync()
        {
            _logger.LogDebug(new LogItem(10, "GetAllLanguageAsync() called.")
            { });

            try
            {

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Languages.ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetAllLanguageAsync() finished.")
                {
                   Custom1 = $"Returning list of {result.Count} Language(s)."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetAllLanguageAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Language> GetLanguageAsync(apiLanguage_Light apiLanguage_Light)
        {
            _logger.LogDebug(new LogItem(10, "GetLanguageAsync() called.")
            {
                Custom1 = apiLanguage_Light != null ? apiLanguage_Light.ToString() : "Parameter apiLanguage_Light is null."
            });

            try
            {
                if (apiLanguage_Light == null)
                    throw new ArgumentNullException(nameof(apiLanguage_Light));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Languages
                                          .FirstOrDefaultAsync(l => apiLanguage_Light.Iso2 == l.Iso2);

                _logger.LogDebug(new LogItem(11, "GetLanguageAsync() finished.")
                {
                    Custom1 = apiLanguage_Light != null ? apiLanguage_Light.ToString() : "Parameter apiLanguage_Light is null.",
                    Custom2 = result != null ? result.ToString() : "No language found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetLanguageAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Language> GetLanguageAsync(int languageId)
        {
            _logger.LogDebug(new LogItem(10, "GetLanguageAsync() called.")
            {
                Custom1 = languageId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Languages
                                          .FirstOrDefaultAsync(l => l.Id == languageId);

                _logger.LogDebug(new LogItem(11, "GetLanguageAsync() finished.")
                {
                    Custom1 = languageId.ToString(),
                    Custom2 = result != null ? result.ToString() : "No language found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetLanguageAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Language> GetLanguageAsync(string iso2)
        {
            _logger.LogDebug(new LogItem(10, "GetLanguageAsync() called.")
            {
                Custom1 = !string.IsNullOrWhiteSpace(iso2) ? iso2 : "Parameter iso2 is null or empty."
            });

            try
            {
                if (iso2 == null)
                    throw new ArgumentNullException(nameof(iso2));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Languages
                                          .FirstOrDefaultAsync(l => iso2 == l.Iso2);

                _logger.LogDebug(new LogItem(11, "GetLanguageAsync() finished.")
                {
                    Custom1 = !string.IsNullOrWhiteSpace(iso2) ? iso2 : "Parameter iso2 is null or empty.",
                    Custom2 = result != null ? result.ToString() : "No language found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetLanguageAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Language

        #region News

        public async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            _logger.LogDebug(new LogItem(10, "GetAllNewsAsync() called.")
            {});

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.News
                                          .Where(n => n.DeletionDate.HasValue == false)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetAllNewsAsync() finished.")
                {
                    Custom1 = $"Returning list of {result.Count} news."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetAllNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<News>> GetNewsAsync(List<int> newsIds)
        {
            _logger.LogDebug(new LogItem(10, "GetNewsAsync() called.")
            {
                Custom1 = newsIds != null ? $"{newsIds.Count} news provided." : "Parameter news is null."
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.News
                                          .Where(n => newsIds.Contains(n.Id))
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetAllNewsAsync() finished.")
                {
                    Custom1 = $"Returning list of {result.Count} news."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetAllNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<IEnumerable<News>> GetAllNewsFromTenantAsync(Tenant tenant, string search = "", IEnumerable<int> infoscreenIds = null, IEnumerable<int> categoryIds = null)
        {
            _logger.LogDebug(new LogItem(10, "GetAllNewsFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                Custom2 = search,
                Custom3 = infoscreenIds != null && infoscreenIds.Any() ? string.Join(",", infoscreenIds) : "",
                Custom4 = categoryIds != null && categoryIds.Any() ? string.Join(",", categoryIds) : ""
            });

            try
            {
                if (tenant == null)
                    throw new ArgumentNullException(nameof(tenant));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var query = context.News
                                          .IncludeOptimized(n => n.File)
                                          .IncludeOptimized(n => n.TitleTranslation)
                                            .IncludeOptimized(n => n.TitleTranslation.TranslatedTexts)
                                              .IncludeOptimized(n => n.TitleTranslation.TranslatedTexts.Select(t => t.Language))
                                          .IncludeOptimized(n => n.ContentMarkdownTranslation)
                                            .IncludeOptimized(n => n.ContentMarkdownTranslation.TranslatedTexts)
                                              .IncludeOptimized(n => n.ContentMarkdownTranslation.TranslatedTexts.Select(t => t.Language))
                                          .IncludeOptimized(n => n.ContentHTMLTranslation)
                                            .IncludeOptimized(n => n.ContentHTMLTranslation.TranslatedTexts)
                                              .IncludeOptimized(n => n.ContentHTMLTranslation.TranslatedTexts.Select(t => t.Language))
                                          .IncludeOptimized(n => n.NewsCategories)
                                            .IncludeOptimized(n => n.NewsCategories.Select(nc => nc.Category))
                                          .IncludeOptimized(n => n.Creator)
                                          .IncludeOptimized(n => n.LastEditor)
                                          .IncludeOptimized(n => n.InfoscreensNews)
                                          .Where(n => n.DeletionDate.HasValue == false)
                                          .Where(n => n.TenantId == tenant.Id);


                // Infoscreen filtering
                if (infoscreenIds != null && infoscreenIds.Any())
                {
                    query = query.Where(n => n.InfoscreensNews.Any(ine => infoscreenIds.Contains(ine.InfoscreenId)));
                }

                // Category filtering
                if (categoryIds != null && categoryIds.Any())
                {
                    query = query.Where(n => n.NewsCategories.Any(nc => categoryIds.Contains(nc.CategoryId)));
                }

                // Text search
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(n => n.TitleTranslation.TranslatedTexts.Any(tt => tt.Text.Contains(search)) 
                                  || n.ContentMarkdownTranslation.TranslatedTexts.Any(tt => tt.Text.Contains(search))
                                  || n.Description.Contains(search)
                    );
                }

                var result = await query.ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetAllNewsFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = $"Returning list of {result.Count} news."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetAllNewsFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<IEnumerable<News>> GetNewsForNotificationAsync(Tenant tenant, DateTimeOffset publishedSinceAtLeast)
        {
            _logger.LogDebug(new LogItem(10, "GetNewsForNotificationAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null."
            });

            try
            {
                if (tenant == null)
                    throw new ArgumentNullException(nameof(tenant));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);

                var result = await context.News
                                          .AsNoTracking()
                                          .Include(n => n.Creator)
                                          .Include(n => n.LastEditor)
                                          .Include(n => n.TitleTranslation)
                                            .ThenInclude(tt => tt.TranslatedTexts)
                                          .Include(n => n.InfoscreensNews)
                                            .ThenInclude(infn => infn.Infoscreen)
                                                .ThenInclude(i => i.Subscriptions)
                                                    .ThenInclude(s => s.User)
                                          .Include(n => n.InfoscreensNews)
                                            .ThenInclude(infn => infn.Infoscreen)
                                                .ThenInclude(i => i.Subscriptions)
                                                    .ThenInclude(s => s.PushToken)
                                          .AsSplitQuery()
                                          .Where(n => n.DeletionDate.HasValue == false)
                                          .Where(n => n.TenantId == tenant.Id)
                                          .Where(n => n.IsVisible)
                                          .Where(n => n.IsForApp)
                                          .Where(n => n.PublicationDate >= publishedSinceAtLeast)
                                          .Where(n => n.PublicationDate <= DateTimeOffset.UtcNow)
                                          .Where(n => n.UsersNotified == null)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetNewsForNotificationAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = $"Returning list of {result.Count} news."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetNewsForNotificationAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<News> GetNewsFromTenantAsync(Tenant tenant, int newsId)
        {
            _logger.LogDebug(new LogItem(10, "GetNewsFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                Custom2 = newsId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.News
                                          .IncludeOptimized(n => n.TitleTranslation)
                                          .IncludeOptimized(n => n.ContentMarkdownTranslation)
                                          .IncludeOptimized(n => n.ContentHTMLTranslation)
                                          .IncludeOptimized(n => n.File)
                                          .IncludeOptimized(n => n.NewsCategories)
                                            .IncludeOptimized(n => n.NewsCategories.Select(nc => nc.Category))
                                          .FirstOrDefaultAsync(t => t.Id == newsId && t.TenantId == tenant.Id);

                _logger.LogDebug(new LogItem(11, "GetNewsFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = newsId.ToString(),
                    Custom3 = result != null ? result.ToString() : "No news found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetNewsFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<News>> GetNewsFromTenantAsync(Tenant tenant, List<int> newsId)
        {
            _logger.LogDebug(new LogItem(10, "GetNewsFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                Custom2 = $"News Ids : {string.Join(",", newsId)}"
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.News
                                          .IncludeOptimized(n => n.InfoscreensNews)
                                          .IncludeOptimized(n => n.NewsCategories)
                                          .Where(n => n.TenantId == tenant.Id && newsId.Contains(n.Id))
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetNewsFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = $"News Ids : {string.Join(",", newsId)}",
                    Custom3 = result != null ? $"Returning {result.Count} News" : "No news found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetNewsFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<News>> GetPublishedNewsForInfoscreenAsync(int infoscreenId, DateTimeOffset? before = null, DateTimeOffset? after = null, int amount = 0, bool mustBeAssignedToInfoscreens = false, bool mustBeAssignedToApp = false)
        {
            _logger.LogDebug(new LogItem(10, "GetPublishedNewsForInfoscreenAsync() called.")
            {
                Custom1 = infoscreenId.ToString(),
                Custom2 = before.HasValue ? $"Before : {before.Value}" : "No maximum date provided",
                Custom3 = after.HasValue ? $"After : {after.Value}" : "No minimum date provided",
                Custom4 = $"Amount: {amount}"
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);

                // Basic filtering and includes
                var query = context.News
                                   .IncludeOptimized(n => n.InfoscreensNews)
                                   .IncludeOptimized(n => n.NewsCategories)
                                   .Where(n => !n.DeletionDate.HasValue 
                                               && n.InfoscreensNews.Any(i => i.InfoscreenId == infoscreenId)
                                               && (n.IsVisible && n.PublicationDate <= DateTimeOffset.UtcNow)
                                               && (!n.ExpirationDate.HasValue || n.ExpirationDate >= DateTimeOffset.UtcNow)
                                   );


                // Filter by targets
                if(mustBeAssignedToInfoscreens)
                    query = query.Where(n => n.IsForInfoscreens);
                if(mustBeAssignedToApp)
                    query = query.Where(n => n.IsForApp);


                // Filter by given time span
                if (before.HasValue)
                    query = query.Where(n => n.PublicationDate < before);

                if (after.HasValue)
                    query = query.Where(n => n.PublicationDate > after);


                // Sort results
                query = query.OrderByDescending(n => n.PublicationDate);


                // Limit amout of results
                if (amount > 0)
                    query = query.Take(amount);


                var result = await query.ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetPublishedNewsForInfoscreenAsync() finished.")
                {
                    Custom1 = infoscreenId.ToString(),
                    Custom2 = before.HasValue ? $"Before : {before.Value}" : "No maximum date provided",
                    Custom3 = after.HasValue ? $"After : {after.Value}" : "No minimum date provided",
                    Custom4 = result != null ? $"Returning {result.Count} News" : "No news found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetPublishedNewsForInfoscreenAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<News> CreateOrUpdateNewsAsync(News news, IEnumerable<TranslatedText> translatedTextsToDelete, IEnumerable<InfoscreenNews> infoscreensNewsToCreate, IEnumerable<InfoscreenNews> infoscreensNewsToDelete, IEnumerable<NewsCategory> newsCategoriesToDelete)
        {
            _logger.LogDebug(new LogItem(10, "CreateOrUpdateNewsAsync() called.")
            {
                Custom1 = news != null ? news.ToString() : "Parameter news is null.",
                Custom2 = translatedTextsToDelete != null ? $"{translatedTextsToDelete.Count()} translatedText to delete" : "Parameter translatedTextsToDelete is null.",
                Custom3 = infoscreensNewsToCreate != null ? $"{infoscreensNewsToCreate.Count()} infoscreenNews link to create" : "Parameter infoscreensNewsToCreate is null.",
                Custom4 = infoscreensNewsToDelete != null ? $"{infoscreensNewsToDelete.Count()} infoscreenNews link to delete" : "Parameter infoscreensNewsToDelete is null."
            });

            try
            {
                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);

                if (news.Id == 0)
                {
                    context.News.Add(news);
                }
                else
                {
                    context.News.Update(news);
                }

                if (translatedTextsToDelete != null && translatedTextsToDelete.Any())
                {
                    context.TranslatedTexts.RemoveRange(translatedTextsToDelete.Select(tt => tt.RemoveIncludes()));
                }

                if (infoscreensNewsToCreate != null && infoscreensNewsToCreate.Any())
                {
                    context.InfoscreensNews.AddRange(infoscreensNewsToCreate);
                }

                if (infoscreensNewsToDelete != null && infoscreensNewsToDelete.Any())
                {
                    context.InfoscreensNews.RemoveRange(infoscreensNewsToDelete);
                }

                if(newsCategoriesToDelete != null && newsCategoriesToDelete.Any())
                {
                    context.NewsCategories.RemoveRange(newsCategoriesToDelete);
                }

                await context.SaveChangesAsync();


                _logger.LogDebug(new LogItem(11, "CreateOrUpdateNewsAsync() finished.")
                {
                    Custom1 = news != null ? news.ToString() : "Parameter news is null.",
                    Custom2 = translatedTextsToDelete != null ? $"{translatedTextsToDelete.Count()} translatedText to delete" : "Parameter translatedTextsToDelete is null.",
                    Custom3 = infoscreensNewsToCreate != null ? $"{infoscreensNewsToCreate.Count()} infoscreenNews link to create" : "Parameter infoscreensNewsToCreate is null.",
                    Custom4 = infoscreensNewsToDelete != null ? $"{infoscreensNewsToDelete.Count()} infoscreenNews link to delete" : "Parameter infoscreensNewsToDelete is null."
                });

                return news;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "CreateOrUpdateNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<IEnumerable<News>> UpdateNewsAsync(IEnumerable<News> news)
        {
            _logger.LogDebug(new LogItem(10, "UpdateNewsAsync() called.")
            {
                Custom1 = news != null ? news.ToString() : "Parameter news is null."
            });

            try
            {
                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);

                context.News.UpdateRange(news);
                await context.SaveChangesAsync();

                _logger.LogDebug(new LogItem(11, "UpdateNewsAsync() finished.")
                {
                    Custom1 = news != null ? news.ToString() : "Parameter news is null."
                });

                return news;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "UpdateNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task DeleteNewsAsync(News news)
        {
            _logger.LogDebug(new LogItem(10, "DeleteNewsAsync() called.")
            {
                Custom1 = news != null ? news.ToString() : "Parameter news is null."
            });

            try
            {
                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                var infoscreenNewsToDelete = await GetInfoscreensNewsForNewsAsync(news.Id);

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                context.InfoscreensNews.RemoveRange(infoscreenNewsToDelete);
                context.News.Remove(news);

                await context.SaveChangesAsync();


                _logger.LogDebug(new LogItem(11, "DeleteNewsAsync() finished.")
                {
                    Custom1 = news != null ? news.ToString() : "Parameter news is null."
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
            _logger.LogDebug(new LogItem(10, "DeleteNewsAsync() called.")
            {
                Custom1 = news != null ? $"Deleting news #{string.Join(",",news.Select(n => n.Id))}" : "Parameter news is null."
            });

            try
            {
                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                context.News.RemoveRange(news);

                await context.SaveChangesAsync();


                _logger.LogDebug(new LogItem(11, "DeleteNewsAsync() finished.")
                {
                    Custom1 = news != null ? $"Deleted {news.Count} news" : "Parameter news is null."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }



        #endregion News

        #region NewsCategories

        public async Task<List<NewsCategory>> GetNewsCategoriesOfNewsAsync(News news)
        {
            _logger.LogDebug(new LogItem(10, "GetMewsCategoriesOfNewsAsync() called.")
            {
                Custom1 = news != null ? news.ToString() : "Parameter news is null."
            });

            try
            {
                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.NewsCategories.Where(nc => nc.NewsId == news.Id).ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetMewsCategoriesOfNewsAsync() finished.")
                {
                    Custom1 = $"Returning {result.Count} NewsCategories"
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetMewsCategoriesOfNewsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<NewsCategory> GetNewsCategoryAsync(string category, News news)
        {
            _logger.LogDebug(new LogItem(10, "GetNewsCategoryAsync() called.")
            {
                Custom1 = category ?? "Parameter category is null.",
                Custom2 = news != null ? news.ToString() : "Parameter news is null.",
            });

            try
            {
                if (string.IsNullOrWhiteSpace(category))
                    throw new ArgumentNullException(nameof(category));

                if (news == null)
                    throw new ArgumentNullException(nameof(news));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.NewsCategories
                                          .IncludeOptimized(nc => nc.Category)
                                          .Where(nc => nc.NewsId == news.Id && nc.Category.Name == category)
                                          .FirstOrDefaultAsync();

                _logger.LogDebug(new LogItem(11, "GetNewsCategoryAsync() finished.")
                {
                    Custom1 = $"Returning {result}"
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetNewsCategoryAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task DeleteNewsCategoriesAsync(List<NewsCategory> newsCategories)
        {
            _logger.LogDebug(new LogItem(10, "DeleteNewsCategoriesAsync() called.")
            {
                Custom1 = newsCategories != null ? newsCategories.Count.ToString() : "Parameter news is null."
            });

            try
            {
                if (newsCategories == null)
                    throw new ArgumentNullException(nameof(newsCategories));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                context.NewsCategories.RemoveRange(newsCategories);
                await context.SaveChangesAsync();

                _logger.LogDebug(new LogItem(11, "DeleteNewsCategoriesAsync() finished."));
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteNewsCategoriesAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<Category>> GetNewsCategoriesOfInfoscreenAsync(Infoscreen infoscreen)
        {
            _logger.LogDebug(new LogItem(10, "GetNewsCategoriesOfInfoscreenAsync() called.")
            {
                Custom1 = infoscreen != null ? infoscreen.ToString() : "Parameter infoscreen is null."
            });

            try
            {
                if (infoscreen == null)
                    throw new ArgumentNullException(nameof(infoscreen));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.NewsCategories
                                          .IncludeOptimized(nc => nc.Category)
                                          .IncludeOptimized(nc => nc.News)
                                            .IncludeOptimized(nc => nc.News.InfoscreensNews)
                                          .Where(nc => nc.News.InfoscreensNews.Any(i => i.InfoscreenId == infoscreen.Id))
                                          .Select(nc => nc.Category)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetNewsCategoriesOfInfoscreenAsync() finished.")
                {
                    Custom1 = $"Returning {result.Count} categories."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetNewsCategoriesOfInfoscreenAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion

        #region Subscription

        public async Task<List<Subscription>> GetAllSubscriptionsForUserAsync(int userId)
        {
            _logger.LogDebug(new LogItem(10, $"{nameof(GetAllSubscriptionsForUserAsync)}() called.")
            {
                Custom1 = userId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Subscriptions
                                          .IncludeOptimized(s => s.Infoscreen)
                                          .IncludeOptimized(s => s.Infoscreen.InfoscreenGroup)
                                          .Where(s => s.UserId == userId)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, $"{nameof(GetAllSubscriptionsForUserAsync)}() finished.")
                {
                    Custom1 = userId.ToString(),
                    Custom2 = result != null ? result.ToString() : "No subscription found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, $"{nameof(GetAllSubscriptionsForUserAsync)}() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Subscription> CreateOrUpdateSubscriptionAsync(Subscription subscription)
        {
            _logger.LogDebug(new LogItem(10, "CreateOrUpdateSubscriptionAsync() called.")
            {
                Custom1 = subscription != null ? subscription.ToString() : "Parameter subscription is null.",
            });

            try
            {
                if (subscription == null)
                    throw new ArgumentNullException(nameof(subscription));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);

                if (subscription.Id == 0)
                {
                    context.Subscriptions.Add(subscription);
                }
                else
                {
                    context.Subscriptions.Update(subscription);
                }
                await context.SaveChangesAsync();


                _logger.LogDebug(new LogItem(11, "CreateOrUpdateSubscriptionAsync() finished.")
                {
                    Custom1 = subscription != null ? subscription.ToString() : "Parameter subscription is null.",
                });

                return subscription;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "CreateOrUpdateSubscriptionAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task DeleteSubscriptionAsync(Subscription subscription)
        {
            _logger.LogDebug(new LogItem(10, $"{nameof(DeleteSubscriptionAsync)}() called.")
            {
                Custom1 = subscription != null ? subscription.ToString() : "Parameter subscription is null.",
            });

            try
            {
                if (subscription == null)
                    throw new ArgumentNullException(nameof(subscription));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);

                context.Subscriptions.Remove(subscription);
                await context.SaveChangesAsync();


                _logger.LogDebug(new LogItem(11, $"{nameof(DeleteSubscriptionAsync)}() finished.")
                {
                    Custom1 = subscription != null ? subscription.ToString() : "Parameter subscription is null.",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, $"{nameof(DeleteSubscriptionAsync)}() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Subscription

        #region Tenant

        public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
        {
            _logger.LogDebug(new LogItem(10, "GetAllTenants() called."));

            try
            {

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Tenants.ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetAllTenants() finished.")
                {
                    Custom1 = result != null && result.Count > 0 ? $"Returning {result.Count} tenants." : "No tenant found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetAllTenants() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Tenant> GetTenantByCodeAsync(string tenantCode)
        {
            _logger.LogDebug(new LogItem(10, "GetTenantByCodeAsync() called.")
            {
                Custom1 = !string.IsNullOrWhiteSpace(tenantCode) ? tenantCode : "Parameter tenantCode is null or empty."
            });

            try
            {
                if(string.IsNullOrWhiteSpace(tenantCode))
                    throw new ArgumentNullException(nameof(tenantCode));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Tenants
                                          .FirstOrDefaultAsync(t => t.Code.Equals(tenantCode));

                _logger.LogDebug(new LogItem(11, "GetTenantByCodeAsync() finished.")
                {
                    Custom1 = !string.IsNullOrWhiteSpace(tenantCode) ? tenantCode : "Parameter tenantCode is null or empty.",
                    Custom2 = result != null ? result.ToString() : "No tenant found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetTenantByCodeAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Tenant> GetTenantByIdAsync(int tenantId)
        {
            _logger.LogDebug(new LogItem(10, "GetTenantByIdAsync() called.")
            {
                Custom1 = tenantId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Tenants
                                          .FirstOrDefaultAsync(t => t.Id == tenantId);

                _logger.LogDebug(new LogItem(11, "GetTenantByIdAsync() finished.")
                {
                    Custom1 = tenantId.ToString(),
                    Custom2 = result != null ? result.ToString() : "No tenant found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetTenantByIdAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Tenant

        #region TranslatedText

        public async Task<TranslatedText> GetTranslatedTextAsync(Translation translation, Language language)
        {
            _logger.LogDebug(new LogItem(10, "GetTranslatedTextAsync() called.")
            {
                Custom1 = translation != null ? translation.ToString() : "Parameter translation is null.",
                Custom2 = language != null ? language.ToString() : "Parameter language is null."
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.TranslatedTexts
                                          .FirstOrDefaultAsync(tt => tt.TranslationId == translation.Id && tt.LanguageId == language.Id);

                _logger.LogDebug(new LogItem(11, "GetTranslatedTextAsync() finished.")
                {
                    Custom1 = translation != null ? translation.ToString() : "Parameter translation is null.",
                    Custom2 = language != null ? language.ToString() : "Parameter language is null.",
                    Custom3 = result != null ? result.ToString() : "No translatedText found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetTranslatedTextAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<TranslatedText>> GetTranslatedTextsOfTranslationAsync(Translation translation)
        {
            _logger.LogDebug(new LogItem(10, "GetTranslatedTextsOfTranslationAsync() called.")
            {
                Custom1 = translation != null ? translation.ToString() : "Parameter translation is null."
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.TranslatedTexts
                                          .IncludeOptimized(tt => tt.Language)
                                          .Where(tt => tt.TranslationId == translation.Id)
                                          .Distinct()
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetTranslatedTextsOfTranslationAsync() finished.")
                {
                    Custom1 = translation != null ? translation.ToString() : "Parameter translation is null.",
                    Custom2 = $"Returning list of {result.Count} translatedText."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetTranslatedTextsOfTranslationAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task DeleteTranslatedTextsAsync(IEnumerable<TranslatedText> translatedTexts)
        {
            _logger.LogDebug(new LogItem(10, "DeleteTranslatedTextsAsync() called.")
            {
                Custom1 = translatedTexts != null ? $"{translatedTexts.Count()} translatedText to delete" : "Parameter translatedTexts is null."
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);

                context.TranslatedTexts
                       .RemoveRange(translatedTexts);

                await context.SaveChangesAsync();

                _logger.LogDebug(new LogItem(11, "DeleteTranslatedTextsAsync() finished.")
                {
                    Custom1 = translatedTexts != null ? $"{translatedTexts.Count()} translatedText to delete" : "Parameter translatedTexts is null."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteTranslatedTextsAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion TranslatedText

        #region Translation

        public async Task<Translation> GetTranslationtByIdAsync(int translationId)
        {
            _logger.LogDebug(new LogItem(10, "GetTranslationtByIdAsync() called.")
            {
                Custom1 = translationId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Translations
                                          .IncludeOptimized(t => t.TranslatedTexts)
                                          .IncludeOptimized(t => t.TranslatedTexts.Select(tt => tt.Language))
                                          .FirstOrDefaultAsync(t => t.Id == translationId);

                _logger.LogDebug(new LogItem(11, "GetTranslationtByIdAsync() finished.")
                {
                    Custom1 = translationId.ToString(),
                    Custom2 = result != null ? result.ToString() : "No translation found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetTranslationtByIdAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Translation

        #region User

        public async Task<User> GetUserByIdAsync(int userId)
        {
            _logger.LogDebug(new LogItem(10, "GetUserByIdAsync() called.")
            {
                Custom1 = userId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Users
                                          .FirstOrDefaultAsync(u => u.Id == userId);

                _logger.LogDebug(new LogItem(11, "GetUserByIdAsync() finished.")
                {
                    Custom1 = userId.ToString(),
                    Custom2 = result != null ? result.ToString() : "No user found."
                });


                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetUserByIdAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<User> GetUserByObjectIdAsync(string objectId)
        {
            _logger.LogDebug(new LogItem(10, "GetUserByObjectIdAsync() called.")
            {
                Custom1 = !string.IsNullOrWhiteSpace(objectId) ? objectId : "Parameter objectId is null or empty."
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Users
                                          .FirstOrDefaultAsync(u => string.Compare(u.ObjectId, objectId) == 0);

                _logger.LogDebug(new LogItem(11, "GetUserByObjectIdAsync() finished.")
                {
                    Custom1 = !string.IsNullOrWhiteSpace(objectId) ? objectId : "Parameter objectId is null or empty.",
                    Custom2 = result != null ? result.ToString() : "No user found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetUserByObjectIdAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }
        public async Task<User> GetUserByEmailAsync(string email, bool isUsernameMatchEnough = false)
        {
            _logger.LogDebug(new LogItem(10, "GetUserByEmailAsync() called.")
            {
                Custom1 = !string.IsNullOrWhiteSpace(email) ? email : "Parameter email is null or empty."
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                User result;
                if (isUsernameMatchEnough)
                {
                    var username = email.Split('@', StringSplitOptions.None)[0];
                    var users = await context.Users
                                          .Where(u => u.Upn.StartsWith(username))
                                          .ToListAsync();
                    
                    // Ensure only one match and return result
                    users = users.Where(u => string.Compare(u.Upn.Split('@')[0].ToLowerInvariant(), username.ToLowerInvariant()) == 0).ToList();
                    return users.Count > 1 ? null : users.FirstOrDefault();
                }
                else
                {
                    result = await context.Users.FirstOrDefaultAsync(u => string.Compare(u.Upn, email, true) == 0);
                }

                _logger.LogDebug(new LogItem(11, "GetUserByEmailAsync() finished.")
                {
                    Custom1 = !string.IsNullOrWhiteSpace(email) ? email : "Parameter objectId is null or empty.",
                    Custom2 = result != null ? result.ToString() : "No user found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetUserByEmailAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }


        public async Task UpdateUserAsync(User user)
        {
            _logger.LogDebug(new LogItem(10, "UpdateUserAsync() called.")
            {
                Custom1 = user != null ? user.ToString() : "Parameter user is null."
            });

            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                context.Users.Update(user);
                await context.SaveChangesAsync();

                _logger.LogDebug(new LogItem(11, "UpdateUserAsync() finished.")
                {
                    Custom1 = user != null ? user.ToString() : "Updated user is null."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "UpdateUserAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<User> CreateUser_Mobile(apiRegisterUser_Mobile register)
        {
            _logger.LogDebug(new LogItem(10, "CreateUser_Mobile() called.")
            {
                Custom1 = register.ObjectId,
                Custom2 = register.DisplayName,
                Custom3 = register.Upn,
                Custom4 = register.Iso2
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);

                var language = await GetLanguageAsync(register.Iso2);
                if (language == null)
                    register.Iso2 = CommonConfigHelper.DefaultUserIso2;

                var user = new User(register);
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                _logger.LogDebug(new LogItem(11, "CreateUser_Mobile() finished.")
                {
                    Custom1 = user.Id.ToString(),
                    Custom2 = user != null ? user.ToString() : "No user found."
                });

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "CreateUser_Mobile() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion User

        #region UserTenant

        public async Task<UserTenant> GetUserTenantAsync(int userId, int tenantId)
        {
            _logger.LogDebug(new LogItem(10, "GetUserTenantAsync() called.")
            {
                Custom1 = userId.ToString(),
                Custom2 = tenantId.ToString(),
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.UsersTenants
                                          .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TenantId == tenantId);

                _logger.LogDebug(new LogItem(11, "GetUserTenantAsync() finished.")
                {
                    Custom1 = userId.ToString(),
                    Custom2 = tenantId.ToString(),
                    Custom3 = result != null ? result.ToString() : "No userTenant link found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetUserTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<UserTenant>> GetUserTenantsFromUserAsync(int userId)
        {
            _logger.LogDebug(new LogItem(10, "GetUserTenantsFromUserAsync() called.")
            {
                Custom1 = userId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.UsersTenants
                                          .IncludeOptimized(ut => ut.Tenant)
                                          .Where(ut => ut.UserId == userId)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetUserTenantsFromUserAsync() finished.")
                {
                    Custom1 = userId.ToString(),
                    Custom2 = $"Returning list of {result.Count} tenants."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetUserTenantsFromUserAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion UserTenant

        #region Video

        public async Task<IEnumerable<Video>> GetAllVideosFromTenantAsync(Tenant tenant, string search = "", IEnumerable<int> infoscreenIds = null, IEnumerable<int> categoryIds = null)
        {
            _logger.LogDebug(new LogItem(10, "GetAllVideosFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null."
            });

            try
            {
                if (tenant == null)
                    throw new ArgumentNullException(nameof(tenant));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var query = context.Videos
                                          .IncludeOptimized(v => v.TitleTranslation)
                                            .IncludeOptimized(v => v.TitleTranslation.TranslatedTexts)
                                              .IncludeOptimized(v => v.TitleTranslation.TranslatedTexts.Select(t => t.Language))
                                          .IncludeOptimized(v => v.VideoCategories)
                                            .IncludeOptimized(v => v.VideoCategories.Select(vc => vc.Category))
                                          .IncludeOptimized(v => v.Creator)
                                          .IncludeOptimized(v => v.LastEditor)
                                          .IncludeOptimized(v => v.InfoscreensVideos)
                                          .Where(v => v.DeletionDate.HasValue == false)
                                          .Where(v => v.TenantId == tenant.Id);

                // Infoscreen filtering
                if (infoscreenIds != null && infoscreenIds.Any())
                {
                    query = query.Where(v => v.InfoscreensVideos.Any(inv => infoscreenIds.Contains(inv.InfoscreenId)));
                }

                // Category filtering
                if (categoryIds != null && categoryIds.Any())
                {
                    query = query.Where(v => v.VideoCategories.Any(vc => categoryIds.Contains(vc.CategoryId)));
                }

                // Text search
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(v => v.TitleTranslation.TranslatedTexts.Any(tt => tt.Text.Contains(search))
                                  || v.Description.Contains(search)
                    );
                }

                var result = await query.ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetAllVideosFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = $"Returning list of {result.Count} videos."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetAllVideosFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Video> GetVideoFromTenantAsync(Tenant tenant, int videoId)
        {
            _logger.LogDebug(new LogItem(10, "GetVideoFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                Custom2 = videoId.ToString()
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Videos
                                          .IncludeOptimized(v => v.TitleTranslation)
                                          .IncludeOptimized(v => v.VideoCategories)
                                            .IncludeOptimized(v => v.VideoCategories.Select(vc => vc.Category))
                                          .FirstOrDefaultAsync(v => v.Id == videoId && v.TenantId == tenant.Id);

                _logger.LogDebug(new LogItem(11, "GetVideoFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = videoId.ToString(),
                    Custom3 = result != null ? result.ToString() : "No video found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetVideoFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<Video>> GetVideoFromTenantAsync(Tenant tenant, List<int> videoIds)
        {
            _logger.LogDebug(new LogItem(10, "GetVideoFromTenantAsync() called.")
            {
                Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                Custom2 = $"Video Ids : {string.Join(",", videoIds)}"
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.Videos
                                          .IncludeOptimized(v => v.TitleTranslation)
                                          .IncludeOptimized(v => v.InfoscreensVideos)
                                          .IncludeOptimized(v => v.VideoCategories)
                                          .Where(v => v.TenantId == tenant.Id && videoIds.Contains(v.Id))
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetVideoFromTenantAsync() finished.")
                {
                    Custom1 = tenant != null ? tenant.ToString() : "Parameter tenant is null.",
                    Custom2 = $"Video Ids : {string.Join(",", videoIds)}",
                    Custom3 = result != null ? $"Returning {result.Count} videos" : "No video found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetVideoFromTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<Video>> GetPublishedVideosForInfoscreenAsync(int infoscreenId, int amount = 0, bool mustBeAssignedToInfoscreens = false, bool mustBeAssignedToApp = false)
        {
            _logger.LogDebug(new LogItem(10, "GetPublishedVideosForInfoscreenAsync() called.")
            {
                Custom1 = infoscreenId.ToString(),
                Custom2 = $"Amount: {amount}"
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                IQueryable<Video> query = context.Videos
                                                 .IncludeOptimized(v => v.InfoscreensVideos)
                                                 .IncludeOptimized(v => v.VideoCategories)
                                                 .Where(v => !v.DeletionDate.HasValue
                                                             && v.InfoscreensVideos.Any(i => i.InfoscreenId == infoscreenId)
                                                             && (v.IsVisible && v.PublicationDate <= DateTimeOffset.UtcNow)
                                                             && (!v.ExpirationDate.HasValue || v.ExpirationDate >= DateTimeOffset.UtcNow)
                                                 );

                // Filter by targets
                if (mustBeAssignedToInfoscreens)
                    query = query.Where(v => v.IsForInfoscreens);
                if (mustBeAssignedToApp)
                    query = query.Where(v => v.IsForApp);


                // Sort result
                query = query.OrderByDescending(v => v.PublicationDate);


                // Limit amout of results
                if (amount > 0)
                    query = query.Take(amount);

                var result = await query.ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetPublishedVideosForInfoscreenAsync() finished.")
                {
                    Custom1 = infoscreenId.ToString(),
                    Custom2 = $"Amount: {amount}",
                    Custom3 = result != null ? $"Returning {result.Count} Videos" : "No videos found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetPublishedVideosForInfoscreenAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<Video>> GetPublishedVideosForInfoscreensAsync(List<int> infoscreenIds, int amount = 0, bool mustBeAssignedToInfoscreens = false, bool mustBeAssignedToApp = false)
        {
            _logger.LogDebug(new LogItem(10, "GetPublishedVideosForInfoscreenAsync() called.")
            {
                Custom1 = infoscreenIds.ToString(),
                Custom2 = $"Amount: {amount}"
            });

            try
            {
                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                IQueryable<Video> query = context.Videos
                                                 .IncludeOptimized(v => v.InfoscreensVideos)
                                                 .IncludeOptimized(v => v.VideoCategories)
                                                 .Where(v => !v.DeletionDate.HasValue
                                                             && v.InfoscreensVideos.Any(i => infoscreenIds.Contains(i.InfoscreenId))
                                                             && (v.IsVisible && v.PublicationDate <= DateTimeOffset.UtcNow)
                                                             && (!v.ExpirationDate.HasValue || v.ExpirationDate >= DateTimeOffset.UtcNow)
                                                 );

                // Filter by targets
                if (mustBeAssignedToInfoscreens)
                    query = query.Where(v => v.IsForInfoscreens);
                if (mustBeAssignedToApp)
                    query = query.Where(v => v.IsForApp);


                // Sort result
                query = query.OrderByDescending(v => v.PublicationDate);


                // Limit amout of results
                if (amount > 0)
                    query = query.Take(amount);

                var result = await query.ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetPublishedVideosForInfoscreenAsync() finished.")
                {
                    Custom1 = infoscreenIds.ToString(),
                    Custom2 = $"Amount: {amount}",
                    Custom3 = result != null ? $"Returning {result.Count} Videos" : "No videos found."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetPublishedVideosForInfoscreenAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<Video> CreateOrUpdateVideoAsync(Video video, IEnumerable<TranslatedText> translatedTextsToDelete, IEnumerable<InfoscreenVideo> infoscreensVideosToCreate, IEnumerable<InfoscreenVideo> infoscreensVideosToDelete, IEnumerable<VideoCategory> videoCategoriesToDelete)
        {
            _logger.LogDebug(new LogItem(10, "CreateOrUpdateVideoAsync() called.")
            {
                Custom1 = video != null ? video.ToString() : "Parameter video is null.",
                Custom2 = translatedTextsToDelete != null ? $"{translatedTextsToDelete.Count()} translatedText to delete" : "Parameter translatedTextsToDelete is null.",
                Custom3 = infoscreensVideosToCreate != null ? $"{infoscreensVideosToCreate.Count()} infoscreenVideo link to create" : "Parameter infoscreensVideosToCreate is null.",
                Custom4 = infoscreensVideosToDelete != null ? $"{infoscreensVideosToDelete.Count()} infoscreenVideo link to delete" : "Parameter infoscreensVideosToDelete is null."
            });

            try
            {
                if (video == null)
                    throw new ArgumentNullException(nameof(video));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);

                if (video.Id == 0)
                {
                    context.Videos.Add(video);
                }
                else
                {
                    context.Videos.Update(video);
                }

                if (translatedTextsToDelete != null && translatedTextsToDelete.Any())
                {
                    context.TranslatedTexts.RemoveRange(translatedTextsToDelete.Select(tt => tt.RemoveIncludes()));
                }

                if (infoscreensVideosToCreate != null && infoscreensVideosToCreate.Any())
                {
                    context.InfoscreensVideos.AddRange(infoscreensVideosToCreate);
                }

                if (infoscreensVideosToDelete != null && infoscreensVideosToDelete.Any())
                {
                    context.InfoscreensVideos.RemoveRange(infoscreensVideosToDelete);
                }

                if(videoCategoriesToDelete != null && videoCategoriesToDelete.Any())
                {
                    context.VideosCategories.RemoveRange(videoCategoriesToDelete);
                }

                await context.SaveChangesAsync();


                _logger.LogDebug(new LogItem(11, "CreateOrUpdateVideoAsync() finished.")
                {
                    Custom1 = video != null ? video.ToString() : "Parameter video is null.",
                    Custom2 = translatedTextsToDelete != null ? $"{translatedTextsToDelete.Count()} translatedText to delete" : "Parameter translatedTextsToDelete is null.",
                    Custom3 = infoscreensVideosToCreate != null ? $"{infoscreensVideosToCreate.Count()} infoscreenVideo link to create" : "Parameter infoscreensVideosToCreate is null.",
                    Custom4 = infoscreensVideosToDelete != null ? $"{infoscreensVideosToDelete.Count()} infoscreenVideo link to delete" : "Parameter infoscreensVideosToDelete is null."
                });

                return video;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "CreateOrUpdateVideoAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task DeleteVideoAsync(Video video)
        {
            _logger.LogDebug(new LogItem(10, "DeleteVideoAsync() called.")
            {
                Custom1 = video != null ? video.ToString() : "Parameter video is null."
            });

            try
            {
                if (video == null)
                    throw new ArgumentNullException(nameof(video));

                var infoscreenVideosToDelete = await GetInfoscreensVideosForVideoAsync(video.Id);

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                context.InfoscreensVideos.RemoveRange(infoscreenVideosToDelete);
                context.Videos.Remove(video);

                await context.SaveChangesAsync();


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

        public async Task DeleteVideoAsync(List<Video> videos)
        {
            _logger.LogDebug(new LogItem(10, "DeleteVideoAsync() called.")
            {
                Custom1 = videos != null ? $"Deleting videos #{string.Join(",", videos.Select(n => n.Id))}" : "Parameter video is null."
            });

            try
            {
                if (videos == null)
                    throw new ArgumentNullException(nameof(videos));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                context.Videos.RemoveRange(videos);

                await context.SaveChangesAsync();


                _logger.LogDebug(new LogItem(11, "DeleteVideoAsync() finished.")
                {
                    Custom1 = videos != null ? $"Deleted {videos.Count} videos" : "Parameter video is null."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteVideoAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion

        #region VideoCategories

        public async Task<List<VideoCategory>> GetVideoCategoriesOfVideoAsync(Video video)
        {
            _logger.LogDebug(new LogItem(10, "GetVideoCategoriesOfVideoAsync() called.")
            {
                Custom1 = video != null ? video.ToString() : "Parameter video is null."
            });

            try
            {
                if (video == null)
                    throw new ArgumentNullException(nameof(video));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.VideosCategories.Where(vc => vc.VideoId == video.Id).ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetVideoCategoriesOfVideoAsync() finished.")
                {
                    Custom1 = $"Returning {result.Count} VideosCategories"
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetVideoCategoriesOfVideoAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<VideoCategory> GetVideoCategoryAsync(string category, Video video)
        {
            _logger.LogDebug(new LogItem(10, "GetVideoCategoryAsync() called.")
            {
                Custom1 = category ?? "Parameter category is null.",
                Custom2 = video != null ? video.ToString() : "Parameter video is null.",
            });

            try
            {
                if (string.IsNullOrWhiteSpace(category))
                    throw new ArgumentNullException(nameof(category));

                if (video == null)
                    throw new ArgumentNullException(nameof(video));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.VideosCategories
                                          .IncludeOptimized(vc => vc.Category)
                                          .Where(vc => vc.VideoId == video.Id && vc.Category.Name == category)
                                          .FirstOrDefaultAsync();

                _logger.LogDebug(new LogItem(11, "GetVideoCategoryAsync() finished.")
                {
                    Custom1 = $"Returning {result}"
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetVideoCategoryAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task DeleteVideoCategoriesAsync(List<VideoCategory> videoCategories)
        {
            _logger.LogDebug(new LogItem(10, "DeleteVideoCategoriesAsync() called.")
            {
                Custom1 = videoCategories != null ? videoCategories.Count.ToString() : "Parameter videoCategories is null."
            });

            try
            {
                if (videoCategories == null)
                    throw new ArgumentNullException(nameof(videoCategories));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                context.VideosCategories.RemoveRange(videoCategories);
                await context.SaveChangesAsync();

                _logger.LogDebug(new LogItem(11, "DeleteVideoCategoriesAsync() finished."));
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "DeleteVideoCategoriesAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<List<Category>> GetVideoCategoriesOfInfoscreenAsync(Infoscreen infoscreen)
        {
            _logger.LogDebug(new LogItem(10, "GetVideoCategoriesOfInfoscreenAsync() called.")
            {
                Custom1 = infoscreen != null ? infoscreen.ToString() : "Parameter infoscreen is null."
            });

            try
            {
                if (infoscreen == null)
                    throw new ArgumentNullException(nameof(infoscreen));

                using var context = new CMSDbModel(CommonConfigHelper.GetCMSDbContextOptions);
                var result = await context.VideosCategories
                                          .IncludeOptimized(vc => vc.Category)
                                          .IncludeOptimized(vc => vc.Video)
                                            .IncludeOptimized(vc => vc.Video.InfoscreensVideos)
                                          .Where(vc => vc.Video.InfoscreensVideos.Any(i => i.InfoscreenId == infoscreen.Id))
                                          .Select(vc => vc.Category)
                                          .ToListAsync();

                _logger.LogDebug(new LogItem(11, "GetVideoCategoriesOfInfoscreenAsync() finished.")
                {
                    Custom1 = $"Returning {result.Count} categories."
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "GetVideoCategoriesOfInfoscreenAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion
    }
}
