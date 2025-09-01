using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS.News;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiNews: IId
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> Title { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> ContentMarkdown { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> ContentHTML { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsVisible { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public DateTimeOffset? PublicationDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public DateTimeOffset? ExpirationDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public apiAttachment Attachment { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTimeOffset CreationDate { get; set; }

        [JsonProperty(Required = Required.Always)]
        public apiUser_Light Creator { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public DateTimeOffset? LastEditDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public apiUser_Light LastEditor { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public List<int> AssignedToInfoscreenIds { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsForInfoscreens { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsForApp { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string Description { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public DateTimeOffset? UsersNotified { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Layout { get; set; }

        [JsonProperty(Required = Required.Always)]
        public apiNewsBox Box1 { get; set; }

        [JsonProperty(Required = Required.Always)]
        public apiNewsBox Box2 { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<apiCategory> Categories { get; set; }


        public apiNews(int id, Dictionary<string, string> title, Dictionary<string, string> contentMarkdown, Dictionary<string, string> contentHTML, bool isVisible, DateTimeOffset? publicationDate, DateTimeOffset? expirationDate, apiAttachment attachment, DateTimeOffset creationDate, apiUser_Light creator, DateTimeOffset? lastEditDate, apiUser_Light lastEditor, List<int> assignedToInfoscreenIds, bool isForInfoscreens, bool isForApp, string description, List<apiCategory> categories, DateTimeOffset? usersNotified, string layout, apiNewsBox box1, apiNewsBox box2)
        {
            Id = id;
            Title = title;
            ContentMarkdown = contentMarkdown;
            ContentHTML = contentHTML;
            IsVisible = isVisible;
            PublicationDate = publicationDate;
            ExpirationDate = expirationDate;
            Attachment = attachment;
            CreationDate = creationDate;
            Creator = creator;
            LastEditDate = lastEditDate;
            LastEditor = lastEditor;
            AssignedToInfoscreenIds = assignedToInfoscreenIds;
            IsForInfoscreens = isForInfoscreens;
            IsForApp = isForApp;
            Description = description;
            Categories = categories;
            UsersNotified = usersNotified;
            Layout = layout;
            Box1 = box1;
            Box2 = box2;
        }

    }
}
