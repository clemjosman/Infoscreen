using Infoscreens.Common.Interfaces;
using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiUser_Light: IId
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string DisplayName { get; set; }


        public apiUser_Light(int id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }
}
