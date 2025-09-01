using System.Threading.Tasks;

namespace Infoscreens.Common.Interfaces
{
    public partial interface ICategoryRepository
    {
        Task CleanUpCategoriesAsync();
    }
}
