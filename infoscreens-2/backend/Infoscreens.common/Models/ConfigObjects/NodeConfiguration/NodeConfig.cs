using Infoscreens.Common.Models.API.CMS;

namespace Infoscreens.Common.Models.Configs
{
    public class NodeConfig
    {
        public string NodeId { get; set; } = null;

        public string FirmwareVersion { get; set; } = null;

        public FrontendConfig FrontendConfig { get; set; } = new FrontendConfig();

        public BackendConfig BackendConfig { get; set; } = new BackendConfig();

        // Methods
        public void Update(apiInfoscreen_ConfigUpdate update)
        {
            FrontendConfig.Update(update);
            BackendConfig.Update(update);
        }
    }
}
