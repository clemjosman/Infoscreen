using Infoscreens.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Common.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ILogger<CategoryRepository> _logger;
        private readonly IDatabaseRepository _databaseRepository;

        public CategoryRepository(ILogger<CategoryRepository> logger, IDatabaseRepository databaseRepository)
        {
            logger.LogDebug(new LogItem(1, "CategoryRepository() Creating a new instance."));

            _logger = logger;
            _databaseRepository = databaseRepository;

            _logger.LogTrace(new LogItem(2, "CategoryRepository() New instance has been created."));
        }

        public async Task CleanUpCategoriesAsync()
        {
            _logger.LogDebug(new LogItem(10, "CleanUpCategoriesAsync() called."));

            try
            {
                var unassociatedCategories = await _databaseRepository.GetUnassociatedCategoriesAsync();
                await _databaseRepository.DeleteCategoriesAsync(unassociatedCategories);

                _logger.LogDebug(new LogItem(11, "CleanUpCategoriesAsync() finished.")
                {
                    Custom1 = $"Deleted {unassociatedCategories.Count} categories."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "CleanUpCategoriesAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }
    }
}
