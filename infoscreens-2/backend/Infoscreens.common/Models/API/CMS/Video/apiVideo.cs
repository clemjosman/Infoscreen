using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiVideo: IId
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> Title { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Url { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string EmbedUrl { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Duration { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public eVideoBackground? Background { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsVisible { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public DateTimeOffset? PublicationDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public DateTimeOffset? ExpirationDate { get; set; }

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
        public List<apiCategory> Categories { get; set; }


        public apiVideo(int id, Dictionary<string, string> title, string url, string embedUrl, int duration, eVideoBackground? background, bool isVisible, DateTimeOffset? publicationDate, DateTimeOffset? expirationDate, DateTimeOffset creationDate, apiUser_Light creator, DateTimeOffset? lastEditDate, apiUser_Light lastEditor, List<int> assignedToInfoscreenIds, bool isForInfoscreens, bool isForApp, string description, List<apiCategory> categories, DateTimeOffset? usersNotified)
        {
            Id = id;
            Title = title;
            Url = url;
            EmbedUrl = embedUrl;
            Duration = duration;
            Background = background;
            IsVisible = isVisible;
            PublicationDate = publicationDate;
            ExpirationDate = expirationDate;
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
        }

    }
}
