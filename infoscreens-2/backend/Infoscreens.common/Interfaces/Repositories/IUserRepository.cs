using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.EntityFramework.CMS;
using System.Threading.Tasks;

namespace Infoscreens.Common.Interfaces
{
    public interface IUserRepository
    {
        #region Update

        Task<User> UpdateSelectedTenantAsync(User user, apiUser_UpdateSelectedTenant newSelectedTenant);

        Task<User> UpdateSelectedLanguageAsync(User user, apiUser_UpdateSelectedLanguage newSelectedLanguage);

        #endregion Update
    }
}
