using Infoscreens.Common.Models.API.CMS;
using Infoscreens.Common.Models.Configs;
using Infoscreens.Common.Models.EntityFramework.CMS;
using System.Threading.Tasks;

namespace Infoscreens.Common.Interfaces
{
    public interface IInfoscreenRepository
    {
        #region Update

        Task<Infoscreen> UpdateInfoscreenMetadataAsync(Infoscreen infoscreen, apiInfoscreen_MetaDataUpdate metaDataUpdate);
        Task<NodeConfig> UpdateInfoscreenConfigAsync(Infoscreen infoscreen, apiInfoscreen_ConfigUpdate metaDataUpdate);

        #endregion Update

        #region Management

        Task TriggerNeededFirmwareUpdates();

        #endregion Management
    }
}
