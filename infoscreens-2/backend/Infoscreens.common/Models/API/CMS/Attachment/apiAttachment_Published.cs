using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiAttachment_Published
    {
        [JsonProperty(Required = Required.Always)]
        public string FileName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string FileExtension { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Base64 { get; set; }


        public override string ToString()
        {
            return $"{FileName}.{FileExtension}";
        }
    }
}
