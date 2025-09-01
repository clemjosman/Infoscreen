using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiVideo_Translate
    {
        [JsonProperty(Required = Required.Always)]
        public string From { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string To { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }
    }
}
