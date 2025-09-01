using Infoscreens.Common.Interfaces;
using Newtonsoft.Json;
using vesact.common.file.Models;

namespace Infoscreens.Common.Models.API.CMS
{
    public class apiAttachment: IId
    {
        [JsonProperty(Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Url { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string FileName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string FileExtension { get; set; }



        public apiAttachment(FileMetadata fileMetaData)
        {
            Id = fileMetaData.FileId;
            Url = fileMetaData.Url;
            FileName = fileMetaData.FileName;
            FileExtension = fileMetaData.Extension;
        }

        public override string ToString()
        {
            return $"{FileName}.{FileExtension}";
        }
    }
}
