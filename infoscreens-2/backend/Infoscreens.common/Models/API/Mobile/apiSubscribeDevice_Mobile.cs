using Newtonsoft.Json;
using System.Collections.Generic;
using vesact.common.message.v2.Models;

namespace Infoscreens.Common.Models.API.Mobile
{
    public class apiSubscribeDevice_Mobile : RegisterDevice
    {

        [JsonProperty(Required = Required.Always)]
        public List<int> InfoscreenIds { get; set; }

        // Needed by the Json deserializer
        public apiSubscribeDevice_Mobile() : base(null, null)
        { }

        public apiSubscribeDevice_Mobile(RegisterDevice register): base(register.TokenId, register.Token)
        { }
    }
}
