using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiInfoscreen_MetaDataUpdate
    {
        [JsonProperty(Required = Required.Always)]
        public string DisplayName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int DefaultLanguageId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool SendMailNoContent { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string ContentAdminEmail { get; set; }
    }
}
