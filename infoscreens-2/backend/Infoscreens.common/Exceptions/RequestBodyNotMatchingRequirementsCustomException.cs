using Infoscreens.Common.Helpers;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public class RequestBodyNotMatchingRequirementsCustomException : CustomExceptionBaseClass
    {
        public override string ExceptionMessageLabel { get; } = "error.requestBodyNotMatching";
        public override List<string> ExceptionMessageParameters { get; set; } = new List<string>();

        public RequestBodyNotMatchingRequirementsCustomException(string message = null) : base(message ?? $"The received request body does not match the required one.")
        {}

        public override async Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req)
        {
            return await HttpResponseHelper.JsonResponseAsync(req, ToResponseObject(), HttpStatusCode.BadRequest);
        }
    }
}
