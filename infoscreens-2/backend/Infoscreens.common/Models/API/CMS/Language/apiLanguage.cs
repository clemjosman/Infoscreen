using Infoscreens.Common.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiLanguage : apiLanguage_Light, IId, ILanguage
    {
        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, string> DisplayName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string CultureCode { get; set; }


        // Methods

        public bool IsSameLanguage(apiLanguage other)
        {
            return string.Compare(Iso2, other.Iso2, false) == 0;
        }

        public apiLanguage(int id, string iso2, Dictionary<string, string> displayName, string cultureCode) : base(id, iso2)
        {
            DisplayName = displayName;
            CultureCode = cultureCode;  
        }
    }
}
