using Infoscreens.Common.Enumerations.Attributes;
using Infoscreens.Common.Models.Tokens;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infoscreens.Common.Helpers.Enumerations
{
    public static class EnumOAuth2PasswordTokenHelper
    {

        public static bool HasOAuth2PasswordTokenToken(this object enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
                return false;

            var attributes = memInfo[0].GetCustomAttributes(typeof(OAuth2PasswordTokenAttribute), false);
            return attributes.Length != 0;

        }

        public static async Task<string> GetOAuth2PasswordTokenTokenAsync(this object enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
                return enumVal.ToString();

            var attribute = (OAuth2PasswordTokenAttribute)(memInfo[0].GetCustomAttributes(typeof(OAuth2PasswordTokenAttribute), false)[0]);
            var tokenBlob = BlobRepository.GetBlockBlob($"tokens/{EnumMemberParamHelper.GetEnumMemberAttrValue(enumVal)}.json");
            var tokenDataString = await BlobRepository.ReadBlockBlobContentAsync(tokenBlob);

            if (!string.IsNullOrWhiteSpace(tokenDataString))
            {
                var tokenData = JsonConvert.DeserializeObject<OAuthToken>(tokenDataString);
                if (IsTokenStillValid(tokenData))
                {
                    return tokenData.Token;
                }
            }
            return await GenerateNewOAuthTokenAsync(attribute, tokenBlob);
        }

        private static bool IsTokenStillValid(OAuthToken tokenData)
        {
            var currentDateTime = DateTime.UtcNow;
            return currentDateTime.AddMinutes(CommonConfigHelper.TokenMinValidityMinutesLeft) < tokenData.ExpiryDate;
        }

        private static async Task<string> GenerateNewOAuthTokenAsync(OAuth2PasswordTokenAttribute attribute, CloudBlockBlob blob)
        {

            attribute.ClientId = Uri.EscapeDataString(attribute.ClientId);
            attribute.ClientSecret = Uri.EscapeDataString(attribute.ClientSecret);

            var consumerConcatenated = Convert.ToBase64String(
                                            Encoding.Default.GetBytes($"{attribute.ClientId}:{attribute.ClientSecret}")
                                          );

            // Preparing http client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", consumerConcatenated);
            
            // Preparing Body 
            var data = $"grant_type=password&username={HttpUtility.UrlEncode(attribute.Username)}&password={HttpUtility.UrlEncode(attribute.Password)}";
            var body = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
            
            // Making call and getting response
            var httpResponse = await client.PostAsync(attribute.Url, body);
            string responseBody = await httpResponse.Content.ReadAsStringAsync();
            var responseParsed = JsonConvert.DeserializeObject<OAuthTokenResponse>(responseBody);

            // Advised by twitter at https://developer.twitter.com/en/docs/basics/authentication/overview/application-only#issuing-application-only-requests
            if (string.Compare(responseParsed.Token_type, "bearer", true) != 0)
            {
                throw new Exception("Received token is not from type bearer");
            }
            responseParsed.Access_token = $"bearer {responseParsed.Access_token}";

            var tokenData = new OAuthToken()
            {
                Attributes = attribute,
                ExpiryDate = null,
                Token = responseParsed.Access_token
            };

            await BlobRepository.WriteDataToBlobAsync(blob, JsonConvert.SerializeObject(tokenData, CommonConfigHelper.JsonCamelCaseSettings));

            return responseParsed.Access_token;
        }
    }
}