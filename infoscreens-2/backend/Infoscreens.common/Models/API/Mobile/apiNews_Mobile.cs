using Newtonsoft.Json;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.API.Mobile
{
    public class apiNews_Mobile
    {
        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> Title { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> Content { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Date { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string ExpirationDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string Thumbnail { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string ThumbnailLarge { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<string> SourceScreenDisplayNameList { get; set; }
    }
}
