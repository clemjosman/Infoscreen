using Infoscreens.Common.Exceptions;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Threading.Tasks;

namespace Infoscreens.Common.Interfaces
{
    public interface IExceptionHelper
    {
        void HandleException(string currentMethodeName, Exception exception);

        void HandleCustomException(string currentMethodeName, CustomExceptionBaseClass customException);

        Task<HttpResponseData> ExceptionToResponseAsync(HttpRequestData req, Exception exception);
    }
}
