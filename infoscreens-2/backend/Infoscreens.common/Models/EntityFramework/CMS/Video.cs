using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Helpers.Enumerations;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.API.Mobile;
using Infoscreens.Common.Models.CachedData;
using Infoscreens.Common.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("Videos")]
    public class Video : IId
    {
        // Primary Key

        [Key]
        public int Id { get; set; }


        // Attributes

        [Required]
        public int TenantId { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        public int TitleTranslationId { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public int Duration { get; set; }

        [StringLength(250)]
        public string Background { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        public DateTimeOffset? UsersNotified { get; set; }

        [Required]
        public DateTimeOffset PublicationDate { get; set; }

        public DateTimeOffset? ExpirationDate { get; set; }

        [Required]
        public DateTimeOffset CreationDate { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DateTimeOffset? LastEditDate { get; set; }

        public int? LastEditedBy { get; set; }

        public DateTimeOffset? DeletionDate { get; set; }

        public int? DeletedBy { get; set; }

        [Required]
        public bool IsForInfoscreens { get; set; }

        [Required]
        public bool IsForApp { get; set; }



        // Foreign Keys

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        [ForeignKey("TitleTranslationId")]
        public Translation TitleTranslation { get; set; }

        [ForeignKey("CreatedBy")]
        public User Creator { get; set; }

        [ForeignKey("LastEditedBy")]
        public User LastEditor { get; set; }

        [ForeignKey("DeletedBy")]
        public User DeletedByUser { get; set; }



        // Relations

        [InverseProperty("Video")]
        public ICollection<VideoCategory> VideoCategories { get; set; }

        [InverseProperty("Video")]
        public ICollection<InfoscreenVideo> InfoscreensVideos { get; set; }



        // Constructors
        public Video() { } // Needed by EntityFramework

        public Video(apiVideo_Publish publishedVideo, Tenant tenant, Translation titleTranslation, List<Category> categories, User creator)
        {
            if (publishedVideo == null)
                throw new ArgumentNullException(nameof(publishedVideo));
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            TitleTranslation = titleTranslation ?? throw new ArgumentNullException(nameof(titleTranslation));

            VideoCategories = categories?.Select(c => { return new VideoCategory(this, c); })?.ToList() ?? throw new ArgumentNullException(nameof(categories));

            TenantId = tenant.Id;
            Url = publishedVideo.Url;
            Duration = publishedVideo.Duration;
            Background = EnumMemberParamHelper.GetEnumMemberAttrValue(publishedVideo.Background);
            IsVisible = publishedVideo.IsVisible;
            PublicationDate = publishedVideo.PublicationDate;
            ExpirationDate = publishedVideo.ExpirationDate;
            CreationDate = DateTimeOffset.UtcNow;
            CreatedBy = creator.Id;
            Description = publishedVideo.Description;
            IsForInfoscreens = publishedVideo.IsForInfoscreens;
            IsForApp = publishedVideo.IsForApp;
        }



        // Methods

        public override string ToString()
        {
            return $"Video #{Id}: TitleTranslationId: #{TitleTranslationId} / Url: {Url} / Duration: {Duration}";
        }

        public Video UpdateVideo(apiVideo_Publish publishedVideo, Translation titleTranslation, List<VideoCategory> videoCategories, User editor)
        {
            if (publishedVideo == null)
                throw new ArgumentNullException(nameof(publishedVideo));

            TitleTranslation = titleTranslation ?? throw new ArgumentNullException(nameof(titleTranslation));
            LastEditor = editor ?? throw new ArgumentNullException(nameof(editor));

            VideoCategories = videoCategories ?? throw new ArgumentNullException(nameof(videoCategories));

            var background = publishedVideo.Background != null ? EnumMemberParamHelper.GetEnumMemberAttrValue(publishedVideo.Background) : null;

            Url = publishedVideo.Url;
            Duration = publishedVideo.Duration;
            Background = background;
            IsVisible = publishedVideo.IsVisible;
            PublicationDate = publishedVideo.PublicationDate;
            ExpirationDate= publishedVideo.ExpirationDate;
            LastEditDate = DateTimeOffset.UtcNow;
            Description = publishedVideo.Description;
            IsForInfoscreens = publishedVideo.IsForInfoscreens;
            IsForApp = publishedVideo.IsForApp;

            return this;
        }

        public async Task<apiVideo> ToApiVideoAsync(IDatabaseRepository _databaseRepository)
        {
            var embedUrl = UrlHelper.GenerateYoutubeEmbedUrlFromYoutubeWebsiteUrl(Url);
            var background = EnumMemberParamHelper.ToEnum<eVideoBackground?>(Background);

            // Translations
            Dictionary<string, string> apiTitleTranslatedTexts = await GetApiTitleTranslatedTextsAsync(_databaseRepository);

            // Creator and Editor
            apiUser_Light apiCreator;
            Creator ??= await _databaseRepository.GetUserByIdAsync(CreatedBy);
            apiCreator = Creator.ToApiUser_Light();

            apiUser_Light apiLastEditor = null;
            if (LastEditedBy.HasValue)
            {
                LastEditor ??= await _databaseRepository.GetUserByIdAsync(LastEditedBy.Value);
                apiLastEditor = LastEditor.ToApiUser_Light();
            }

            // Infoscreens
            List<int> assignedToInfoscreenIds;
            InfoscreensVideos ??= await _databaseRepository.GetInfoscreensVideosForVideoAsync(Id);
            assignedToInfoscreenIds = InfoscreensVideos.Select(e => e.InfoscreenId).ToList();


            // Categories
            var apiCategories = VideoCategories.Select(nc => nc.Category.ToApiCategory()).ToList();


            return new apiVideo(Id, apiTitleTranslatedTexts, Url, embedUrl, Duration, background, IsVisible, PublicationDate, ExpirationDate, CreationDate, apiCreator, LastEditDate, apiLastEditor, assignedToInfoscreenIds, IsForInfoscreens, IsForApp, Description, apiCategories, UsersNotified);
        }

        public async Task<apiVideo_Mobile> ToApiVideo_MobileAsync(IDatabaseRepository _databaseRepository)
        {
            Dictionary<string, string> apiTitleTranslatedTexts = await GetApiTitleTranslatedTextsAsync(_databaseRepository);

            (_, string embedUrl) = DataManipulationRepository.GetYoutubeUrls(Url);

            return new apiVideo_Mobile()
            {
                Title = apiTitleTranslatedTexts,
                Duration = Duration,
                Url = Url,
                EmbedUrl = embedUrl
            };
        }

        public async Task<YoutubeVideoCached> ToYoutubeVideoCachedAsync(IDatabaseRepository _databaseRepository)
        {
            Dictionary<string, string> apiTitleTranslatedTexts = await GetApiTitleTranslatedTextsAsync(_databaseRepository);
            var embedUrl = UrlHelper.GenerateYoutubeEmbedUrlFromYoutubeWebsiteUrl(Url);

            return new YoutubeVideoCached(Url, embedUrl, Duration, EnumMemberParamHelper.ToEnum<eVideoBackground?>(Background), apiTitleTranslatedTexts);
        }

        private async Task<Dictionary<string, string>> GetApiTitleTranslatedTextsAsync(IDatabaseRepository _databaseRepository)
        {
            TitleTranslation ??= await _databaseRepository.GetTranslationtByIdAsync(TitleTranslationId);
            return await TitleTranslation.ToDictionaryAsync(_databaseRepository);
        }
    }
}
