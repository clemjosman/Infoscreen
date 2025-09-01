using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Helpers.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infoscreens.Common.Repositories
{
    public class HttpRepository
    {
        #region Data API

        public static async Task<(string responseBody, HttpResponseMessage httpResponse)> GetApiAsync(eApi api, ApiRequest apiRequest)
        {
            (string url, HttpClient client) = CreateUrlAndHeaders(api, apiRequest);
            var httpResponse = await client.GetAsync(url);
            string responseBody = await httpResponse.Content.ReadAsStringAsync();
            client.Dispose();
            return (responseBody, httpResponse);
        }

        public async static Task<string> PatchApiAsync(eApi api, ApiRequest apiRequest, string data)
        {
            (string url, HttpClient client) = CreateUrlAndHeaders(api, apiRequest);
            var httpResponse = await client.PatchAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
            string responseBody = await httpResponse.Content.ReadAsStringAsync();
            client.Dispose();
            return responseBody;
        }

        #endregion

        #region Standard calls

        public async static Task<(string responseBody, HttpResponseMessage httpResponse)> GetAsync(string url)
        {
            using var client = new HttpClient();
            var httpResponse = await client.GetAsync(url);
            string responseBody = await httpResponse.Content.ReadAsStringAsync();
            return (responseBody, httpResponse);
        }

        #endregion

        #region Private

        private static (string url, HttpClient client) CreateUrlAndHeaders(eApi api, ApiRequest apiRequest)
        {
            var url = CreateUrlWithQueryParameters(api, apiRequest);
            HttpClient client = new();
            client = AddHeaderParametersToRequest(client, api, apiRequest);

            return (url, client);
        }

        private static string CreateUrlWithQueryParameters(eApi api, ApiRequest apiRequest)
        {
            string url = EnumRootUrlHelper.GetRootUrl(api);

            // Adding url extension
            if(!string.IsNullOrWhiteSpace(apiRequest.UrlExtension))
            {
                url = url.TrimEnd('/') + '/' + apiRequest.UrlExtension.TrimStart('/');
            }

            // Merging parameters defined in the api enumeration and the one from the node config file
            var parameters = api.GetParams().Concat(apiRequest.Params ?? new List<KeyValuePair<string, string>>()).ToList();

            // Adding the parameters to the root url
            if (parameters.Count > 0)
            {
                url += "?";
                bool firstParam = true;
                foreach (KeyValuePair<string, string> param in parameters)
                {
                    if (string.IsNullOrWhiteSpace(param.Key))
                        continue;

                    if (firstParam)
                    {
                        firstParam = false;
                    }
                    else
                    {
                        url += "&";
                    }
                    url += param.Key;

                    if (string.IsNullOrWhiteSpace(param.Value))
                        continue;
                    url += $"={param.Value}";
                }
            }

            return url;
        }

        private static HttpClient AddHeaderParametersToRequest(HttpClient client, eApi api, ApiRequest apiRequest)
        {
            // Merging headers defined in the api enumeration and the one from the node config file
            var headers = api.GetHeaders().Concat(apiRequest.Headers ?? new List<KeyValuePair<string, string>>()).ToList();

            // Adding sas token if needed
            if(api.HasSasToken())
            {
                var index = headers.FindIndex(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase));
                if(index != -1)
                {
                    headers.RemoveAt(index);
                }
                var taskToken = Task.Run(async () => await api.GetSasTokenAsync());
                var token = taskToken.Result;
                headers.Add(new KeyValuePair<string, string>("authorization", token));
            }

            // Adding OAuth2 token if needed
            var hasOAuth2ClientCredentialsToken = api.HasOAuth2ClientCredentialsToken();
            var hasOAuth2PasswordTokenToken = api.HasOAuth2PasswordTokenToken();
            if (hasOAuth2ClientCredentialsToken || hasOAuth2PasswordTokenToken)
            {
                var index = headers.FindIndex(h => h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase));
                if (index != -1)
                {
                    headers.RemoveAt(index);
                }

                var taskToken = Task.Run(async () => {
                    if (hasOAuth2ClientCredentialsToken) return await api.GetOAuth2ClientCredentialsTokenAsync();
                    else return await api.GetOAuth2PasswordTokenTokenAsync();
                });
                taskToken.Wait();
                var token = taskToken.Result;
                headers.Add(new KeyValuePair<string, string>("authorization", token));
            }

            // Adding the headers to the request
            foreach (var header in headers)
            {
                if (string.IsNullOrWhiteSpace(header.Key) || string.IsNullOrWhiteSpace(header.Value))
                    continue;
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            return client;
        }

        #endregion Private
    }
}
