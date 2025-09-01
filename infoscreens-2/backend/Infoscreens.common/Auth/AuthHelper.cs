using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using HttpRequestData = Microsoft.Azure.Functions.Worker.Http.HttpRequestData;

namespace Infoscreens.Common.Auth
{

    public class AuthHelper
    {
        public static void AutenticateJwtToken(HttpRequestData request)
        {
            // Ignore for localDev environment
            if (CommonConfigHelper.IsLocalDevelopmentEnvironment)
                return;

            if (request == null) throw new AuthCustomException("Missing context for authentication.");

            // Get request's token
            request.Headers.TryGetValues("Authorization", out IEnumerable<string> authorizations);
            var jwtToken = authorizations?.FirstOrDefault() ?? throw new AuthCustomException($"No token provided in the request.");
            if(jwtToken.ToLower().StartsWith("bearer ", StringComparison.CurrentCultureIgnoreCase))
            {
                jwtToken = jwtToken["bearer ".Length..];
            }


            // Gather needed info for auth verification
            var tenantId = CommonConfigHelper.GetCmsTenantId;
            var tenantName = CommonConfigHelper.GetCmsTenantName;
            var clientId = CommonConfigHelper.GetCmsClientId;
            string issuer = $"https://{tenantName}.b2clogin.com/{tenantId}/v2.0/";


            // Parse the JWT token to get policy used to generate it (only checking the tfp claim, the acr claim isn't configured to be used yet)
            // and the signing key id
            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken jwtTokenObj = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken ?? throw new AuthCustomException($"Token couldn't be parsed.");
            Claim policyClaim = jwtTokenObj.Claims.FirstOrDefault(c => c.Type == "tfp") ?? throw new AuthCustomException("No policy claim found in token.");
            string policy = policyClaim.Value;
            string signingKeyId = jwtTokenObj.Header["kid"].ToString() ?? throw new AuthCustomException("No signing key id found in token.");


            // Dynamically check for public keys as those are automatically rotated by Microsoft - Shouldn't cache them more than 24 hours
            // Doc: https://learn.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview#validate-signature
            var openIdConfigUrl = $"https://{tenantName}.b2clogin.com/{tenantName}.onmicrosoft.com/{policy}/v2.0/.well-known/openid-configuration";


            // Validate the token
            // Documentaiton: https://learn.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview#validate-claims
            try
            {

                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(openIdConfigUrl, new OpenIdConnectConfigurationRetriever());
                OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().Result;

                TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidAudience = clientId,

                    IssuerSigningKeys = config.SigningKeys,
                    ValidIssuer = config.Issuer
                };

                var handler = new JwtSecurityTokenHandler();
                ClaimsPrincipal claimsPrincipal = handler.ValidateToken(jwtToken, validationParameters, out SecurityToken validatedToken);

                // Token is valid
                // Set the claims principal as the identity of the current thread
                Thread.CurrentPrincipal = claimsPrincipal;
            }
            catch (SecurityTokenException e)
            {
                throw new AuthCustomException("Token invalid. "+e.Message);
            }
        }
    }
}
