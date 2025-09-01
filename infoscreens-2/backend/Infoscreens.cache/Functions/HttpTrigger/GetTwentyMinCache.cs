using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Models.CachedData;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetTwentyMinCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetTwentyMinCache> _logger;

        public GetTwentyMinCache(ILogger<GetTwentyMinCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetTwentyMinCache";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/twentyMin")] HttpRequestData req)
        {
            try
            {
                var api = eApi.TwentyMin;
                string nodeId = req.Query["nodeId"];
                var node = await BlobRepository.GetNodeConfigurationAsync(nodeId);

                var cachedFileNames = node.BackendConfig.GetCachedFileNames(api);
                var maxNewsDateAge = node.BackendConfig.DataEndpointConfig.TwentyMin.MaxNewsDateAge;
                var currentDateStart = DateTimeOffset.UtcNow.Date;
                var minDate = currentDateStart.AddDays(-maxNewsDateAge);

                var cachedNewsChannel = new List<TwentyMinChannelCached>();
                foreach(var cachedFileName in cachedFileNames)
                {
                    var channelCached = JsonConvert.DeserializeObject<TwentyMinChannelCached>(await BlobRepository.GetCachedDataAsync(api, cachedFileName));
                    channelCached.News = channelCached.News.Where(n => DateTimeOffset.Parse(n.PublicationDate) >= minDate).ToList();
                    cachedNewsChannel.Add(channelCached);
                }

                return await HttpResponseHelper.JsonResponseAsync(req, cachedNewsChannel);
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
