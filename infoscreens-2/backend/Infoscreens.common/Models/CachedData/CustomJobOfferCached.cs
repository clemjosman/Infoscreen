using System;
using System.Collections.Generic;
using System.Text;

namespace Infoscreens.Common.Models.CachedData
{
    public class CustomJobOfferCached
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImgSrc { get; set; }
        public List<string> Skills { get; set; }
        public CustomJobOfferContact Contact { get; set; }
    }

    public class CustomJobOfferContact
    {
        public string Company { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
    }
}
