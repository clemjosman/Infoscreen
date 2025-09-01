using Infoscreens.Common.Helpers;
using System;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.CachedData
{
    public class TwentyMinChannelCached
    {
        public string Title { get; set; }
        public string LogoSrc { get; set; }
        public string Link { get; set; }
        public string Language { get; set; }
        public ICollection<TwentyMinNewsCached> News { get; set;}


        public TwentyMinChannelCached(string title,
                                      string logoSrc,
                                      string link,
                                      string language,
                                      ICollection<TwentyMinNewsCached> news)
        {
            Title = title;
            LogoSrc = logoSrc;
            Link = link;
            Language = language;
            News = news;
        }
    }

    public class TwentyMinNewsCached
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublicationDate { get; set; }
        public string ImageSrc { get; set; }
        public string Link { get; set; }
        

        public TwentyMinNewsCached(string title,
                                   string description,
                                   DateTimeOffset publicationDate,
                                   string imageSrc,
                                   string link)
        {
            Title = title;
            Description = description;
            PublicationDate = DateHelper.ToIsoDateTimeString(publicationDate);
            ImageSrc = imageSrc;
            Link = link;
        }
    }
}
