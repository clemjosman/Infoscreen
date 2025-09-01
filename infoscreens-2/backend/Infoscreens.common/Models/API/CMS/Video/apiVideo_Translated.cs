using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiVideo_Translated
    {
        [JsonProperty(Required = Required.Always)]
        public apiLanguage_Light Language { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }

        public apiVideo_Translated(apiLanguage_Light language, string title)
        {
            Language = language;
            Title = title;
        }
    }
}
