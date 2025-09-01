// using Infoscreens.Common.Enumerations;
// using Infoscreens.Common.Helpers;
// using Infoscreens.Common.Models.CachedData;
// using Infoscreens.Common.Repositories;
// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Azure.Functions.Worker.Http;
// using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Net;
// using System.Threading.Tasks;
// using vesact.common.Log;
// 
// namespace Infoscreens.Cache.Functions
// {
//     public class GetNewsPublicCache
//     {
//         #region Constructor / Dependency Injection
//
//        readonly ILogger<GetNewsPublicCache> _logger;
//
//        public GetNewsPublicCache(ILogger<GetNewsPublicCache> logger)
//        {
//            _logger = logger;
//        }
//
//         #endregion Constructor / Dependency Injection
// 
//         const string FUNCTION_NAME = "GetNewsPublicCache";
//         [Disable]
//         [Function(FUNCTION_NAME)]
//         public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/newspublic")] HttpRequestData req)
//         {
//             try
//             {
//                 var api = eApi.NewsPublic;
//                 string nodeId = req.Query["nodeId"];
//                 var node = await BlobRepository.GetNodeConfigurationAsync(nodeId);
//                 var config = await BlobRepository.GetNodeConfigurationAsync(nodeId);
//                 var maxNewsCount = config.BackendConfig.DataEndpointConfig.NewsPublic.MaxNewsCount;
// 
//                 var newsString = await BlobRepository.GetCachedDataAsync(api, node.BackendConfig.GetCachedFileName(api));
//                 var news = JsonConvert.DeserializeObject<IEnumerable<ActemiumNewsCached>>(newsString);
// 
//                 if (maxNewsCount > 0)
//                 {
//                     news = news.OrderByDescending(n => n.PublicationDate).Take(maxNewsCount);
//                 }
// 
//                 return await HttpResponseHelper.JsonResponseAsync(req, news);
//             }
//             catch(FileNotFoundException ex)
//             {
//                 return await HttpResponseHelper.TextResponseAsync(req, ex.Message, HttpStatusCode.BadRequest);
//             }
//             catch (Exception exception)
//             {
//                 _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
//                 throw;
//             }
//         }
//     }
// }
