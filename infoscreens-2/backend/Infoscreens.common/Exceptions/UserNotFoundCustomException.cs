using Infoscreens.Common.Helpers;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public class UserNotFoundCustomException : CustomExceptionBaseClass
    {
        public override string ExceptionMessageLabel { get; } = "error.userNotFound";
        public override List<string> ExceptionMessageParameters { get; set; } = new List<string>();

        public UserNotFoundCustomException(string userId, string message = null) : base(message ?? $"User with id #{userId} was not found.")
        {}

        public override async Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req)
        {
            return await HttpResponseHelper.JsonResponseAsync(req, ToResponseObject(), HttpStatusCode.BadRequest);
        }
    }
}
