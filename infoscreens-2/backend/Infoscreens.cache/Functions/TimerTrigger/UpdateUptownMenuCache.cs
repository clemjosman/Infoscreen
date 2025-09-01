using Infoscreens.Common.Helpers;
using Infoscreens.Common.Models.CachedData;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Cache.Functions
{
    public class UpdateUptownMenuCache
    {
        #region Constructor / Dependency Injection

        readonly ILogger<UpdateUptownMenuCache> _logger;

        public UpdateUptownMenuCache(ILogger<UpdateUptownMenuCache> logger)
        {
            _logger = logger;
        }

        #endregion Constructor / Dependency Injection

        // Trigger: every hours from 07:00 to 15:00 UTC
        const string FUNCTION_NAME = "UpdateUptownMenuCache";
        [Function(FUNCTION_NAME)]
        public async Task RunAsync([TimerTrigger("0 0 7-15 * * *", RunOnStartup = false)] TimerInfo timer)
        {
            try {
                using HttpClient client = new ();
                string url = "https://community-admin.uptownbasel.ch/restaurant/menu";
                byte[] bytes;

                // Get the PDF
                using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                using Stream stream = await response.Content.ReadAsStreamAsync();

                // Convert it to base 64 string
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
                string base64 = Convert.ToBase64String(bytes);

                // Concert it to a JSON string
                var cache = new UptownMenuCached(base64);
                var cacheString = JsonConvert.SerializeObject(cache, CommonConfigHelper.JsonCamelCaseSettings);

                // Save the base 64 string
                var blob = BlobRepository.GetBlockBlob(CommonConfigHelper.UptownMenuFile);
                await BlobRepository.WriteDataToBlobAsync(blob, cacheString);
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
            }
        }
    }
}
