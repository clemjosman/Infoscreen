using Infoscreens.Common;
using Infoscreens.Common.Interfaces;
using Infoscreens.Management.Functions;
using Infoscreens.Management.Helpers;
using InfoscreensConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;
using vesact.common.file.Interfaces;
using vesact.common.message.v2;
using vesact.common.message.v2.Interfaces;
using vesact.common.message.v2.Models;
using Infoscreens.Common.Helpers;
using Infoscreens.Cache;
using Infoscreens.Cache.Functions;

var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

Environment.SetEnvironmentVariable(CommonConfigHelper.StorageConnectionStringName, config.GetValue<string>(CommonConfigHelper.StorageConnectionStringName));
Environment.SetEnvironmentVariable("CMS_CONNECTION_STRING", config.GetValue<string>("CMS_CONNECTION_STRING"));
Environment.SetEnvironmentVariable("ENVIRONMENT", config.GetValue<string>("ENVIRONMENT"));
Environment.SetEnvironmentVariable("DEV_OBJECT_ID", config.GetValue<string>("DEV_OBJECT_ID"));

var host = Host.CreateDefaultBuilder(args)
           .ConfigureServices((hostContext, services) =>
           {
               // remove the hosted service
               // services.AddHostedService<Worker>();
               InfoscreensCommonSetup.ApplyDependencyInjection(services);
               // register your services here.
           })
           .Build();



var messageService = host.Services.GetRequiredService<MessageService<IEmailProvider, FirebaseConfig_V1>>();
var dbRepository = host.Services.GetRequiredService<IDatabaseRepository>();
//var exceptionHelper = host.Services.GetRequiredService<IExceptionHelper>();
//var fileHelper = host.Services.GetRequiredService<IFileHelper>();
//var subRepository = host.Services.GetRequiredService<ISubscriptionRepository>();

// Create loggers
ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
}));

//var getNewsApp = new GetNews_App(loggerFactory.CreateLogger<BaseApiClass>(), dbRepository, exceptionHelper, fileHelper);
//var subscribeApp = new Subscribe_App(loggerFactory.CreateLogger<BaseApiClass>(), dbRepository, exceptionHelper, messageService, subRepository);
//var sgCache = new UpdateAllJobOffersCache(loggerFactory.CreateLogger<UpdateAllJobOffersCache>());

//var body = new MemoryStream(Encoding.ASCII.GetBytes("{\"userId\": 12}"));
//var mockUrl = "http://mockhost:7071/api/v1/mock?after=2024-01-01&before=2024-12-01";
var body = new MemoryStream(Encoding.ASCII.GetBytes("{\"userId\": 12}"));
var mockUrl = "http://mockhost:7071/api/v1/mock";

var context = new FunctionContextMock();


var requestdata = new HttpRequestDataMock(context, new(mockUrl), body);

//await getNewsApp.RunAsync(requestdata);
//await subscribeApp.RunAsync(requestdata);
//await sgCache.RunTimerAsync(new Microsoft.Azure.Functions.Worker.TimerInfo() { IsPastDue = true });

