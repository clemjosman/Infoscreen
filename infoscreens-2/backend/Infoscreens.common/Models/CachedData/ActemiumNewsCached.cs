using Infoscreens.Common.Helpers;
using Newtonsoft.Json;
using System;

namespace Infoscreens.Common.Models.CachedData
{
    public class ActemiumNewsCached
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string PublicationDate { get; set; }

        public string ExpirationDate { get; set; }

        public string FileSrc { get; set; }

        public string FileExtension { get; set; }

        public ActemiumNewsCached(string title, string content, DateTimeOffset publicationDate, DateTimeOffset? expirationDate, string fileSrc, string fileExtension)
        {
            Title = title;
            Content = content;
            PublicationDate = DateHelper.ToIsoDateTimeString(publicationDate);
            ExpirationDate = expirationDate == null ? null : DateHelper.ToIsoDateTimeString(expirationDate);
            FileSrc = fileSrc;
            FileExtension = fileExtension;
        }
    }
}
