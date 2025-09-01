using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Threading.Tasks;

namespace Infoscreens.Common.Middlewares
{
    public class CorsOptionsMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var httpRequest = await context.GetHttpRequestDataAsync();

            if (httpRequest != null && httpRequest.Method == "OPTIONS")
            {
                var response = httpRequest.CreateResponse(System.Net.HttpStatusCode.OK);
                // Add CORS headers manually if needed:
                SetCORSHeaders(response);

                context.GetInvocationResult().Value = response;
                return; // Short-circuit the pipeline; don't invoke function
            }

            await next(context);
            var httpResponse = context.GetHttpResponseData();
            if (httpResponse != null)
            {
                SetCORSHeaders(httpResponse);
            }
        }

        private HttpResponseData SetCORSHeaders(HttpResponseData response) {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PATCH,PUT,DELETE,OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type,Authorization");
            return response;
        }
    }

}


