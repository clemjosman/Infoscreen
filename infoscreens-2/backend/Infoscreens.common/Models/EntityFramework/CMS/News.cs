using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.API.CMS.News;
using Infoscreens.Common.Models.API.Mobile;
using Infoscreens.Common.Models.CachedData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using vesact.common.file.Interfaces;
using vesact.common.file.Models;

namespace Infoscreens.Common.Models.EntityFramework.CMS
{
    [Table("News")]
    public class News : IId
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
        public int ContentMarkdownTranslationId { get; set; }

        [Required]
        public int ContentHTMLTranslationId { get; set; }

        public int? FileId { get; set; }

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

        [Required]
        [StringLength(10)]
        public string Layout { get; set; }

        [Required]
        public string Box1Content { get; set; }

        [Required]
        public string Box2Content { get; set; }

        [Required]
        public int Box1Size { get; set; }

        [Required]
        public int Box2Size { get; set; }


        // Foreign Keys

        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        [ForeignKey("TitleTranslationId")]
        public Translation TitleTranslation { get; set; }

        [ForeignKey("ContentMarkdownTranslationId")]
        public Translation ContentMarkdownTranslation { get; set; }

        [ForeignKey("ContentHTMLTranslationId")]
        public Translation ContentHTMLTranslation { get; set; }

        [ForeignKey("FileId")]
        public File File { get; set; }

        [ForeignKey("CreatedBy")]
        public User Creator { get; set; }

        [ForeignKey("LastEditedBy")]
        public User LastEditor { get; set; }

        [ForeignKey("DeletedBy")]
        public User DeletedByUser { get; set; }



        // Relations

        [InverseProperty("News")]
        public ICollection<NewsCategory> NewsCategories { get; set; }

        [InverseProperty("News")]
        public ICollection<InfoscreenNews> InfoscreensNews { get; set; }


        // Constructors
        public News() { } // Needed by EntityFramework

        public News(apiNews_Publish publishedNews, Tenant tenant, Translation titleTranslation, Translation contentMarkdownTranslation, Translation contentHTMLTranslation, List<Category> categories, User creator, int? fileId = null)
        {
            if (publishedNews == null)
                throw new ArgumentNullException(nameof(publishedNews));
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            TitleTranslation = titleTranslation ?? throw new ArgumentNullException(nameof(titleTranslation));
            ContentMarkdownTranslation = contentMarkdownTranslation ?? throw new ArgumentNullException(nameof(contentMarkdownTranslation));
            ContentHTMLTranslation = contentHTMLTranslation ?? throw new ArgumentNullException(nameof(contentHTMLTranslation));

            NewsCategories = categories?.Select(c => { return new NewsCategory(this, c); })?.ToList() ?? throw new ArgumentNullException(nameof(categories));

            TenantId = tenant.Id;
            IsVisible = publishedNews.IsVisible;
            PublicationDate = publishedNews.PublicationDate;
            ExpirationDate = publishedNews.ExpirationDate;
            CreationDate = DateTimeOffset.UtcNow;
            CreatedBy = creator.Id;
            Description = publishedNews.Description;
            IsForInfoscreens = publishedNews.IsForInfoscreens;
            IsForApp = publishedNews.IsForApp;
            Layout = publishedNews.Layout;
            Box1Content = publishedNews.Box1.Content;
            Box2Content = publishedNews.Box2.Content;
            Box1Size = publishedNews.Box1.Size;
            Box2Size = publishedNews.Box2.Size;

            UpdateFileId(fileId);
        }


        // Methods

        public override string ToString()
        {
            return $"News #{Id}: TitleTranslationId: #{TitleTranslationId} / Tenant: {Tenant}";
        }

