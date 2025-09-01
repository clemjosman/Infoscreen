using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS.News
{
    public class apiNewsBox
    {
        [JsonProperty(Required = Required.Always)]
        public string Content { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Size { get; set; }

        public apiNewsBox(string content, int size)
        {
            Content = content;
            Size = size;
        }
    }
}
