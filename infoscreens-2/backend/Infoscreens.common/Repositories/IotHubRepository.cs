using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Models.ConfigObjects.IoT_Hub_DeviceTwin;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Infoscreens.Common.Repositories
{
    public class IotHubRepository
    {
        public static async Task UpdateDeviceStateAsync(string nodeId)
        {
            var api = eApi.IotHub;
            var apiRequest = new ApiRequest()
            {
                UrlExtension = $"twins/{nodeId}"
            };

            //var deviceTwin = HttpRepository.GetApi(api, apiRequest);
            var nodeConfig = await BlobRepository.GetNodeConfigurationAsync(nodeId);

            var deviceTwinModifications = new
            {
                properties = new
                {
                    desired = new
                    {
                        nodeId = nodeConfig.NodeId,
                        language = nodeConfig.FrontendConfig.Language,
                        infoscreen = nodeConfig.FrontendConfig
                    }
                }
            };

            var requestBody = JsonConvert.SerializeObject(deviceTwinModifications, CommonConfigHelper.JsonCamelCaseSettings);
            await HttpRepository.PatchApiAsync(api, apiRequest, requestBody);
        }

        public async static Task<DeviceTwin> GetDeviceTwinAsync(string nodeId)
        {
            var api = eApi.IotHub;
            var apiRequest = new ApiRequest()
            {
                UrlExtension = $"twins/{nodeId}"
            };

            (var deviceTwin_str, _) = await HttpRepository.GetApiAsync(api, apiRequest);
            return JsonConvert.DeserializeObject<DeviceTwin>(deviceTwin_str);
        }


    }
}
