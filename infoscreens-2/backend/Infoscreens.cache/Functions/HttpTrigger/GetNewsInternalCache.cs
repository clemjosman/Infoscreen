using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using vesact.common.file.Interfaces;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetNewsInternalCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetNewsInternalCache> _logger;
        readonly IDatabaseRepository _databaseRepository;
        readonly IFileHelper _fileHelper;

        public GetNewsInternalCache(ILogger<GetNewsInternalCache> logger, IDatabaseRepository databaseRepository, IFileHelper fileHelper)
        {
            _logger = logger;
            _databaseRepository = databaseRepository;
            _fileHelper = fileHelper;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetNewsInternalCache";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/newsinternal")] HttpRequestData req)
        {
            try
            {
                // Get infoscreen
                string nodeId = req.Query["nodeId"];
                var infoscreen = await _databaseRepository.GetInfoscreenByNodeIdAsync(nodeId);
                var config = await BlobRepository.GetNodeConfigurationAsync(nodeId);
                var maxNewsCount = config.BackendConfig.DataEndpointConfig.NewsInternal.MaxNewsCount;

                // Get news to return
                var news = await _databaseRepository.GetPublishedNewsForInfoscreenAsync(infoscreen.Id, amount: maxNewsCount, mustBeAssignedToInfoscreens: true);
                news = news.Where(n => n.IsForInfoscreens).ToList();

                // Format data
                var newsResponse = news.Select(async n => await n.ToInternalNewsCachedAsync(_databaseRepository, _fileHelper, CommonConfigHelper.AttachmentFileSasExpiry_Infoscreens, infoscreen.DefaultContentLanguage))
                                    .Select(t => t.Result);

                return await HttpResponseHelper.JsonResponseAsync(req, newsResponse);
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
