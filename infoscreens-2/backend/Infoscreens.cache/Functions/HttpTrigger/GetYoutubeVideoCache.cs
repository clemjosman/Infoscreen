using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetYoutubeVideoCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetYoutubeVideoCache> _logger;
        readonly IDatabaseRepository _databaseRepository;

        public GetYoutubeVideoCache(ILogger<GetYoutubeVideoCache> logger, IDatabaseRepository databaseRepository)
        {
            _logger = logger;
            _databaseRepository = databaseRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetYoutubeVideoCache";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/youtube")] HttpRequestData req)
        {
            try
            {
                const int AMOUNT_VIDEOS_RETURNED = 3;

                // Get infoscreen
                string nodeId = req.Query["nodeId"];
                var infoscreen = await _databaseRepository.GetInfoscreenByNodeIdAsync(nodeId);

                // Get videos to return
                var videos = await _databaseRepository.GetPublishedVideosForInfoscreenAsync(infoscreen.Id, amount: AMOUNT_VIDEOS_RETURNED, mustBeAssignedToInfoscreens: true);
                videos = videos.Where(n => n.IsForInfoscreens).ToList();

                // Format data
                var response = videos.Select(async n => await n.ToYoutubeVideoCachedAsync(_databaseRepository))
                .Select(t => t.Result);

                return await HttpResponseHelper.JsonResponseAsync(req, response);
            }
            catch (FileNotFoundException ex)
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
