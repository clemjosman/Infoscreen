using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiNews_Translated
    {
        [JsonProperty(Required = Required.Always)]
        public apiLanguage_Light Language { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Content { get; set; }

        public apiNews_Translated(apiLanguage_Light language, string title, string content)
        {
            Language = language;
            Title = title;
            Content = content;
        }
    }
}
