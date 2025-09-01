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
using System.Net;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class GetJobOffersCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<GetJobOffersCache> _logger;

        public GetJobOffersCache(ILogger<GetJobOffersCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetJobOffersCache";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/jobOffers")] HttpRequestData req)
        {
            try
            {
                var api = eApi.JobOffers;
                string nodeId = req.Query["nodeId"];
                var node = await BlobRepository.GetNodeConfigurationAsync(nodeId);

                var fileNames = node.BackendConfig.GetCachedFileNames(api);

                List<JobOffersJobCached> combinedJobsList = [];
                foreach (var fileName in fileNames)
                {
                    var response = await BlobRepository.GetCachedDataAsync(api, fileName);
                    var jobs = JsonConvert.DeserializeObject<IEnumerable<JobOffersJobCached>>(response);

                    combinedJobsList.AddRange(jobs);
                }

                return await HttpResponseHelper.JsonResponseAsync(req, combinedJobsList);
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
