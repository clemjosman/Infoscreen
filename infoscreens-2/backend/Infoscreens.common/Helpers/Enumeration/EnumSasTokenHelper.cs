using Infoscreens.Common.Enumerations.Attributes;
using Infoscreens.Common.Models.Tokens;
using Infoscreens.Common.Repositories;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infoscreens.Common.Helpers.Enumerations
{
    public static class EnumSasTokenHelper
    {

        public static bool HasSasToken(this object enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
                return false;

            var attributes = memInfo[0].GetCustomAttributes(typeof(SasTokenAttribute), false);
            return attributes.Length != 0;

        }

        public static async Task<string> GetSasTokenAsync(this object enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo == null || memInfo.Length == 0)
                return enumVal.ToString();

            var attribute = (SasTokenAttribute)(memInfo[0].GetCustomAttributes(typeof(SasTokenAttribute), false)[0]);
            var tokenBlob = BlobRepository.GetBlockBlob("tokens/iothub.json");
            var tokenDataString = await BlobRepository.ReadBlockBlobContentAsync(tokenBlob);

            if (!string.IsNullOrWhiteSpace(tokenDataString))
            {
                var tokenData = JsonConvert.DeserializeObject<SasToken>(tokenDataString);
                if (IsTokenStillValid(tokenData))
                {
                    return tokenData.Token;
                }
            }
            return await GenerateNewSasTokenAsync(attribute, tokenBlob);
        }

        private static bool IsTokenStillValid(SasToken tokenData)
        {
            var currentDateTime = DateTime.UtcNow;
            return currentDateTime.AddMinutes(CommonConfigHelper.TokenMinValidityMinutesLeft) < tokenData.ExpiryDate;
        }

        private static async Task<string> GenerateNewSasTokenAsync(SasTokenAttribute attribute, CloudBlockBlob blob)
        {
            // source: https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-security
            TimeSpan fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
            string expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + attribute.MinutesOfValidity * 60);

            string stringToSign = WebUtility.UrlEncode(attribute.EndPoint) + "\n" + expiry;

            HMACSHA256 hmac = new(Convert.FromBase64String(attribute.PolicyKey));
            string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            string sasToken = string.Format(CultureInfo.InvariantCulture,
                                            "SharedAccessSignature sr={0}&sig={1}&se={2}",
                                            WebUtility.UrlEncode(attribute.EndPoint),
                                            WebUtility.UrlEncode(signature),
                                            expiry);

            if (!string.IsNullOrEmpty(attribute.PolicyName))
            {
                sasToken += "&skn=" + attribute.PolicyName;
            }

            var tokenData = new SasToken()
            {
                Attributes = attribute,
                ExpiryDate = DateTime.UtcNow.AddMinutes(attribute.MinutesOfValidity),
                Token = sasToken
            };

            await BlobRepository.WriteDataToBlobAsync(blob, JsonConvert.SerializeObject(tokenData, CommonConfigHelper.JsonCamelCaseSettings));

            return sasToken;
        }
    }
}