using Infoscreens.Common.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions.Timer_trigger
{
    public class TriggerNeededFirmwareUpdates
    {
        #region Constructor / Dependency Injection

        private readonly ILogger<TriggerNeededFirmwareUpdates> _logger;
        private readonly IInfoscreenRepository _infoscreenRepository;

        public TriggerNeededFirmwareUpdates(
            ILogger<TriggerNeededFirmwareUpdates> logger,
            IInfoscreenRepository infoscreenRepository
        )
        {
            _logger = logger;
            _infoscreenRepository = infoscreenRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "TriggerNeededFirmwareUpdates";
        [Function(FUNCTION_NAME)]
        // Runs at 09:00 UTC from Monday to Friday
        public async Task RunAsync([TimerTrigger("0 0 9 * * 1-5", RunOnStartup = false)]TimerInfo timer)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Timer Function {FUNCTION_NAME}() called."));

                await _infoscreenRepository.TriggerNeededFirmwareUpdates();

                _logger.LogDebug(new LogItem(11, $"Timer Function {FUNCTION_NAME}() finished."));
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
            }
        }
    }
}
