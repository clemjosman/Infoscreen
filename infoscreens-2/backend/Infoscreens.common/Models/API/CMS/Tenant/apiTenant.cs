using Infoscreens.Common.Interfaces;
using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiTenant: IId
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Code { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string DisplayName { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string AppName { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string ContentAdminEmail { get; set; }


        public apiTenant(int id, string code, string displayName, string appName, string contentAdminEmail)
        {
            Id = id;
            Code = code;
            DisplayName = displayName;
            AppName = appName;
            ContentAdminEmail = contentAdminEmail;
        }
    }
}
