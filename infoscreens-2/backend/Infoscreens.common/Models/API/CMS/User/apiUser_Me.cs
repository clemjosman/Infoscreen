using Infoscreens.Common.Comparers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiUser_Me : apiUser_Light
    {
        [JsonProperty(Required = Required.Always)]
        public string ObjectId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Upn { get; set; }


        [JsonProperty(Required = Required.Always)]
        public string Iso2 { get; set; }


        [JsonProperty(Required = Required.Always)]
        public IEnumerable<apiTenant> Tenants { get; set; }


        [JsonProperty(Required = Required.AllowNull)]
        public apiTenant SelectedTenant { get; set; }


        public apiUser_Me(int id, string displayName, string objectId, string upn, string iso2, IEnumerable<apiTenant> tenants, apiTenant selectedTenant) : base (id, displayName)
        {
            ObjectId = objectId;
            Upn = upn;
            Iso2 = iso2;
            Tenants = tenants;
            SelectedTenant = selectedTenant;
        }

        internal bool CheckConsistancy()
        {
            // Validity check for Mobile users not using the CMS
            if (SelectedTenant == null && !Tenants.Any())
                return true;
                
            // Validity check for the CMS users
            var isSelectedTenantInTheListOfTenants = Tenants.Any(t => new IdComparer<apiTenant>().Equals(t, SelectedTenant));
            
            return isSelectedTenantInTheListOfTenants;
        }
    }
}
