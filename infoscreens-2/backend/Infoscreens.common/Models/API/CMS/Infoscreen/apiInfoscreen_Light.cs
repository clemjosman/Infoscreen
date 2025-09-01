using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiInfoscreen_Light: IId
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string NodeId { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string MsbNodeId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string DisplayName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int InfoscreenGroupId { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public bool SendMailNoContent { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string ContentAdminEmail { get; set; }

        [JsonProperty(Required = Required.Always)]
        public apiLanguage Language { get; set; }


        public apiInfoscreen_Light(Infoscreen infoscreen, IDatabaseRepository databaseRepository)
        {
            var languageTask = Task.Run(() => infoscreen.DefaultContentLanguage.ToApiLanguageAsync(databaseRepository));

            Id = infoscreen.Id;
            NodeId = infoscreen.NodeId;
            MsbNodeId = infoscreen.MsbNodeId;
            DisplayName = infoscreen.DisplayName;
            Description = infoscreen.Description;
            InfoscreenGroupId = infoscreen.InfoscreenGroupId;
            SendMailNoContent = infoscreen.SendMailNoContent;
            ContentAdminEmail = infoscreen.ContentAdminEmail;

            languageTask.Wait();
            Language = languageTask.Result;
        }
    }
}