        public News UpdateNews(apiNews_Publish publishedNews, Translation titleTranslation, Translation contentMarkdownTranslation, Translation contentHTMLTranslation, List<NewsCategory> newsCategories, User editor, int? fileId = null)
        {
            if (publishedNews == null)
                throw new ArgumentNullException(nameof(publishedNews));

            TitleTranslation = titleTranslation ?? throw new ArgumentNullException(nameof(titleTranslation));
            ContentMarkdownTranslation = contentMarkdownTranslation ?? throw new ArgumentNullException(nameof(contentMarkdownTranslation));
            ContentHTMLTranslation = contentHTMLTranslation ?? throw new ArgumentNullException(nameof(contentHTMLTranslation));

            NewsCategories = newsCategories ?? throw new ArgumentNullException(nameof(newsCategories));

            IsVisible = publishedNews.IsVisible;
            PublicationDate = publishedNews.PublicationDate;
            ExpirationDate = publishedNews.ExpirationDate;
            LastEditDate = DateTimeOffset.UtcNow;
            LastEditor = editor ?? throw new ArgumentNullException(nameof(editor));
            Description = publishedNews.Description;
            IsForInfoscreens = publishedNews.IsForInfoscreens;
            IsForApp = publishedNews.IsForApp;
            Layout = publishedNews.Layout;
            Box1Content = publishedNews.Box1.Content;
            Box2Content = publishedNews.Box2.Content;
            Box1Size = publishedNews.Box1.Size;
            Box2Size = publishedNews.Box2.Size;

            return UpdateFileId(fileId);
        }

        public News UpdateFileId(int? fileId)
        {
            FileId = fileId;

            return this;
        }

        public async Task<apiNews> ToApiNewsAsync(IDatabaseRepository _databaseRepository, IFileHelper _fileHelper, TimeSpan attachmentExpiry)
        {
            // Translations
            Dictionary<string, string> apiTitleTranslatedTexts = await GetApiTitleTranslatedTextsAsync(_databaseRepository);
            Dictionary<string, string> apiContentMarkdownTranslatedTexts = await GetApiContentMarkdownTranslatedTextsAsync(_databaseRepository);
            Dictionary<string, string> apiContentHTMLTranslatedTexts = await GetApiContentHTMLTranslatedTextsAsync(_databaseRepository);
            

            // Attachment
            apiAttachment apiAttachment = await GetApiAttachmentAsync(_fileHelper, attachmentExpiry);
            

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
            var assignedToInfoscreenIds = new List<int>();
            InfoscreensNews ??= await _databaseRepository.GetInfoscreensNewsForNewsAsync(Id);
            assignedToInfoscreenIds = InfoscreensNews.Select(e => e.InfoscreenId).ToList();


            // Categories
            var apiCategories = NewsCategories.Select(nc => nc.Category.ToApiCategory()).ToList();


            // Layout 
            var box1 = new apiNewsBox(Box1Content, Box1Size);
            var box2 = new apiNewsBox(Box2Content, Box2Size);


            // Result
            return new apiNews(Id, apiTitleTranslatedTexts, apiContentMarkdownTranslatedTexts, apiContentHTMLTranslatedTexts, IsVisible, PublicationDate, ExpirationDate, apiAttachment, CreationDate, apiCreator, LastEditDate, apiLastEditor, assignedToInfoscreenIds, IsForInfoscreens, IsForApp ,Description, apiCategories, UsersNotified, Layout, box1, box2);
        }

        public async Task<apiNews_Mobile> ToApiNews_MobileAsync(IDatabaseRepository _databaseRepository, IFileHelper _fileHelper, TimeSpan attachmentExpiry)
        {
            // Translations
            Dictionary<string, string> apiTitleTranslatedTexts = await GetApiTitleTranslatedTextsAsync(_databaseRepository);
            Dictionary<string, string> apiContentHTMLTranslatedTexts = await GetApiContentHTMLTranslatedTextsAsync(_databaseRepository);


            // Attachment
            apiAttachment apiAttachment = await GetApiAttachmentAsync(_fileHelper, attachmentExpiry);

            // SourceScreenList
            List<string> sourceScreenList = await GetSourceScreensOfNewsAsync(_databaseRepository);

            return new apiNews_Mobile()
            {
                Title = apiTitleTranslatedTexts,
                Content = apiContentHTMLTranslatedTexts,
                Date = PublicationDate.ToString(),
                ExpirationDate = ExpirationDate.ToString(),
                Thumbnail = apiAttachment?.Url,
                ThumbnailLarge = apiAttachment?.Url,
                SourceScreenDisplayNameList = sourceScreenList,
            };
        }

