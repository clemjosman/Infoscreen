using Infoscreens.Common.Interfaces;
using Newtonsoft.Json;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiInfoscreenGroup : IId
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }


        public apiInfoscreenGroup(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
