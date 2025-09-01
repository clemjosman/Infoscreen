using Infoscreens.Common.Helpers;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public class VideoListNotFoundCustomException : CustomExceptionBaseClass
    {
        public override string ExceptionMessageLabel { get; } = "error.videoListNotFound";
        public override List<string> ExceptionMessageParameters { get; set; } = new List<string>();

        public VideoListNotFoundCustomException(int numberOfVideosRequested, string message = null) : base(message ?? $"Not all {numberOfVideosRequested} videos could be found.")
        { }

        public override async Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req)
        {
            return await HttpResponseHelper.JsonResponseAsync(req, ToResponseObject(), HttpStatusCode.BadRequest);
        }
    }
}
