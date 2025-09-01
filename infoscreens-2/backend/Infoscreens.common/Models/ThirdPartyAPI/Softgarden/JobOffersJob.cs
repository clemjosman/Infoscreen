using Newtonsoft.Json;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.ApiResponse
{
    public class JobOffersJob_Wrapper
    {
        public IEnumerable<JobOffersJob> Results { get; set; }
    }

    public class JobOffersJob
    {
        public string ExternalPostingName { get; set; }

        public string InternalPostingName { get; set; }

        public string JobAdText { get; set; } // HTML of the job ad
        
        public string InternalJobAdText { get; set; } // HTML of the internal job ad

        [JsonProperty("job_ad_url")]
        public string ShowUrl { get; set; }

        public string ApplyOnlineLink { get; set; }

        [JsonProperty("company_id")]
        public string CompanyId { get; set; }

        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonProperty("geo_city")]
        public string City { get; set; }

        [JsonProperty("geo_country")]
        public string Country { get; set; }

        public string Locale { get; set; }

        public JobOffersSalary Salary { get; set; }

        public long JobStartDate { get; set; } // Miliseconds since UnixTime (1970-01-01)
    }

    public class JobOffersSalary
    {
        [JsonProperty("min_value")]
        public double? MinValue { get; set; }

        [JsonProperty("max_value")]
        public double? MaxValue { get; set; }

        public string Currency { get; set; } // EUR, CHF, etc.

        public string Period { get; set; } // month, year, etc.
    }
}
