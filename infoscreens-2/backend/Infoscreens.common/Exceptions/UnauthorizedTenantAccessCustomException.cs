using Infoscreens.Common.Helpers;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public class UnauthorizedTenantAccessCustomException : CustomExceptionBaseClass
    {
        public override string ExceptionMessageLabel { get; } = "error.unauthorizedTenantAccess";
        public override List<string> ExceptionMessageParameters { get; set; } = new List<string>();

        public UnauthorizedTenantAccessCustomException(User user, Tenant tenant, string message = null): base(message ?? $"User with id #{user.Id} is not allowed to access Tenant with code {tenant.Code}.")
        {}

        public override async Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req)
        {
            return await HttpResponseHelper.JsonResponseAsync(req, ToResponseObject(), HttpStatusCode.Unauthorized);
        }
    }
}
