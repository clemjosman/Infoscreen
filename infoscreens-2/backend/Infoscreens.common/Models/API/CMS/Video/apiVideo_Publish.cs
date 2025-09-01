using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiVideo_Publish
    {
        [JsonProperty(Required = Required.AllowNull)]
        public int? Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> Title { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Url { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Duration{ get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public eVideoBackground? Background { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsVisible { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTimeOffset PublicationDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public DateTimeOffset? ExpirationDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public List<int> AssignedToInfoscreenIds { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsForInfoscreens { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsForApp { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string Description { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public List<string> Categories { get; set; }


        // Methods

        internal bool CheckConsistancy()
        {
            var isUrlValid = UrlHelper.IsYoutubeWebsiteUrl(Url);

            return isUrlValid;
        }
    }
}
