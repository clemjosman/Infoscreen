using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.ApiResponse;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Infoscreens.Common.Repositories;
using Infoscreens.Management.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions
{
    public class GetInfoscreensStatus : BaseApiClass
    {
        #region Constructor / Dependency Injection

        public GetInfoscreensStatus(
            ILogger<BaseApiClass> logger,
            IDatabaseRepository databaseRepository,
            IExceptionHelper exceptionHelper
         ) : base(logger, databaseRepository, exceptionHelper)
        { }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "GetInfoscreensStatus";
        [Function(FUNCTION_NAME)]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/infoscreens/{tenantCode}/status")] HttpRequestData req, string tenantCode)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Http Function {FUNCTION_NAME}() called.")
                { });

                #region Authentication and tenant permission check

                Tenant tenant;
                (_, tenant) = await BasicApiCallPermissionCheckAsync(req, tenantCode);

                #endregion Authentication and tenant permission check


                var infoscreensFromTenant = await _databaseRepository.GetInfoscreensFromTenantAsync(tenant);

                // Filter out virtual infoscreens 
                infoscreensFromTenant = infoscreensFromTenant.Where(i => !i.IsVirtualInfoscreen());

                // If no infoscreens found, return empty list
                if (!infoscreensFromTenant.Any())
                {
                    return await HttpResponseHelper.JsonResponseAsync(req, new List<apiInfoscreen_Status>());
                }

                // Get data from MSB for each infoscreen bind to the tenant
                var infoscreensMSBData = new ConcurrentBag<MSB_Node>();
                var api = eApi.Microservicebus;
                object lock_infoscreensMSBData = new();
                await Parallel.ForEachAsync(infoscreensFromTenant, async (infoscreen, cancellationToken) =>
                {
                    var apiConfig = new ApiRequest(){
                        UrlExtension = $"/nodes/{infoscreen.MsbNodeId}"
                    };

                    try
                    {
                        (var responseBody, var responseHttp) = await HttpRepository.GetApiAsync(api, apiConfig);
                        if (responseHttp.IsSuccessStatusCode)
                        {
                            var nodeData = JsonConvert.DeserializeObject<MSB_Node>(responseBody);
                            infoscreensMSBData.Add(nodeData);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occured when getting infoscreen status for #" + infoscreen.Id);
                    }
                });

                // Manipulating the node data and adding the connection status to it
                var nodeStatusCached = await DataManipulationRepository.ManipulateMSBDataAndGetConnectionDataAsync(
                    JsonConvert.SerializeObject(infoscreensMSBData, CommonConfigHelper.JsonCamelCaseSettings)
                );

                // Adding management data, extracting nodes not defined in the database, formating into api model
                var nodeStatus = nodeStatusCached.Select(nsc =>{
                                                    var infoscreen = infoscreensFromTenant.FirstOrDefault(i => i.MsbNodeId == nsc.Id);
                                                    if (infoscreen == null)
                                                        return null;
                                                    return new apiInfoscreen_Status(infoscreen, nsc, _databaseRepository);
                                                  })
                                                  .Where(ns => ns != null)
                                                  .ToList();


                _logger.LogDebug(new LogItem(11, $"Http Function {FUNCTION_NAME}() finished.")
                {
                    Custom1 = nodeStatus != null ? JsonConvert.SerializeObject(nodeStatus) : "Returning null."
                });

                return await HttpResponseHelper.JsonResponseAsync(req, nodeStatus);
            }
            catch (CustomExceptionBaseClass customException)
            {
                _exceptionHelper.HandleCustomException(FUNCTION_NAME, customException);
                return await customException.ToApiResponseAsync(req);
            }
            catch (Exception exception)
            {
                _exceptionHelper.HandleException(FUNCTION_NAME, exception);
                return await _exceptionHelper.ExceptionToResponseAsync(req, exception);
            }
        }
    }
}
