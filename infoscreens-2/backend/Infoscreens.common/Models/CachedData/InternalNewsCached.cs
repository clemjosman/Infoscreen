using Infoscreens.Common.Helpers;
using Infoscreens.Common.Models.API.CMS.News;
using Newtonsoft.Json;
using System;

namespace Infoscreens.Common.Models.CachedData
{
    public class InternalNewsCached : ActemiumNewsCached
    {
        public string Layout { get; set; }

        public apiNewsBox Box1 { get; set; }

        public apiNewsBox Box2 { get; set; }

        public InternalNewsCached(string title, string content, DateTimeOffset publicationDate, DateTimeOffset? expirationDate, string fileSrc, string fileExtension, string layout, apiNewsBox box1, apiNewsBox box2) : base(title, content, publicationDate, expirationDate, fileSrc, fileExtension)
        {
            Layout = layout;
            Box1 = box1;
            Box2 = box2;
        }
    }
}
