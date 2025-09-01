using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Models.CachedData;
using Infoscreens.Common.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoscreens.Cache
{
    public class CacheRepository
    {
        public static void UpdateApiCache(eApi api, List<ApiRequest> apiRequestsConfig)
        {
            // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
            var exceptions = new ConcurrentQueue<Exception>();

            Parallel.ForEach(apiRequestsConfig, async(apiRequest) =>
            {
                try
                {
                    await UpdateApiCacheAsync(api, apiRequest);
                }
                // Store the exception and continue with the loop.
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
            });

            // Throw the exceptions here after the loop completes.
            if (!exceptions.IsEmpty)
            {
                throw new AggregateException(exceptions);
            }
        }

        public async static Task UpdateApiCacheAsync(eApi api, ApiRequest apiRequest)
        {
            (var response, _) = await HttpRepository.GetApiAsync(api, apiRequest);
            response = await ManipulateApiResponseAsync(api, apiRequest, response);
            await BlobRepository.WriteApiDataAsync(api, apiRequest, response);
        }

        #region Manipulate/Parse data

        public static async Task<string> ManipulateApiResponseAsync(eApi api, ApiRequest apiRequest, string data)
        {
            switch (api)
            {
                #region IdeaBox

                case eApi.Ideabox:
                    var ideas_JArray = JArray.Parse(data);
                    foreach (JObject idea in ideas_JArray.Cast<JObject>())
                    {
                        idea["created"] = DateHelper.ToIsoDateTimeString(idea.Value<DateTime>("created"));
                    }
                    var ideas = ideas_JArray.ToObject<IEnumerable<IdeaCached>>();
                       
                    return JsonConvert.SerializeObject(
                        ideas,
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion IdeaBox

                #region Public Transport

                case eApi.PublicTransport:
                    return JsonConvert.SerializeObject(
                        DataManipulationRepository.ManipulatePublicTransportData(data, apiRequest),
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion Public Transport

                #region Actemium University

                case eApi.University:
                    return JsonConvert.SerializeObject(
                        DataManipulationRepository.ManipulateActemiumUniversityData(data),
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion Actemium University

                #region OpenWeather

                case eApi.OpenWeather:
                    // Get air pollution
                    var airQualityRequest = apiRequest;
                    airQualityRequest.UrlExtension = "/2.5/air_pollution";
                    (var airQualityResponse, _) = await HttpRepository.GetApiAsync(api, apiRequest);

                    // Parse and extract pollution data
                    var pollutionResponseObject = JObject.Parse(airQualityResponse);
                    var pollutionData = pollutionResponseObject.Value<JArray>("list").FirstOrDefault();

                    // Parse weather data, add pollution and serialize
                    var weatherDataObject = JObject.Parse(data);
                    if(pollutionData != null)
                        weatherDataObject.Add("pollution", pollutionData);

                    return weatherDataObject.ToString(
                        CommonConfigHelper.JsonCamelCaseSettings.Formatting
                    );

                #endregion OpenWeather

                #region JobOffers

                case eApi.JobOffers:
                    return JsonConvert.SerializeObject(
                        DataManipulationRepository.ManipulateJobOffersData(data, apiRequest),
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion JobOffers

                #region Twenty min

                case eApi.TwentyMin:
                    return JsonConvert.SerializeObject(
                        DataManipulationRepository.ManipulateTwentyMinData(data),
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion Twenty min

                #region Twitter

                case eApi.Twitter:
                    return JsonConvert.SerializeObject(
                        DataManipulationRepository.ManipulateTwitterData(data),
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion Twitter

                #region Micorservicebus

                case eApi.Microservicebus:
                    return JsonConvert.SerializeObject(
                        await DataManipulationRepository.ManipulateMSBDataAndGetConnectionDataAsync(data),
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion Microservicebus

                #region Sociabble

                case eApi.Sociabble:
                    return JsonConvert.SerializeObject(
                        DataManipulationRepository.ManipulateSociabbleData(data, apiRequest.CachedFileName),
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion Sociabble

                #region UptownArticles

                case eApi.UptownArticle:
                    return JsonConvert.SerializeObject(
                        DataManipulationRepository.ManipulateUptownArticlesData(data),
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion UptownArticles

                #region UptownEvents

                case eApi.UptownEvent:
                    return JsonConvert.SerializeObject(
                        DataManipulationRepository.ManipulateUptownEventsData(data),
                        CommonConfigHelper.JsonCamelCaseSettings
                    );

                #endregion UptownEvents

                default:
                    return data;
            }
        }

        #endregion Manipulate/Parse data
    }
}
