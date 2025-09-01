using Infoscreens.Common.Models.EntityFramework.CMS;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiInfoscreenCategories
    {
        [JsonProperty(Required = Required.Always)]
        public List<apiCategory> NewsCategories { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<apiCategory> VideoCategories { get; set; }

        public apiInfoscreenCategories(List<apiCategory> newsCategories, List<apiCategory> videoCategories)
        {
            NewsCategories = newsCategories;
            VideoCategories = videoCategories;
        }

    }
}
