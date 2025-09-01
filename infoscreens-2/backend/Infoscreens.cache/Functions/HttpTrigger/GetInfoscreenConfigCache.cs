using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetInfoscreenConfigCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetInfoscreenConfigCache> _logger;
        readonly IDatabaseRepository _databaseRepository;

        public GetInfoscreenConfigCache(ILogger<GetInfoscreenConfigCache> logger, IDatabaseRepository databaseRepository)
        {
            _logger = logger;
            _databaseRepository = databaseRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetInfoscreenConfigCache";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/infoscreen")] HttpRequestData req)
        {
            try
            {
                const int AMOUNT_VIDEOS_RETURNED = 10;

                string nodeId = req.Query["nodeId"];
                _logger.LogInformation(new LogItem(101, $"Getting config cache for {nodeId}"));

                if (string.IsNullOrWhiteSpace(nodeId))
                    throw new ArgumentException($"Query parameter 'NodeId' not provided!");

                var node = await BlobRepository.GetNodeConfigurationAsync(nodeId);

                // Handle special case of youtube videos
                if (node.FrontendConfig.Slides.Order.Contains(eSlide.Youtube))
                {
                    var infoscreen = await _databaseRepository.GetInfoscreenByNodeIdAsync(nodeId);

                    // Get youtube videos
                    var videos = await _databaseRepository.GetPublishedVideosForInfoscreenAsync(infoscreen.Id, amount: AMOUNT_VIDEOS_RETURNED, mustBeAssignedToInfoscreens: true);

                    // Add videos to the config if any, else remove the youtube slide from config
                    var videos_json = "[]";
                    if (videos.Count > 0)
                    {
                        // Format data
                        var videoList = videos.Select(async n => await n.ToYoutubeVideoCachedAsync(_databaseRepository))
                                            .Select(t => t.Result);


                        videos_json = JsonConvert.SerializeObject(videoList, CommonConfigHelper.JsonCamelCaseSettings);
                    }

                    // Add videos to node config
                    var youtubeSlideConfig = new JObject
                    {
                        ["videos"] = JArray.Parse(videos_json)
                    };
                    node.FrontendConfig.Slides.Config[eSlide.Youtube] = youtubeSlideConfig;
                }

                return await HttpResponseHelper.JsonResponseAsync(req, node.FrontendConfig);
            }
            catch(FileNotFoundException ex)
            {
                return await HttpResponseHelper.TextResponseAsync(req, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
                throw;
            }
        }
    }
}
