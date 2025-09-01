using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoscreens.Common.Exceptions
{
    public abstract class CustomExceptionBaseClass : Exception, ICustomException
    {
        public abstract string ExceptionMessageLabel { get; }
        public abstract List<string> ExceptionMessageParameters { get; set; }

        public abstract Task<HttpResponseData> ToApiResponseAsync(HttpRequestData req);

        public CustomExceptionBaseClass(string message): base(message)
        {}

        protected apiCustomException ToResponseObject()
        {
            return new apiCustomException(ExceptionMessageLabel, ExceptionMessageParameters);
        }
    }
}
