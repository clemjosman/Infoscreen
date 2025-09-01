using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Interfaces;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Common.Helpers
{
    public class ExceptionHelper : IExceptionHelper
    {
        private readonly ILogger<ExceptionHelper> _logger;

        public ExceptionHelper(ILogger<ExceptionHelper> logger)
        {
            _logger = logger;
        }


        public void HandleException(string currentMethodeName, Exception exception)
        {
            _logger.LogError(new LogItem(300, exception, currentMethodeName + "() has thrown an exception: {0}", exception.Message));
        }

        public void HandleCustomException(string currentMethodeName, CustomExceptionBaseClass customException)
        {
            _logger.LogError(new LogItem(300, customException, currentMethodeName+"() has thrown an exception: {0}", customException.Message));
        }

        public Task<HttpResponseData> ExceptionToResponseAsync(HttpRequestData req, Exception exception)
        {
            return HttpResponseHelper.TextResponseAsync(req, exception.Message, HttpStatusCode.InternalServerError);
        }
    }
}
