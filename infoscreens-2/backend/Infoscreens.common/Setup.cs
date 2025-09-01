using Infoscreens.Common.Helpers;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Repositories;
using Microsoft.Extensions.DependencyInjection;
using vesact.common.file;
using vesact.common.message.v2;

namespace Infoscreens.Common
{
    public static class InfoscreensCommonSetup
    {
        public static void ApplyDependencyInjection(IServiceCollection serviceCollection)
        {
            // Registering helpers
            serviceCollection.AddSingleton<IExceptionHelper, ExceptionHelper>();
            serviceCollection.AddSingleton<ILabelTranslationHelper, LabelTranslationHelper>();

            // File service
            FileServiceSetup.ApplyDependencyInjection(serviceCollection, CommonConfigHelper.GetFileServiceConfig);

            // Message service
            serviceCollection.AddMessageService(CommonConfigHelper.GetTenantMessageServiceConfig(CommonConfigHelper.ActemiumTenantCode));

            // Registering repositories
            serviceCollection.AddSingleton<ICategoryRepository, CategoryRepository>();
            serviceCollection.AddSingleton<IDatabaseRepository, DatabaseRepository>();
            serviceCollection.AddSingleton<IInfoscreenRepository, InfoscreenRepository>();
            serviceCollection.AddSingleton<INewsRepository, NewsRepository>();
            serviceCollection.AddSingleton<ISubscriptionRepository, SubscriptionRepository>();
            serviceCollection.AddSingleton<ITranslationRepository, TranslationRepository>();
            serviceCollection.AddSingleton<IUserRepository, UserRepository>();
            serviceCollection.AddSingleton<IVideoRepository, VideoRepository>();

            // Adding loggin
            serviceCollection.AddLogging();
        }
    }
}
