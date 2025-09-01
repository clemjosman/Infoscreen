using Azure.AI.OpenAI;
using HtmlAgilityPack;
using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Models.ApiResponse;
using Infoscreens.Common.Models.CachedData;
using Infoscreens.Common.Models.ConfigObjects.IoT_Hub_DeviceTwin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Infoscreens.Common.Repositories
{
    public class DataManipulationRepository
    {
        const string JOB_OFFERS_BASE_AI_PROMPT_STRING = "Please summarize the job ad text I am going to give you the following way:\n- In the same language it was written in\n- Use exactly five sentences\n- The output should sound like the job ad focusing on the specific skills needed for the position. Don't use any Markdown syntax, use basic HTML tags to structure your answer. The job ad text is: ";

        public static (string title, string content, string imgSrc, DateTime date) GetDataFromInternalNews(JObject news)
        {
            string imgSrc = null;
            try
            {
                // Getting image source
                if (imgSrc == null && news["_embedded"]?["wp:featuredmedia"]?[0]?["media_details"]?["sizes"]?["full"]?["source_url"] != null)
                {
                    imgSrc = news["_embedded"]["wp:featuredmedia"][0]["media_details"]["sizes"]["full"].Value<string>("source_url");
                }
                if (imgSrc == null && news["_embedded"]?["wp:featuredmedia"]?[0]?["source_url"] != null)
                {
                    imgSrc = news["_embedded"]["wp:featuredmedia"][0].Value<string>("source_url");
                }
                if (imgSrc == null && news["cover_url_large"] != null)
                {
                    imgSrc = news.Value<string>("cover_url_large");
                }
                if (imgSrc == null && news["cover_url"] != null)
                {
                    imgSrc = news.Value<string>("cover_url");
                }

            }
            catch (NullReferenceException)
            {
                imgSrc = null;
            }

            var title = news["title"].Value<string>("rendered");
            var content = news["content"].Value<string>("rendered").Replace("\n<p>", "").Replace("</p>\n", "");
            var date = news.Value<DateTime>("date");

            return (title, content, imgSrc, date);
        }

        public static PublicTransportCached ManipulatePublicTransportData(string data, ApiRequest apiRequest)
        {
            var transportData = JObject.Parse(data);
            var station = new StationCached(
                                transportData["station"].Value<string>("id"),
                                transportData["station"].Value<string>("name")
                            );

            var stationBoard = new List<JourneyCached>();

            foreach (JObject journey in transportData.Value<JArray>("stationboard").Cast<JObject>())
            {
                string transportType = null;
                if (journey.Value<string>("category") == "NFB")
                {
                    transportType = "Bus";
                }
                else if (journey.Value<string>("category") == "NFT")
                {
                    transportType = "Tram";
                }

                var journeyCached = new JourneyCached(
                        journey.Value<string>("number"),
                        transportType,
                        journey.Value<string>("to"),
                        journey["stop"].Value<DateTime>("departure"),
                        journey["stop"].Value<string>("delay")
                    );

                stationBoard.Add(journeyCached);
            }


            var destinationGroups = new List<DestinationGroup>();
            var config = apiRequest.Config;

            if (config != null)
            {
                var configJson = JsonConvert.SerializeObject(config, CommonConfigHelper.JsonCamelCaseSettings);
                var configParsed = (JArray)(JObject.Parse(configJson)["destinationGroups"]);
                destinationGroups = configParsed.ToObject<List<DestinationGroup>>();
            }


            var publicTransport = new PublicTransportCached(station, stationBoard, destinationGroups);
            return publicTransport;
        }

        public static List<UniversityCourseCached> ManipulateActemiumUniversityData(string data)
        {
            var links = Regex.Matches(data, "((href=\"https://app1.edoobox.com/de/vesact/)([^\n\r\"])+\")")
                                     .Select(m => m.Value);

            links = links.Distinct(StringComparer.OrdinalIgnoreCase)
                         .Select(s => s.Replace("href=\"", "")
                                       .TrimEnd('"'));

            object lockList = new();
            var courseList = new List<UniversityCourseCached>();

            Parallel.ForEach(links, link =>
            {
                try
                {
                    var coursePage = new HtmlWeb().Load(link);
                    var courseData = JObject.Parse(coursePage.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']")
                                                                          .InnerText);

                    var imageSrc = coursePage.DocumentNode
                                             .SelectSingleNode("//div[@id='wdg139265219043910547364ff41d2c56b78ce731f71599d086fab348']//img")?
                                             .Attributes["src"]?
                                             .Value;

                    var placesLeft = int.Parse(coursePage.DocumentNode
                                                         .SelectSingleNode("//div[@id='wdg139280676471710547364ff435d756b78ce733ad0599d086fac043']//tbody//tr[1]//td[2]")?
                                                         .InnerText);

                    var courseName = Regex.Match(courseData.Value<string>("name"), @"[^0-9.\-\s]*(?:.*[0-9].[-])?(.*)").Groups.Values.Last().Value.Trim().Replace("  ", " ");
                    var startDate = courseData.Value<DateTime>("startDate");
                    var endDate = courseData.Value<DateTime>("endDate");
                    var currency = courseData["offers"].Value<string>("priceCurrency");
                    var price = courseData["offers"].Value<string>("highPrice");
                    var organizer = courseData["organizer"].Value<string>("name");
                    var performer = courseData["performer"].FirstOrDefault()?.Value<string>("name");
                    var location = new UniversityCourseLocationCached(
                                            courseData["location"]["address"].Value<string>("addressCountry"),
                                            courseData["location"]["address"].Value<string>("addressLocality"),
                                            courseData["location"]["address"].Value<string>("streetAddress"),
                                            courseData["location"]["address"].Value<string>("postalCode")
                                        );


                    var course = new UniversityCourseCached(
                                        courseName,
                                        startDate,
                                        endDate,
                                        placesLeft,
                                        link,
                                        imageSrc,
                                        currency,
                                        price,
                                        organizer,
                                        location,
                                        performer);


                    lock (lockList)
                    {
                        courseList.Add(course);
                    }
                }
                catch (Exception) {
                }
            });

            // Filter too early courses - 6 Months
            courseList = courseList.Where(t => DateTimeOffset.Parse(t.StartDate).CompareTo(DateTimeOffset.UtcNow.AddMonths(6)) <= 0).ToList();

            return courseList;
        }

        public static TwentyMinChannelCached ManipulateTwentyMinData(string data)
        {
            TwentyMinChannelCached channel;
            var cachedNews = new List<TwentyMinNewsCached>();


            XmlDocument doc = new();
            doc.LoadXml(data);
            var channelNode = doc.DocumentElement.SelectSingleNode("channel");

            string channelTitle = channelNode.SelectSingleNode("title")?.InnerText;
            string channelLink = channelNode.SelectSingleNode("link")?.InnerText;
            string channelLanguage = channelNode.SelectSingleNode("language")?.InnerText;
            string channelLogoSource = channelNode.SelectSingleNode("image")?.SelectSingleNode("url")?.InnerText;

            foreach (XmlNode node in channelNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "item":
                        var itemTitle = node.SelectSingleNode("title")?.InnerText;
                        var itemDescription = node.SelectSingleNode("description")?.InnerText;
                        itemDescription = Regex.Replace(itemDescription, "<.*?>", string.Empty).Replace("&nbsp;", " ");
                        var itemPublicationDate = DateTimeOffset.Parse(node.SelectSingleNode("pubDate")?.InnerText);
                        var itemImageSrc = node.SelectSingleNode("enclosure")?.Attributes["url"]?.Value;
                        var itemLink = node.SelectSingleNode("link")?.InnerText;
                        cachedNews.Add(new TwentyMinNewsCached(itemTitle, itemDescription, itemPublicationDate, itemImageSrc, itemLink));
                        break;

                    default:
                        break;
                }
            }

            if (channelLogoSource == null
                || channelTitle == null
                || channelLink == null
                || channelLanguage == null)
            {
                throw new FormatException("The data retrieved from the Twenty Min API are missing some channel informations!");
            }

            channel = new TwentyMinChannelCached(channelTitle, channelLogoSource, channelLink, channelLanguage, cachedNews);
            return channel;
        }

        public static (string videoUrl, string embedVideoUrl, int duration, Dictionary<string, string> title) GetDataFromVideo(JObject video)
        { 
            (string videoUrl, string embedVideoUrl) = GetYoutubeUrls(video.Value<string>("url"));

            var duration = int.Parse(video.Value<string>("duration"));

            var title = new Dictionary<string, string>()
                        {
                            {"de", video.Value<string>("titlede")},
                            {"fr", video.Value<string>("titlefr")}
                        };

            return (videoUrl, embedVideoUrl, duration, title);
        }

        public static (string youtubeUrl, string embedYoutubeUrl) GetYoutubeUrls(string url)
        {
            var videoUrl = url;
            var embedVideoUrl = "";

            // Handling different type of youtube url
            if (!videoUrl.StartsWith("https://www.youtube.com/embed/"))
            {
                var videoId = "";
                if (videoUrl.StartsWith("https://youtu.be/"))
                    videoId = videoUrl[17..];
                else if (videoUrl.StartsWith("https://www.youtube.com/watch?v="))
                    videoId = videoUrl[32..];
                if (!string.IsNullOrWhiteSpace(videoId))
                    embedVideoUrl = "https://www.youtube.com/embed/" + videoId;
            }

            // Removing query parameters from embed url
            var indexQuery = embedVideoUrl.IndexOf('?');
            if (indexQuery > 0)
            {
                embedVideoUrl = embedVideoUrl[..indexQuery];
            }
            // Handling special case where link starts with https://www.youtube.com/watch?v= and has other parameter after the videoId fro the embed url
            indexQuery = embedVideoUrl.IndexOf('&');
            if (indexQuery > 0)
            {
                embedVideoUrl = embedVideoUrl[..indexQuery];
            }

            return (videoUrl, embedVideoUrl);
        }

        public static List<TweetCached> ManipulateTwitterData(string data)
        {
            JObject tweetResponse;
            JArray tweetsRaw;

            // Parsing data following 2 types of models (search and statuses)
            try
            {
                tweetResponse = JObject.Parse(data);
                tweetsRaw = tweetResponse.Value<JArray>("statuses");
            }
            catch (JsonReaderException)
            {
                tweetsRaw = JArray.Parse(data);
            }

            var tweets = new List<TweetCached>();
            foreach (JObject tweetRaw in tweetsRaw.Cast<JObject>())
            {
                var hashtags = new List<string>();
                if (tweetRaw["entities"] != null && tweetRaw["entities"]["media"] != null && tweetRaw["entities"].Value<JArray>("hashtags").Count > 0)
                {
                    foreach (JObject hashtag in tweetRaw["entities"].Value<JArray>("hashtags").Cast<JObject>())
                    {
                        hashtags.Add(hashtag.Value<string>("text"));
                    }
                }

                var imgs = new List<string>();
                if (tweetRaw["extended_entities"] != null && tweetRaw["extended_entities"]["media"] != null && tweetRaw["extended_entities"].Value<JArray>("media").Count > 0)
                {
                    foreach (JObject mediaRaw in tweetRaw["extended_entities"].Value<JArray>("media").Cast<JObject>())
                    {
                        if (string.Compare(mediaRaw.Value<string>("type"), "photo", true) == 0)
                            imgs.Add(mediaRaw.Value<string>("media_url"));
                    }
                }

                tweets.Add(new TweetCached(
                        tweetRaw.Value<ulong>("id"),
                        tweetRaw.Value<string>("full_text"),
                        DateTimeOffset.ParseExact(tweetRaw.Value<string>("created_at"), DateHelper.GetTwitterDateTemplate(), new System.Globalization.CultureInfo("en-US")),
                        hashtags,
                        imgs,
                        tweetRaw["user"].Value<string>("name"),
                        tweetRaw["user"].Value<string>("screen_name"),
                        tweetRaw["user"].Value<string>("profile_image_url"),
                        tweetRaw.Value<int>("retweet_count"),
                        tweetRaw.Value<int>("favorite_count")
                    )
                );
            }

            // Filter too old tweets - 6 Months
            var recentTweets = tweets.Where(t => DateTimeOffset.Parse(t.PublishDate).CompareTo(DateTimeOffset.UtcNow.AddMonths(-6)) >= 0).ToList();
            if (recentTweets.Count >= 6)
                tweets = recentTweets;

            return tweets;
        }

        public async static Task<List<InfoscreenNodeStatusCached>> ManipulateMSBDataAndGetConnectionDataAsync(string data)
        {
            var nodesList = JArray.Parse(data);
            var cachedNodesList = new List<InfoscreenNodeStatusCached>();

            foreach (JObject node in nodesList.Cast<JObject>())
            {
                string nodeId = node.Value<string>("id");
                string nodeName = node.Value<string>("name");

                // Not checking for legacy nodes as they're not on the same MSB tenant
                if (!nodeName.StartsWith("INF-"))
                {
                    continue;
                }

                // There is an issue with the JObject library that automatically convert string dates into format DD/MM/YYYY and makes the conversion to DateTimeOffset fail and loss of timezone data.
                // The maintainer of the library, on a thread from 2018, said that it will not be changed, so we use this solution to get datetimeOffset.
                JObject nodeConnectionState;
                DateTimeOffset nodeConnectionStateUpdatedTime;
                DateTimeOffset nodeStatusUpdatedTime;
                DateTimeOffset nodeLastActivityTime;
                (var response, _) = await HttpRepository.GetApiAsync(eApi.Microservicebus, new ApiRequest() { UrlExtension = $"/nodes/{nodeId}/connectionstate" });
                using (var reader = new JsonTextReader(new StringReader(response)) { DateParseHandling = DateParseHandling.None })
                {
                    nodeConnectionState = JObject.Load(reader);
                    nodeConnectionStateUpdatedTime = DateTimeOffset.Parse((string)nodeConnectionState["connectionStateUpdatedTime"]);
                    nodeStatusUpdatedTime = DateTimeOffset.Parse((string)nodeConnectionState["statusUpdatedTime"]);
                    nodeLastActivityTime = DateTimeOffset.Parse((string)nodeConnectionState["lastActivityTime"]);
                }

                // Getting device twin and config
                string deviceId = nodeConnectionState.Value<string>("deviceId");
                DeviceTwin deviceTwin = await IotHubRepository.GetDeviceTwinAsync(deviceId);

                var configAsync = BlobRepository.GetNodeConfigurationAsync(deviceId);


                string uiVersion = "?";
                uiVersion = deviceTwin?.Properties?.Reported?.UiVersion ?? uiVersion;

                string desiredUiVersion = "?";
                try
                {
                    desiredUiVersion = (await configAsync).FrontendConfig.Version ?? desiredUiVersion;
                }
                catch (Exception) { }

                string firmwareVersion = "?";
                var raucState = deviceTwin?.Properties?.Reported?.RaucState;
                var runningFirmwareVersion = raucState?.Rootfs0?.State == "booted" ? raucState?.Rootfs0?.FirmwareVersion : raucState?.Rootfs1?.FirmwareVersion;
                firmwareVersion = runningFirmwareVersion ?? firmwareVersion;

                string desiredFirmwareVersion = "?";
                try
                {
                    desiredFirmwareVersion = (await configAsync).FirmwareVersion ?? desiredFirmwareVersion;
                }
                catch (Exception) { }

                cachedNodesList.Add(new InfoscreenNodeStatusCached(
                    nodeId,
                    node.Value<string>("name"),
                    node.Value<string>("connectionId"),
                    node.Value<string>("machineName"),
                    node.Value<string>("details"),
                    node.Value<string>("publicIp"),
                    node.Value<bool>("enabled"),
                    node.Value<string>("organizationId"),
                    node.Value<bool>("lockToMachine"),
                    node.Value<string>("protocol"),
                    node.Value<string>("npmVersion"),
                    node.Value<bool>("debug"),
                    node.Value<int>("webPort"),
                    node.Value<string>("mode"),
                    node.Value<string>("tags").Split(", "),
                    node.Value<string>("platform"),
                    node.Value<string>("longitude"),
                    node.Value<string>("latitude"),
                    node.Value<bool?>("tracking"),
                    node.Value<string>("iccid"),
                    node.Value<string>("manufactureId"),
                    node.Value<string>("imei"),
                    node.Value<bool>("allowSend"),
                    node.Value<string>("timeZone"),
                    node.Value<int>("retentionPeriod"),

                    nodeConnectionState.Value<bool>("IsOnlineMsb"),
                    deviceId,
                    nodeConnectionState.Value<string>("generationId"),
                    nodeConnectionState.Value<string>("etag"),
                    nodeConnectionState.Value<string>("connectionState"),
                    nodeConnectionState.Value<string>("status"),
                    nodeConnectionState.Value<string>("statusReason"),
                    nodeConnectionStateUpdatedTime,
                    nodeStatusUpdatedTime,
                    nodeLastActivityTime,
                    nodeConnectionState.Value<int>("cloudToDeviceMessageCount"),

                    uiVersion,
                    desiredUiVersion,
                    firmwareVersion,
                    desiredFirmwareVersion
                ));
            }
            cachedNodesList = cachedNodesList.OrderBy(n => n.DeviceId).ToList();
            return cachedNodesList;
        }

        public static List<SociabblePostCached> ManipulateSociabbleData(string data, string cachedFileName)
        {
            var cachedPosts = new List<SociabblePostCached>();

            XmlDocument doc = new();
            doc.LoadXml(data);

            var channelNode = doc.DocumentElement.SelectSingleNode("channel");

            foreach (XmlNode node in channelNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "item":
                        var postGuid = node.SelectSingleNode("guid")?.InnerText;
                        var postLink = node.SelectSingleNode("link")?.InnerText;
                        var postTitle = node.SelectSingleNode("title")?.InnerText;
                        var postDescription = System.Net.WebUtility.HtmlDecode(node.SelectSingleNode("description")?.InnerText);
                        postDescription = Regex.Replace(postDescription, "<.*?>", string.Empty).Replace("&nbsp;", " ");
                        var postPublucationDate = DateTimeOffset.Parse(node.SelectSingleNode("pubDate")?.InnerText);
                        var postImage = node.SelectSingleNode("enclosure")?.Attributes["url"]?.Value;

                        if (string.IsNullOrEmpty(postTitle)) {
                            postTitle = "Social Networks News";
                        }

                        var postCategories = node.SelectNodes("category");
                        var singlePostCategory = "";
                        for (int i = 0; i < postCategories.Count; i++)
                        {
                            if (i != 0) { singlePostCategory += " "; }
                            singlePostCategory += postCategories[i]?.InnerText;
                        }
                        
                        if (cachedFileName == "sociabbleActemium.json" && singlePostCategory.Contains("Actemium", StringComparison.OrdinalIgnoreCase))
                        {
                            cachedPosts.Add(new SociabblePostCached(guid: postGuid, organization: singlePostCategory, link: postLink, title: postTitle, description: postDescription, publicationDate: postPublucationDate, image: postImage));
                        }
                        else if (cachedFileName == "sociabbleAxians.json" && singlePostCategory.Contains("Axians", StringComparison.OrdinalIgnoreCase))
                        {
                            cachedPosts.Add(new SociabblePostCached(guid: postGuid, organization: singlePostCategory, link: postLink, title: postTitle, description: postDescription, publicationDate: postPublucationDate, image: postImage));
                        }
                        else if (cachedFileName == "sociabbleEtavis.json" && singlePostCategory.Contains("Etavis", StringComparison.OrdinalIgnoreCase))
                        {
                            cachedPosts.Add(new SociabblePostCached(guid: postGuid, organization: singlePostCategory, link: postLink, title: postTitle, description: postDescription, publicationDate: postPublucationDate, image: postImage));
                        }
                        break;

                    default:
                        break;
                }
            }
            cachedPosts = cachedPosts.OrderByDescending(p => p.PublicationDate).Take(5).ToList();
            return cachedPosts;
        }

        public static List<UptownArticleCached> ManipulateUptownArticlesData(string data)
        {
            var articleWrapper = JsonConvert.DeserializeObject<UptownArticle_Wrapper>(data);
            return articleWrapper.Articles.Select(a => new UptownArticleCached(a))
                                          .OrderByDescending(a => a.CreatedAt)
                                          .Take(5)
                                          .ToList();
        }

        public static List<UptownEventCached> ManipulateUptownEventsData(string data)
        {
            var events = JsonConvert.DeserializeObject<List<UptownEvent>>(data);

            return events.Select(e => new UptownEventCached(e))
                         .Where(e => string.Compare(e.EventStatus, "confirmed", true) == 0)
                         .Where(e => DateTimeOffset.Parse(e.Ending) > DateTimeOffset.UtcNow)
                         .ToList();
        }

        public static List<JobOffersJobCached> ManipulateJobOffersData(string data, ApiRequest apiRequest)
        {
            var articleWrapper = JsonConvert.DeserializeObject<JobOffersJob_Wrapper>(data);
            var jobsAfterPostProcessing = DoSimplePostProcessing(articleWrapper.Results, apiRequest);

            var mostRecentJobs = jobsAfterPostProcessing
                                          .Select(j => new JobOffersJobCached(j))
                                          .OrderByDescending(j => j.PublishedAt)
                                          .Take(5)
                                          .ToList();

            // Go over the most recent job ads and summarize the content with AI
            foreach (var job in mostRecentJobs)
            {
                // Send prompt to Azure Open AI chat bot and update job content/description
                job.Content = AiChatBotRepository.GetAiChatBotResponse(JOB_OFFERS_BASE_AI_PROMPT_STRING + job.Content);
            }

            return mostRecentJobs;
        }

        /// <summary>
        /// Filter data based on property name and value using the Postprocessing Keyvaluepair list of ApiRequest.
        /// </summary>
        /// <typeparam name="T">Type of the input data</typeparam>
        /// <param name="inputData">Data to filter</param>
        /// <param name="apiRequest">ApiRequest containing the postprocessing key-value pairs</param>
        /// <returns></returns>
        private static IEnumerable<T> DoSimplePostProcessing<T>(IEnumerable<T> inputData, ApiRequest apiRequest)
        {
            var processedData = inputData;
            if(apiRequest.Postprocessing != null)
            {
                // Iterate over the postprocessing entries and filter the data according to it.
                // Uses the "key" as the property name and the "value" as the value of the property.
                foreach (var ppFilter in apiRequest.Postprocessing)
                {
                    // Extract all properties of the type.
                    foreach (var property in typeof(T).GetProperties())
                    {
                        // Try to find the property matching the "key" of this postprocessing entry.
                        if (property.Name.Equals(ppFilter.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            // Extract and compare the value of the found property to filter the input data.
                            processedData = processedData.Where(e => property.GetValue(e).Equals(ppFilter.Value));
                        }
                    }
                }
            }
            
            return processedData;
        }
    }
}
