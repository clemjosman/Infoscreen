import { Branding } from "../enums/brandings";
import { BannerStyle } from "../enums/bannerStyle";
import { DateFormat } from "../enums/dateFormat";
import { FooterStyle } from "../enums/footerStyle";
import { LocalCache } from "../enums/localCache";
import { LogLevelStrings } from "../enums/logLevel";
import { SlideshowLanguage } from "../enums/slideshowLanguage";
import { Theme } from "../enums/theme";
import { DeviceSleepConfig } from "./deviceSleepConfig";
import { SlidesConfig } from "./slideConfig";

export interface FrontendConfig extends GenericFrontendConfig {
  slides: SlidesConfig;
  localCache: LocalCacheConfig;
}

export interface GenericFrontendConfig {
  version: string;
  language: SlideshowLanguage;
  timezone: string;
  uiLogLevel: LogLevelStrings;
  syncLogLevel: LogLevelStrings;
  msbLogLevel: LogLevelStrings;
  disableAnimations: boolean;
  rollingMessage: string;
  sleepConfig: DeviceSleepConfig;
  bannerStyle: BannerStyle;
  footerStyle: FooterStyle;
  invertDuoBranding: boolean;
  secondBranding?: Branding | null;
  dateFormat: DateFormat;
  theme: Theme;
}

export interface LocalCacheConfig {
  refreshRates: { [localCache in LocalCache]: number };
}
