using Infoscreens.Common.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions.Timer_trigger
{
    public class SendNewsPushNotifications
    {
        #region Constructor / Dependency Injection

        private readonly ILogger<SendNewsPushNotifications> _logger;
        private readonly INewsRepository _newsRepository;
        private readonly IDatabaseRepository _databaseRepository;

        public SendNewsPushNotifications(
            ILogger<SendNewsPushNotifications> logger,
            INewsRepository newsRepository,
            IDatabaseRepository databaseRepository
        )
        {
            _logger = logger;
            _newsRepository = newsRepository;
            _databaseRepository = databaseRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "SendNewsPushNotifications";
        [Function(FUNCTION_NAME)]
        // Runs at 07:07 UTC from Monday to Friday (08:07 CET, 09:07 CEST)
        public async Task RunAsync([TimerTrigger("0 7 7 * * 1-5", RunOnStartup = false)]TimerInfo timer)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Timer Function {FUNCTION_NAME}() called."));

                // Get tenants that must notify the users
                var tenants = (await _databaseRepository.GetAllTenantsAsync()).Where(t => t.NotifyUsers);

                foreach(var tenant in tenants)
                {
                    await _newsRepository.NotifyNewsForTenantAsync(tenant);
                }

                _logger.LogDebug(new LogItem(11, $"Timer Function {FUNCTION_NAME}() finished."));
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
            }
        }
    }
}
