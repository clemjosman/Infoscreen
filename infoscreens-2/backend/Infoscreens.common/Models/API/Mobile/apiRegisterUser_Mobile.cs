using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.Mobile
{
    public class apiRegisterUser_Mobile
    {
        [JsonProperty(Required = Required.Always)]
        public string ObjectId { get; set; }


        [JsonProperty(Required = Required.Always)]
        public string DisplayName { get; set; }


        [JsonProperty(Required = Required.Always)]
        public string Upn { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Iso2 { get; set; }
    }
}
