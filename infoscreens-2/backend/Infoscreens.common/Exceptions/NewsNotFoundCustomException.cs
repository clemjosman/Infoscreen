using Infoscreens.Common.Helpers;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public class NewsNotFoundCustomException : CustomExceptionBaseClass
    {
        public override string ExceptionMessageLabel { get; } = "error.newsNotFound";
        public override List<string> ExceptionMessageParameters { get; set; } = new List<string>();

        public NewsNotFoundCustomException(int newsId, string message = null) : base(message ?? $"News with id {newsId} was not found.")
        {}

        public override async Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req)
        {
            return await HttpResponseHelper.JsonResponseAsync(req, ToResponseObject(), HttpStatusCode.BadRequest);
        }
    }
}
