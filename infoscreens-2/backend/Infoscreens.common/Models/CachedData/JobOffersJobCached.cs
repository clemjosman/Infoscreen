using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Models.ApiResponse;
using System;

namespace Infoscreens.Common.Models.CachedData
{
    public class JobOffersJobCached
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string ShowUrl { get; set; }

        public string ApplyUrl { get; set; }

        public string City { get; set; }

        public string CountryCode { get; set; }

        public string CompanyName { get; set; }

        public DateTimeOffset PublishedAt { get; set; }

        public eSlideshowLanguage? Language { get; set; }


        public JobOffersJobCached() { }
        public JobOffersJobCached(JobOffersJob job)
        {
            eSlideshowLanguage? language = job.Locale switch
            {
                "de" => eSlideshowLanguage.DE_CH,
                "fr" => eSlideshowLanguage.FR_CH,
                "it" => eSlideshowLanguage.IT_CH,
                "en" => eSlideshowLanguage.EN_GB,
                _ => null
            };
            var defaultLanguage = eSlideshowLanguage.EN_GB;


            Title = job.ExternalPostingName;
            Content = job.JobAdText;
            ShowUrl = job.ShowUrl;
            ApplyUrl = job.ApplyOnlineLink;
            CompanyName = job.CompanyName;
            City = job.City;
            CountryCode = job.Country;
            PublishedAt = DateTimeOffset.FromUnixTimeMilliseconds(job.JobStartDate);
            Language = language;
        }
    }
}
