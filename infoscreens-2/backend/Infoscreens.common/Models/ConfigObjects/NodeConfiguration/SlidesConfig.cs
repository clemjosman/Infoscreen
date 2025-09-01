using Infoscreens.Common.Enumerations;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.Configs
{
    public class SlidesConfig
    {

        public IEnumerable<eSlide> Order { get; set; }

        public Dictionary<eSlide, JObject> Config { get; set; }


        public SlidesConfig(IEnumerable<eSlide> order, Dictionary<eSlide, JObject> config)
        {
            Order = order;
            Config = config;

            // This code segment goes with the SlideConfigWrapper concept.
            // Could not make deserializing work yet but would enfore a stronger typing
            //
            //var newConfigs = new SlideConfigWrapper();
            //if (config != null)
            //{
            //    foreach (KeyValuePair<eSlide, JObject> configPair in config)
            //    {
            //        newConfigs[configPair.Key] = configPair.Value;
            //    }
            //}
        }

    }

    
}

#region SlideConfigWrapper concept
/*
public class SlideConfigWrapper
{
    // INFO: Let theses attributes in lower case as they refere to the slide name
    public SimpleSlideConfig coffee { get; set; }
    public SimpleSlideConfig ideabox { get; set; }
    public IframeSlideConfig iframe { get; set; }
    public SimpleSlideConfig infoscreenmonitoring { get; set; }
    public SimpleSlideConfig joboffer { get; set; }
    public LocalVideoSlideConfig localvideo { get; set; }
    public SimpleSlideConfig maintenance { get; set; }
    public IframeSlideConfig monitoringiframe { get; set; }
    public SimpleSlideConfig newsinternal { get; set; }
    public SimpleSlideConfig newspublic { get; set; }
    public SimpleSlideConfig publictransport { get; set; }
    public SpotlightSlideConfig spotlight { get; set; }
    public StockSlideConfig stock { get; set; }
    public TrafficSlideConfig traffic { get; set; }
    public SimpleSlideConfig twentymin { get; set; }
    public SimpleSlideConfig twitter { get; set; }
    public SimpleSlideConfig university { get; set; }
    public UniversityOverviewSlideConfig universityoverview { get; set; }
    public SimpleSlideConfig weatherdaily { get; set; }
    public SimpleSlideConfig weatherweekly { get; set; }
    public YoutubeSlideConfig youtube { get; set; }

    public object this[eSlide slide]
    {
        get { return GetConfigObject(slide); }
        set { SetConfigObject(slide, (JObject)value); }
    }

    public object GetConfigObject(eSlide slide)
    {
        return GetType().GetProperty(slide.ToString()).GetValue(this, null);
    }

    public void SetConfigObject(eSlide slide, JObject value)
    {
        // Get EnumMember value
        var propertyName = typeof(eSlide)
                                .GetTypeInfo()
                                .DeclaredMembers
                                .SingleOrDefault(x => x.Name == slide.ToString())
                                ?.GetCustomAttribute<EnumMemberAttribute>(false)
                                ?.Value;

        if (propertyName != null)
        {
            var property = GetType().GetProperty(propertyName);
            var json = value.ToString(Formatting.None);
            Type type = property.GetType();
            property.SetValue(this, JsonConvert.DeserializeObject(json));
        }
    }
}

#endregion

#region Config interfaces
public class SimpleSlideConfig
{
    public int Duration { get; set; }

    [JsonConstructor]
    public SimpleSlideConfig(int duration)
    {
        Duration = duration;
    }
}

public class IframeSlideConfig
{
    public IEnumerable<IframPages> Pages { get; set; }

    [JsonConstructor]
    public IframeSlideConfig(IEnumerable<IframPages> pages)
    {
        Pages = pages;
    }
}

public class LocalVideoSlideConfig
{
    public IEnumerable<LocalVideos> Videos { get; set; }

    [JsonConstructor]
    public LocalVideoSlideConfig(IEnumerable<LocalVideos> videos)
    {
        Videos = videos;
    }
}

public class SpotlightSlideConfig
{
    public IEnumerable<SpotlightPages> Pages { get; set; }

    [JsonConstructor]
    public SpotlightSlideConfig(IEnumerable<SpotlightPages> pages)
    {
        Pages = pages;
    }
}

public class StockSlideConfig
{
    public int Duration { get; set; }
    public UrlWrapper Exchange { get; set; }
    public UrlWrapper Stock { get; set; }

    [JsonConstructor]
    public StockSlideConfig(int duration, UrlWrapper exchange, UrlWrapper stock)
    {
        Duration = duration;
        Exchange = exchange;
        Stock = stock;
    }
}

public class TrafficSlideConfig
{
    public int Duration { get; set; }
    public Gmap Gmap { get; set; }

    [JsonConstructor]
    public TrafficSlideConfig(int duration, Gmap gmap)
    {
        Duration = duration;
        Gmap = gmap;
    }
}

public class UniversityOverviewSlideConfig
{
    public int Duration { get; set; }
    public UrlWrapper Page { get; set; }

    [JsonConstructor]
    public UniversityOverviewSlideConfig(int duration, UrlWrapper page)
    {
        Duration = duration;
        Page = page;
    }
}

public class YoutubeSlideConfig
{
    public IEnumerable<YoutubeVideos> Videos { get; set; }

    [JsonConstructor]
    public YoutubeSlideConfig(IEnumerable<YoutubeVideos> videos)
    {
        Videos = videos;
    }
}
#endregion

#region Helper Interfaces
public class UrlWrapper
{
    public string Url { get; set; }

    [JsonConstructor]
    public UrlWrapper(string url) { 
        Url = url;
    }
}

public class Gmap {
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public int Zoom { get; set; }
    public string ApiKey { get; set;}

    [JsonConstructor]
    public Gmap(float longitude, float latitude, int zoom, string apiKey) {
        Longitude = longitude;
        Latitude = latitude;
        Zoom = zoom;
        ApiKey = apiKey;
    }
}

public class IframPages {
    public string BannerTitle { get; set; }
    public int Duration { get; set; }
    public string Url { get; set; }

    [JsonConstructor]
    public IframPages(string bannerTitle, int duration, string url) {
        BannerTitle = bannerTitle;
        Duration = duration;
        Url = url;
    }
}

public class LocalVideos {
    public int Duration { get; set; }
    public string File { get; set; }
    public Dictionary<eSlideshowLanguage, string> Title { get; set; }

    [JsonConstructor]
    public LocalVideos(int duration, string file, Dictionary<eSlideshowLanguage, string> title) {
        Duration = duration;
        File = file;
        Title = title;
    }
}

public class SpotlightPages {
    public int Duration { get; set; }
    public string Url { get; set; }

    [JsonConstructor]
    public SpotlightPages(int duration, string url) {
        Duration = duration;
        Url = url;
    }
}

public class YoutubeVideos {
    public int Duration { get; set; }
    public string EmbedUrl { get; set; }
    public Dictionary<eSlideshowLanguage, string> Title { get; set; }
    public string Url { get; set; }

    [JsonConstructor]
    public YoutubeVideos(int duration, string embedUrl, Dictionary<eSlideshowLanguage, string> title, string url) {
        Duration = duration;
        EmbedUrl = embedUrl;
        Title = title;
        Url = url;
    }
}
*/
#endregion
