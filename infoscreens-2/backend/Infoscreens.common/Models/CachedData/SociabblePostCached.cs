using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.CachedData
{
    public class SociabblePostCached
    {
        public string Guid { get; set; }

        public string Organization {get; set;}

        public string Link { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public DateTimeOffset PublicationDate { get; set; }

        public SociabblePostCached(string guid, string organization, string link, string title, string description, string image, DateTimeOffset publicationDate)
        {
            Guid = guid;
            Organization = organization;
            Link = link;
            Title = title;
            Description = description;
            Image = image;
            PublicationDate = publicationDate;
        }
    }
}
