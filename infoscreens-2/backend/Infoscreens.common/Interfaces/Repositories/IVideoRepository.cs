using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.EntityFramework.CMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoscreens.Common.Interfaces
{
    public interface IVideoRepository
    {
        #region Create or Update

        Task<Video> CreateOrUpdateVideoAsync(apiVideo_Publish publishedVideo, Tenant tenant, User user);

        #endregion Create or Update

        #region Delete

        Task DeleteVideoAsync(Video video);
        Task DeleteVideoAsync(List<Video> video);

        #endregion Delete

        #region Translate

        Task<apiVideo_Translated> TranslateVideoAsync(apiVideo_Translate apiVideo_Translate);

        #endregion Translate
    }
}
