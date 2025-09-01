using Infoscreens.Common.Interfaces;
using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiLanguage_Light : IId, ILanguage
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        // ISO 639-1
        [JsonProperty(Required = Required.Always)]
        public string Iso2 { get; set; }


        // Methods

        public bool IsSameLanguage(apiLanguage_Light other)
        {
            return string.Compare(Iso2, other.Iso2, false) == 0;
        }

        public apiLanguage_Light(int id, string iso2)
        {
            Id = id;
            Iso2 = iso2;
        }
    }
}
