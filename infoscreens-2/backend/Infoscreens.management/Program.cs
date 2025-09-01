using Infoscreens.Common;
using Infoscreens.Common.Middlewares;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker => {
        worker.UseNewtonsoftJson();
        worker.UseMiddleware<CorsOptionsMiddleware>();
    })
    .ConfigureOpenApi()
    .ConfigureServices(services =>
    {
        InfoscreensCommonSetup.ApplyDependencyInjection(services);
    })
    .Build();

host.Run();