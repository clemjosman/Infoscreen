using Infoscreens.Common.Helpers;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public class AuthCustomException : CustomExceptionBaseClass
    {
        public override string ExceptionMessageLabel { get; } = "error.auth";
        public override List<string> ExceptionMessageParameters { get; set; } = new List<string>();

        public AuthCustomException(string details, string message = null) : base(message ?? $"An error occured during the authentication: {details}.")
        {}

        public override async Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req)
        {
            return await HttpResponseHelper.JsonResponseAsync(req, ToResponseObject(), HttpStatusCode.BadRequest);
        }
    }
}
