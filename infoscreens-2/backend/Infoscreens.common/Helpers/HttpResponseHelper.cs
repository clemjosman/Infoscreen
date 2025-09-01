using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Helpers
{
    public static class HttpResponseHelper
    {

        public static HttpResponseData EmptyResponse(HttpRequestData req, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var response = req.CreateResponse(statusCode);
            return response;
        }

        public static async Task<HttpResponseData> JsonResponseAsync(HttpRequestData req, object body, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var settings = NewtonsoftJsonObjectSerializer.CreateJsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var serializer = new NewtonsoftJsonObjectSerializer(settings);

            var response = req.CreateResponse(statusCode);
            await response.WriteAsJsonAsync(body, serializer, "application/json;charset=utf-8", statusCode);
            return response;
        }

        public static async Task<HttpResponseData> TextResponseAsync(HttpRequestData req, string body, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var response = req.CreateResponse(statusCode);
            response.Headers.Add("Content-Type", "text/plain;charset=utf-8");
            await response.WriteStringAsync(body);
            return response;
        }
    }
}
