using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.EntityFramework.CMS;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using vesact.common.Log;

namespace Infoscreens.Common.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly IDatabaseRepository _databaseRepository;

        public UserRepository(ILogger<UserRepository> logger, IDatabaseRepository databaseRepository)
        {
            logger.LogDebug(new LogItem(1, "UserRepository() Creating a new instance."));

            _logger = logger;
            _databaseRepository = databaseRepository;

            _logger.LogTrace(new LogItem(2, "UserRepository() New instance has been created."));
        }


        #region Update

        public async Task<User> UpdateSelectedTenantAsync(User user, apiUser_UpdateSelectedTenant newSelectedTenant)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "UpdateSelectedTenantAsync() called.")
                {
                    Custom1 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom4 = newSelectedTenant != null ? JsonConvert.SerializeObject(newSelectedTenant) : "Parameter newSelectedTenant is null."
                });

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (newSelectedTenant == null)
                    throw new ArgumentNullException(nameof(newSelectedTenant));


                // Update fields
                user.SelectedTenantId = newSelectedTenant.NewSelectedTenantId;

                // Persist modifications
                await _databaseRepository.UpdateUserAsync(user);


                _logger.LogDebug(new LogItem(11, "UpdateSelectedTenantAsync() finished.")
                {
                    Custom1 = user != null ? user.ToString() : "Updated user is null.",
                    Custom4 = newSelectedTenant != null ? JsonConvert.SerializeObject(newSelectedTenant) : "Parameter newSelectedTenant is null."
                });

                // Return modified infoscreen
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "UpdateSelectedTenantAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        public async Task<User> UpdateSelectedLanguageAsync(User user, apiUser_UpdateSelectedLanguage newSelectedLanguage)
        {
            try
            {
                _logger.LogDebug(new LogItem(10, "UpdateSelectedLanguageAsync() called.")
                {
                    Custom1 = user != null ? user.ToString() : "Parameter user is null.",
                    Custom4 = newSelectedLanguage != null ? JsonConvert.SerializeObject(newSelectedLanguage) : "Parameter newSelectedLanguage is null."
                });

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (newSelectedLanguage == null)
                    throw new ArgumentNullException(nameof(newSelectedLanguage));


                // Update fields
                user.Iso2 = newSelectedLanguage.NewSelectedIso2;

                // Persist modifications
                await _databaseRepository.UpdateUserAsync(user);


                _logger.LogDebug(new LogItem(11, "UpdateSelectedLanguageAsync() finished.")
                {
                    Custom1 = user != null ? user.ToString() : "Updated user is null.",
                    Custom4 = newSelectedLanguage != null ? JsonConvert.SerializeObject(newSelectedLanguage) : "Parameter newSelectedLanguage is null."
                });

                // Return modified infoscreen
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogItem(300, ex, "UpdateSelectedLanguageAsync() has thrown an exception: {0}", ex.Message));
                throw;
            }
        }

        #endregion Update

    }
}
