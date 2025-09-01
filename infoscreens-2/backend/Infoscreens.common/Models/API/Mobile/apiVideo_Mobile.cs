using Newtonsoft.Json;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.API.Mobile
{
    public class apiVideo_Mobile
    {
        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> Title { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Duration { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Url { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string EmbedUrl { get; set; }
    }
}
