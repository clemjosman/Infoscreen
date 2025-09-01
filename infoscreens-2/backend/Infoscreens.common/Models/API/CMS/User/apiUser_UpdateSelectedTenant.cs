using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiUser_UpdateSelectedTenant
    {
        [JsonProperty(Required = Required.Always)]
        public int NewSelectedTenantId { get; set; }
    }
}
