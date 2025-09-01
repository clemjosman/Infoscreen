using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Models.Configs;
using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiInfoscreen_ConfigUpdate
    {
        // Frontend related

        [JsonProperty(Required = Required.Always)]
        public string Language { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Timezone { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool DisableAnimations { get; set; }

        [JsonProperty(Required = Required.Always)]
        public eTheme Theme { get; set; }

        [JsonProperty(Required = Required.Always)]
        public eBannerStyle BannerStyle { get; set; }

        [JsonProperty(Required = Required.Always)]
        public eFooterStyle FooterStyle { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string RollingMessage { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool InvertDuoBranding { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DeviceSleepConfig SleepConfig { get; set; }

        [JsonProperty(Required = Required.Always)]
        public SlidesConfig Slides { get; set; }

        [JsonProperty(Required = Required.Always)]
        public LocalCacheConfig LocalCache { get; set; }

        // Backend related

        [JsonProperty(Required = Required.Always)]
        public DataEndpointConfig DataEndpointConfig { get; set; }
    }
}
