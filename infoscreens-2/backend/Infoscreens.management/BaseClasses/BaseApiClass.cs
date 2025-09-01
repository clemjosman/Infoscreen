using Infoscreens.Common.Auth;
using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Infoscreens.Management.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Helpers
{
    public class BaseApiClass
    {

        protected readonly ILogger<BaseApiClass> _logger;
        protected readonly IDatabaseRepository _databaseRepository;
        protected readonly IExceptionHelper _exceptionHelper;

        public BaseApiClass(ILogger<BaseApiClass> logger, IDatabaseRepository databaseRepository, IExceptionHelper exceptionHelper)
        {
            _logger = logger;
            _databaseRepository = databaseRepository;
            _exceptionHelper = exceptionHelper;
        }


        public async Task<T> ExtractRequestBodyAsync<T> (HttpRequestData req)
        {
            _logger.LogDebug(new LogItem(10, $"ExtractRequestBodyAsync<{typeof(T).Name}>() called.")
            {
                Custom1 = req != null ? req.ToString() : "Parameter req is null."
            });

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                var bodyObject = JsonConvert.DeserializeObject<T>(requestBody);


                _logger.LogDebug(new LogItem(11, $"ExtractRequestBodyAsync<{typeof(T).Name}>() finished.")
                {
                    Custom1 = req != null ? req.ToString() : "Parameter req is null.",
                    Custom4 = requestBody
                });

                return bodyObject;
            }
            catch(Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, $"Could not parse body of the request into type '{typeof(T).Name}'.")
                {
                    Custom4 = requestBody
                });
                throw new RequestBodyNotMatchingRequirementsCustomException();
            }
        }



        #region User Authentication

        protected static ClaimsPrincipal GetClaimsPrincipal()
        {
            if (CommonConfigHelper.IsLocalDevelopmentEnvironment)
            {
                ClaimsIdentity identity = new (new List<Claim>() {
                    new(CommonConfigHelper.ObjectIdClaimType, CommonConfigHelper.LocalDevObjectId)
                });
                return new ClaimsPrincipal(identity);
            }
            else
            {
                return Thread.CurrentPrincipal as ClaimsPrincipal;
            }
        }

        protected static string GetObjectIdFromTokenWithoutVerification(HttpRequestData request)
        {
            if (CommonConfigHelper.IsLocalDevelopmentEnvironment)
            {
                return CommonConfigHelper.LocalDevObjectId;
            }
            else
            {
                request.Headers.TryGetValues("Authorization", out IEnumerable<string> authorizations);
                var jwtToken = authorizations.FirstOrDefault() ?? throw new AuthCustomException($"No token provided un the request.");
                JwtSecurityTokenHandler tokenHandler = new();
                var jwtTokenObj = new JwtSecurityToken(jwtToken.Replace("Bearer ", ""));
                //JwtSecurityToken jwtTokenObj = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken ?? throw new AuthCustomException($"Token couldn't be parsed.");
                return jwtTokenObj.Claims.FirstOrDefault(c => {
                    return c.ValueType == CommonConfigHelper.ObjectIdClaimType || c.Type == CommonConfigHelper.ObjectIdClaim;
                })?.Value;
            }
        }

        private static string GetObjectId()
        {
            var claims = GetClaimsPrincipal();
            return claims.FindFirst(CommonConfigHelper.ObjectIdClaimType)?.Value;
        }

        protected async Task<User> BasicApiCallPermissionCheckAsync(HttpRequestData request, eApplication app = eApplication.CMS_WEB_UI)
        {
            var (user, _) = await BasicApiCallPermissionCheckAsync(request, null, app);
            return user;
        }

        protected async Task<(User user, Tenant tenant)> BasicApiCallPermissionCheckAsync(HttpRequestData request, string tenantCode, eApplication app = eApplication.CMS_WEB_UI)
        {
            // Init
            Tenant tenant = null;

            // Validate token
            AuthHelper.AutenticateJwtToken(request);

            // Get Authenticated user
            var userId = GetObjectId();
            User user = await _databaseRepository.GetUserByObjectIdAsync(userId);
            switch (app)
            {
                case eApplication.MOBILE_APP:
                case eApplication.CMS_WEB_UI:
                default:
                    {
                        if (user == null)
                        {
                            throw new UserNotFoundCustomException(userId);
                        }
                        break;
                    }
            }

            // Check if user account valid for selected app
            if(app != eApplication.MOBILE_APP)
            {
                if (!user.SelectedTenantId.HasValue)
                    throw new UnauthorizedAccessException();
            }

            // Only check for tenant if a tenant code is given
            if (tenantCode != null)
            {
                // Get requested tenant
                tenant = await _databaseRepository.GetTenantByCodeAsync(tenantCode);
                if (tenant == null)
                {
                    throw new TenantNotFoundCustomException(tenantCode);
                }

                // Check if user has access to requested tenant
                switch (app)
                {
                    case eApplication.MOBILE_APP:
                        {
                            if(!CommonConfigHelper.MobileAppAllowedTenantCodes.Contains(tenantCode))
                            {
                                throw new UnauthorizedTenantAccessCustomException(user, tenant);
                            }
                            break;
                        }
                    case eApplication.CMS_WEB_UI:
                    default:
                        {
                            if (!(await PermissionHelper.HasUserAccessToTenantAsync(_databaseRepository, user, tenant)))
                            {
                                throw new UnauthorizedTenantAccessCustomException(user, tenant);
                            }
                            break;
                        }
                }
            }

            // Everything is ok, we return the user and tenant
            return (user, tenant);
        }


        #endregion User Authentication
    }
}
