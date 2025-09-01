using Infoscreens.Common.Exceptions;
using Infoscreens.Common.Interfaces;
using Infoscreens.Common.Models.EntityFramework.CMS;
using System.Threading.Tasks;

namespace Infoscreens.Management.Helpers
{
    public class PermissionHelper
    {
        public static async Task<bool> HasUserAccessToTenantAsync(IDatabaseRepository _databaseRepository, User user, Tenant tenant)
        {
            return (await _databaseRepository.GetUserTenantAsync(user.Id, tenant.Id)) != null;
        }

        public static async Task EnsureHasAccessToTenantAsync(IDatabaseRepository _databaseRepository, User user, Tenant tenant)
        {
            if(! await HasUserAccessToTenantAsync(_databaseRepository, user, tenant))
            {
                throw new UnauthorizedTenantAccessCustomException(user, tenant);
            }
        }
    }
}