        public async Task<InternalNewsCached> ToInternalNewsCachedAsync(IDatabaseRepository _databaseRepository, IFileHelper _fileHelper, TimeSpan attachmentExpiry, Language language)
        {
            // Translations
            Dictionary<string, string> apiTitleTranslatedTexts = await GetApiTitleTranslatedTextsAsync(_databaseRepository);
            Dictionary<string, string> apiContentHTMLTranslatedTexts = await GetApiContentHTMLTranslatedTextsAsync(_databaseRepository);

            // Attachment
            apiAttachment apiAttachment = await GetApiAttachmentAsync(_fileHelper, attachmentExpiry);
            // Getting right language
            apiTitleTranslatedTexts.TryGetValue(language.ToSlideshowLanguageString(), out var title);
            apiContentHTMLTranslatedTexts.TryGetValue(language.ToSlideshowLanguageString(), out var content);

            var layout = Layout;
            var box1 = new apiNewsBox(Box1Content, Box1Size);
            var box2 = new apiNewsBox(Box2Content, Box2Size);

            // Fallback if language not available
            if (string.IsNullOrEmpty(title))
                title = apiTitleTranslatedTexts.Values.FirstOrDefault();
            if (string.IsNullOrEmpty(content))
                content = apiContentHTMLTranslatedTexts.Values.FirstOrDefault();

            return new InternalNewsCached(title, content, PublicationDate, ExpirationDate, apiAttachment?.Url, apiAttachment?.FileExtension, layout, box1, box2);
        }

        private async Task<apiAttachment> GetApiAttachmentAsync(IFileHelper _fileHelper, TimeSpan attachmentExpiry)
        {
            if (FileId.HasValue)
            {
                FileMetadata fileMetadata;
                if (File == null)
                {
                    fileMetadata = await _fileHelper.GetFileMetadataAsync(FileId.Value, attachmentExpiry);
                }
                else
                {
                    fileMetadata = _fileHelper.GetFileMetadata(File, attachmentExpiry);
                }
                return new apiAttachment(fileMetadata);
            }
            return null;
        }

        private async Task<List<string>> GetSourceScreensOfNewsAsync(IDatabaseRepository _databaseRepository)
        {
            var sourceScreenList = await _databaseRepository.GetInfoscreensByNewsIdAsync(Id);

            // We only need the display names
            return sourceScreenList.Select(s => s.DisplayName).ToList();
        }

        private async Task<Dictionary<string, string>> GetApiTitleTranslatedTextsAsync(IDatabaseRepository _databaseRepository)
        {
            TitleTranslation ??= await _databaseRepository.GetTranslationtByIdAsync(TitleTranslationId);
            return await TitleTranslation.ToDictionaryAsync(_databaseRepository);
        }

        private async Task<Dictionary<string, string>> GetApiContentMarkdownTranslatedTextsAsync(IDatabaseRepository _databaseRepository)
        {
            ContentMarkdownTranslation ??= await _databaseRepository.GetTranslationtByIdAsync(ContentMarkdownTranslationId);
            return await ContentMarkdownTranslation.ToDictionaryAsync(_databaseRepository);
        }

        private async Task<Dictionary<string, string>> GetApiContentHTMLTranslatedTextsAsync(IDatabaseRepository _databaseRepository)
        {
            ContentHTMLTranslation ??= await _databaseRepository.GetTranslationtByIdAsync(ContentHTMLTranslationId);
            return await ContentHTMLTranslation.ToDictionaryAsync(_databaseRepository);
        }
    }
}
