using Infoscreens.Common.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Management.Functions.Timer_trigger
{
    public class CleanUpCategories
    {
        #region Constructor / Dependency Injection

        private readonly ILogger<CleanUpCategories> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CleanUpCategories(
            ILogger<CleanUpCategories> logger,
            ICategoryRepository categoryRepository
        )
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        #endregion Constructor / Dependency Injection

        const string FUNCTION_NAME = "CleanUpCategories";
        [Function(FUNCTION_NAME)]
        public async Task RunAsync([TimerTrigger("0 0 0 * * *", RunOnStartup = false)]TimerInfo timer)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, $"Timer Function {FUNCTION_NAME}() called."));

                // Cleaning categories
                await _categoryRepository.CleanUpCategoriesAsync();

                _logger.LogDebug(new LogItem(11, $"Timer Function {FUNCTION_NAME}() finished."));
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogItem(300, exception, FUNCTION_NAME + "() has thrown an exception: {0}", exception.Message));
            }
        }
    }
}
