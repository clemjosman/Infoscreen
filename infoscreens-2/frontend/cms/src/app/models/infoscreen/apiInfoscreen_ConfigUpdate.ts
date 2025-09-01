import { BannerStyle, DataEndpointConfig, DeviceSleepConfig, FooterStyle, LocalCacheConfig, SlidesConfig, Theme } from '../../../../../common';

export interface apiInfoscreen_ConfigUpdate {
    // Frontend related
    language: string;
    timezone: string;
    disableAnimations: boolean;
    theme: Theme;
    bannerStyle: BannerStyle;
    footerStyle: FooterStyle;
    rollingMessage: string;
    invertDuoBranding: boolean;
    sleepConfig: DeviceSleepConfig;
    slides: SlidesConfig;
    localCache: LocalCacheConfig;

    // Backend related
    dataEndpointConfig: DataEndpointConfig;
}
