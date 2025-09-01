using Infoscreens.Common;
using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using vesact.common.message.v2;
using vesact.common.message.v2.Interfaces;
using vesact.common.message.v2.Models;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Environment.SetEnvironmentVariable(CommonConfigHelper.StorageConnectionStringName, "none");
        Environment.SetEnvironmentVariable("CMS_CONNECTION_STRING", "");

        var host = CreateHostBuilder(args).Build();
        var messageService = host.Services.GetRequiredService<MessageService<IEmailProvider, FirebaseConfig_V1>>();
        var dbRepository = host.Services.GetRequiredService<IDatabaseRepository>();

        var user = await dbRepository.GetUserByObjectIdAsync("70a4dfec-110a-4ec9-89d0-233b29bd44a6");

        Console.WriteLine($"Sending to {user.DisplayName} on {messageService.GetCurrentPushConfig().ProjectId}");

        var pushRequest = new PushRequest_Fcm_V1(null, "Test push", "Sorry if you received it, please ignore it")
        {
            //Topic = "",
            ReceiversUserIds = new List<string>() { user.Id.ToString() },
            Icon = "fcm_push_icon",
            CustomParameters = new Dictionary<string, string>() { { "action", "NEWS_UPDATE" }, { "title", "Test push" }, { "message", "Sorry if you received it, please ignore it" } } 
        };

        while (true)
        {
            await messageService.SendPushAsync(pushRequest);

        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .ConfigureServices((hostContext, services) =>
           {
               // remove the hosted service
               // services.AddHostedService<Worker>();
               InfoscreensCommonSetup.ApplyDependencyInjection(services);
               // register your services here.
           });
}