using Infoscreens.Common.Helpers;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public class MissingOrBadQueryParameterCustomException : CustomExceptionBaseClass
    {
        public override string ExceptionMessageLabel { get; } = "error.missingOrBadQueryParameter";
        public override List<string> ExceptionMessageParameters { get; set; } = new List<string>();

        public MissingOrBadQueryParameterCustomException(string message = null) : base(message ?? $"The query parameters received request does not match the required one or are not valid.")
        {}

        public override async Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req)
        {
            return await HttpResponseHelper.JsonResponseAsync(req, ToResponseObject(), HttpStatusCode.BadRequest);
        }
    }
}
