using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Helpers.Enumerations;
using Infoscreens.Common.Models.Configs;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infoscreens.Common.Repositories
{
    public class BlobRepository
    {

        private static CloudBlobContainer _container;

        public static CloudBlobContainer Container {
            get
            {
                if(_container == null)
                {
                    var storageAccount = CloudStorageAccount.Parse(CommonConfigHelper.StorageConnectionString);
                    var blobClient = storageAccount.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference(CommonConfigHelper.BlobContainerName);
                    var creationTask = container.CreateIfNotExistsAsync();
                    creationTask.Wait();
                    _container = container;
                }
                return _container;
            }
        }

        #region Help Methodes

        private static string GetCachedFilePath(eApi api, string cachedFileName)
        {
            return $"{CommonConfigHelper.CacheDirectoryPath}{EnumMemberParamHelper.GetEnumMemberAttrValue(api)}/{cachedFileName}";
        }

        #region Node Config

        public async static Task<List<NodeConfig>> GetAllNodesConfigurationAsync()
        {
            var configBlobDirectory = GetBlobDirectory(CommonConfigHelper.NodeConfigDirectoryPath);
            var configBlobs = await configBlobDirectory.ListBlobsSegmentedAsync(true, BlobListingDetails.Metadata, int.MaxValue, null, null, null);

            var nodes = new List<NodeConfig>();
            foreach (CloudBlockBlob configBlob in configBlobs.Results.Cast<CloudBlockBlob>())
            {
                var config = await ReadBlockBlobContentAsync(configBlob);
                nodes.Add(JsonConvert.DeserializeObject<NodeConfig>(config));
            }

            return nodes;
        }

        public static CloudBlockBlob GetNodeConfigBlob(string nodeId)
        {
            return GetBlockBlob(CommonConfigHelper.NodeConfigDirectoryPath + $"/{nodeId}.json");
        }

        public async static Task<NodeConfig> GetNodeConfigurationAsync(string nodeId)
        {
            var configBlob = GetNodeConfigBlob(nodeId);
            if(configBlob == null || ! await configBlob.ExistsAsync())
                throw new KeyNotFoundException($"The node with the Id {nodeId} was not found.");

            var config = await ReadBlockBlobContentAsync(configBlob);
            return JsonConvert.DeserializeObject<NodeConfig>(config);
        }

        public async static Task UpdateNodeConfigurationAsync(string nodeId, NodeConfig nodeConfig)
        {
            var configBlob = GetNodeConfigBlob(nodeId);
            if (configBlob == null || !await configBlob.ExistsAsync())
                throw new KeyNotFoundException($"The node with the Id {nodeId} was not found.");

            await WriteDataToBlobAsync(configBlob, JsonConvert.SerializeObject(nodeConfig, CommonConfigHelper.JsonCamelCaseSettings));
        }

        public async static Task<string> GetCachedDataAsync(eApi api, string cachedFileName)
        {
            var dataBlob = Container.GetBlockBlobReference(GetCachedFilePath(api, cachedFileName));
            string data;

            if (!(await dataBlob.ExistsAsync()))
            {
                throw new FileNotFoundException($"There are no cached data from api '{EnumMemberParamHelper.GetEnumMemberAttrValue(api)}' with the name '{cachedFileName}'.");
            }

            using (StreamReader reader = new(await dataBlob.OpenReadAsync()))
            {
                data = reader.ReadToEnd();
            }

            return data;
        }

        public static CloudBlob GetUiBlob(string version)
        {
            return GetBlockBlob(CommonConfigHelper.ReleaseDirectoryPath + CommonConfigHelper.GetReleaseName(version));
        }

        #endregion Node Config

        #region Api Config

        public async static Task<List<ApiRequest>> GetApiRequestsConfigAsync(eApi api)
        {
            var configBlob = GetBlockBlob(CommonConfigHelper.ApiConfigDirectoryPath + $"/{EnumMemberParamHelper.GetEnumMemberAttrValue(api)}.json");

            if (configBlob == null || !await configBlob.ExistsAsync())
                throw new KeyNotFoundException($"The config file for the api {EnumMemberParamHelper.GetEnumMemberAttrValue(api)} was not found.");

            var config = await ReadBlockBlobContentAsync(configBlob);
            return JsonConvert.DeserializeObject<List<ApiRequest>>(config);
        }

        #endregion Api Cofig


        #region Job Offers

        public static async Task<string> GetJobOffersConfigAsync(string fileName)
        {
            var blob = GetBlockBlob(CommonConfigHelper.ConfigDirectoryPath + $"/common/jobOffers/{fileName}");
            return await ReadBlockBlobContentAsync(blob);
        }

        #endregion Job Offers

        public static CloudBlockBlob GetBlockBlob(string path)
        {
            return Container.GetBlockBlobReference(path);
        }

        public static CloudBlobDirectory GetBlobDirectory(string path)
        {
            return Container.GetDirectoryReference(path);
        }

        public static async Task<string> GetBlobDownloadUrlAsync(CloudBlob blob) {
            if (!(await blob.ExistsAsync()))
                throw new FileNotFoundException("Cannot create a downloadlink for non existing blob");

            SharedAccessBlobPolicy sharedAccessBlobPolicy = new()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.Add(CommonConfigHelper.BuildDownloadLinkTimeOfLive),
                Permissions = SharedAccessBlobPermissions.Read
            };

            var sharedAccessSignature = blob.GetSharedAccessSignature(sharedAccessBlobPolicy);
            return blob.Uri + sharedAccessSignature;

        }

        #endregion helpMethodes

        #region Read

        public async static Task<string> ReadBlockBlobContentAsync(CloudBlockBlob blob)
        {
            string content;

            if (!(await blob.ExistsAsync()))
            {
                return null;
            }

            using (StreamReader reader = new(await blob.OpenReadAsync()))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }

        #endregion Read

        #region Write

        public async static Task WriteApiDataAsync(eApi api, ApiRequest apiRequest, string data)
        {
            var blob = Container.GetBlockBlobReference(GetCachedFilePath(api, apiRequest.CachedFileName));
            await WriteDataToBlobAsync(blob, data);
        }

        public async static Task WriteDataToBlobAsync(CloudBlockBlob blob, string data)
        {
            await blob.DeleteIfExistsAsync();
            await blob.UploadTextAsync(data);
        }

        #endregion Write
    }
}
