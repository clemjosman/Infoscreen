using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InfoscreensConsoleApp
{
    public class HttpRequestDataMock : HttpRequestData
    {
        public HttpRequestDataMock(FunctionContext functionContext, Uri url, Stream body) : base(functionContext)
        {
            Url = url;
            Body = body;
        }

        public override Stream Body { get; }

        public override HttpHeadersCollection Headers => [];

        public override IReadOnlyCollection<IHttpCookie> Cookies { get; }

        public override Uri Url { get; }

        public override IEnumerable<ClaimsIdentity> Identities { get; }

        public override string Method { get; }

        public override HttpResponseData CreateResponse()
        {
            return new HttpResponseDataMock(new FunctionContextMock());
        }
    }

    public class HttpResponseDataMock : HttpResponseData
    {
        public HttpResponseDataMock(FunctionContext functionContext) : base(functionContext)
        {
        }

        public override HttpStatusCode StatusCode { get; set; }
        public override HttpHeadersCollection Headers { get; set; } = new HttpHeadersCollection();
        public override Stream Body { get; set; } = new MemoryStream();
        public override HttpCookies Cookies { get; }
    }

    public class FunctionContextMock : FunctionContext
    {
        public override string InvocationId => "invocationId";

        public override string FunctionId => "functionId";

        public override TraceContext TraceContext { get; }

        public override BindingContext BindingContext { get; }

        public override RetryContext RetryContext { get; }

        public override IServiceProvider InstanceServices { get; set; }

        public override FunctionDefinition FunctionDefinition { get; }

        public override IDictionary<object, object> Items { get; set; }

        public override IInvocationFeatures Features { get; }
    }
}
