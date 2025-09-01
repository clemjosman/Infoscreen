using Infoscreens.Common.Helpers;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public class TenantNotFoundCustomException : CustomExceptionBaseClass
    {
        public override string ExceptionMessageLabel { get; } = "error.tenantNotFound";
        public override List<string> ExceptionMessageParameters { get; set; } = new List<string>();

        public TenantNotFoundCustomException(string tenantCode, string message = null) : base(message ?? $"Tenant with code {tenantCode} was not found.")
        {}

        public override async Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req)
        {
            return await HttpResponseHelper.JsonResponseAsync(req, ToResponseObject(), HttpStatusCode.BadRequest);
        }
    }
}
