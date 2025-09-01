using Infoscreens.Common.Models.API.CMS.News;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiNews_Publish
    {
        [JsonProperty(Required = Required.AllowNull)]
        public int? Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> Title { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> ContentMarkdown { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> ContentHTML { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsVisible { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTimeOffset PublicationDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public DateTimeOffset? ExpirationDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public apiAttachment_Published Attachment { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public int? DeleteAttachment { get; set; }

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

        [JsonProperty(Required = Required.Always)]
        public string Layout { get; set; }

        [JsonProperty(Required = Required.Always)]
        public apiNewsBox Box1 { get; set; }

        [JsonProperty(Required = Required.Always)]
        public apiNewsBox Box2 { get; set; }


        // Methods

        internal bool CheckConsistancy()
        {
            // Test if languages received in all content fields (here: Title and Content) are always defined in all of them
            // This is to avoid situations where one of the field has a translation in a laguage that is not present in another field
            var languagesFromTitle = Title.Keys;
            var languagesFromContentMarkdown = ContentMarkdown.Keys;
            var languagesFromContentHTML = ContentHTML.Keys;
            var allLanguages = languagesFromTitle.Concat(languagesFromContentMarkdown)
                                                 .Concat(languagesFromContentHTML)
                                                 .Distinct();
            bool areAllLanguageInAllContentFields = !allLanguages.Except(languagesFromTitle).Any()
                                                 && !allLanguages.Except(languagesFromContentMarkdown).Any()
                                                 && !allLanguages.Except(languagesFromContentHTML).Any();

            return areAllLanguageInAllContentFields;
        }
    }
}
