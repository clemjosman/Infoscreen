using Infoscreens.Common.Enumerations;
using Infoscreens.Common.Models.API.CMS;
using System.Collections.Generic;

namespace Infoscreens.Common.Models.Configs
{
    public class FrontendConfig : IGenericFrontendConfig
    {
        #region IGenericFrontendConfig
        public string Version { get; set; } = null;
        public string Language { get; set; } = null;
        public string Timezone { get; set; } = null;
        public eLogLevel UiLogLevel { get; set; } = eLogLevel.Info;
        public eLogLevel SyncLogLevel { get; set; } = eLogLevel.Info;
        public eLogLevel MsbLogLevel { get; set; } = eLogLevel.Info;
        public bool DisableAnimations { get; set; } = false;
        public eBannerStyle BannerStyle { get; set; } = eBannerStyle.Left;
        public eFooterStyle FooterStyle { get; set; } = eFooterStyle.RollingText;
        public bool InvertDuoBranding { get; set; } = false;
        public eBranding? SecondBranding { get; set; }
        public eDateFormat DateFormat { get; set; } = eDateFormat.Long;
        public eTheme Theme { get; set; } = eTheme.LightDefault;
        public string RollingMessage { get; set; } = "";
        public DeviceSleepConfig SleepConfig { get; set; } = new DeviceSleepConfig(null, null);

        #endregion

        public SlidesConfig Slides { get; set; } = new SlidesConfig(null, null);

        public LocalCacheConfig LocalCache { get; set; } = new LocalCacheConfig();


        // Methods
        public void Update(apiInfoscreen_ConfigUpdate update)
        {
            Language = update.Language;
            Timezone = update.Timezone;
            DisableAnimations = update.DisableAnimations;
            Theme = update.Theme;
            BannerStyle = update.BannerStyle;
            FooterStyle = update.FooterStyle;
            RollingMessage = update.RollingMessage;
            InvertDuoBranding = update.InvertDuoBranding;
            SleepConfig = update.SleepConfig;
            Slides = update.Slides;
            LocalCache = update.LocalCache;
        }
    }

    public interface IGenericFrontendConfig
    {
        public string Version { get; set; }

        public string Language { get; set; }

        public string Timezone { get; set; }

        public eLogLevel UiLogLevel { get; set; }

        public eLogLevel SyncLogLevel { get; set; }

        public eLogLevel MsbLogLevel { get; set; }

        public bool DisableAnimations { get; set; }

        public eBannerStyle BannerStyle { get; set; }
        public eFooterStyle FooterStyle { get; set; }
        public bool InvertDuoBranding { get; set; }
        public eBranding? SecondBranding { get; set; }
        public eDateFormat DateFormat { get; set; }
        public eTheme Theme { get; set; }
    }

    public class LocalCacheConfig
    {
        public Dictionary<eLocalCache, float> RefreshRates = new();
    }
}
