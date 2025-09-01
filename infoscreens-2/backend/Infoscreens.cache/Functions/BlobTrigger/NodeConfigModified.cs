using Infoscreens.Common.Helpers;
using Infoscreens.Common.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;
using Microsoft.Azure.Functions.Worker;

namespace Infoscreens.Cache.Functions
{
    public class NodeConfigModified
    {
        #region Constructor / Dependency Injection

        //readonly ILogger<NodeConfigModified> _logger;

        public NodeConfigModified(
        //    ILogger<NodeConfigModified> logger
        )
        {
        //    _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        //const string FUNCTION_NAME = "NodeConfigModified";
        // DEACTIVATED FOR NOW AS NOT USED ANYMORE
        //[Disable]
        //[Function(FUNCTION_NAME)]
        //public async Task RunAsync([BlobTrigger("infoscreens/config/nodes/{name}", Connection = CommonConfigHelper.StorageConnectionStringName)] string content, string name)
        //{
        //    try { 
        //        if(string.Compare(Environment.GetEnvironmentVariable("ENVIRONMENT"), "Production", true) == 0)
        //        {
        //            var nodeId = name.Split('.')[0];
        //            await IotHubRepository.UpdateDeviceStateAsync(nodeId);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
        //    }
        //}
    }
}
