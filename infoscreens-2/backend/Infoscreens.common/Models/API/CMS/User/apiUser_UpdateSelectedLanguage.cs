using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiUser_UpdateSelectedLanguage
    {
        [JsonProperty(Required = Required.Always)]
        public string NewSelectedIso2 { get; set; }
    }
}
