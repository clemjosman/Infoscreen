using Infoscreens.Common.Helpers;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public class InfoscreenNotFoundCustomException : CustomExceptionBaseClass
    {
        public override string ExceptionMessageLabel { get; } = "error.infoscreenNotFound";
        public override List<string> ExceptionMessageParameters { get; set; } = new List<string>();

        public InfoscreenNotFoundCustomException(string nodeId, string message = null) : base(message ?? $"Infoscreen with node id {nodeId} was not found.")
        {}

        public override async Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req)
        {
            return await HttpResponseHelper.JsonResponseAsync(req, ToResponseObject(), HttpStatusCode.BadRequest);
        }
    }
}
