using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Infoscreens.Management.Functions
{
    public class Cors
    {
        #region Constructor / Dependency Injection

        public Cors(ILogger<Cors> logger)
        {}

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "Cors";
        [Function(FUNCTION_NAME)]
        public HttpResponseData Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "options", Route = "v1/{*restOfPath}")] HttpRequestData req, string restOfPath)
        {
            return req.CreateResponse(System.Net.HttpStatusCode.OK);
        }
    }
}
