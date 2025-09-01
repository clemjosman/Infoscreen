using Infoscreens.Common.Helpers;
using System;

namespace Infoscreens.Common.Models.CachedData
{
    public class UniversityCourseCached
    {
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int PlacesLeft { get; set; }
        public string Link { get; set; }
        public string ImageSrc { get; set; }
        public string Price { get; set; }
        public string Organizer { get; set; }
        public UniversityCourseLocationCached Location { get; set; }
        public string Performer { get; set; }

        public UniversityCourseCached(string name,
                                      DateTimeOffset startDate,
                                      DateTimeOffset endDate,
                                      int placesLeft,
                                      string link,
                                      string imageSrc,
                                      string currency,
                                      string price,
                                      string organizer,
                                      UniversityCourseLocationCached location,
                                      string performer)
        {
            Name = name;
            StartDate = DateHelper.ToIsoDateTimeString(startDate);
            EndDate = DateHelper.ToIsoDateTimeString(endDate);
            PlacesLeft = placesLeft;
            Link = link;
            ImageSrc = imageSrc;
            Price = PriceHelper.ToCurrencyPriceString(currency, price);
            Organizer = organizer;
            Location = location;
            Performer = performer;
        }
    }

    public class UniversityCourseLocationCached
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }

        public UniversityCourseLocationCached(string country, string city, string address, string postalCode)
        {
            Country = country;
            City = city;
            Address = address;
            PostalCode = postalCode;
        }
    }
}
