import { Slide } from "../enums/slide";
import { SlideshowLanguage } from "../enums/slideshowLanguage";
import { VideoBackground } from "../enums/videoBackground";

export interface SlidesConfig {
  order: Slide[];
  config: SlideConfigWrapper;
}

export interface SlideConfigWrapper {
  [Slide.Coffee]?: SimpleSlideConfig;
  [Slide.CustomJobOffer]?: SimpleSlideConfig;
  [Slide.Ideabox]?: SimpleSlideConfig;
  [Slide.Iframe]?: IframeSlideConfig;
  [Slide.InfoscreenMonitoring]?: SimpleSlideConfig;
  [Slide.JobOffers]?: SimpleSlideConfig;
  [Slide.LocalVideo]?: LocalVideoSlideConfig;
  [Slide.Maintenance]?: SimpleSlideConfig;
  [Slide.MonitoringIframe]?: IframeSlideConfig;
  [Slide.NewsInternal]?: SimpleSlideConfig;
  [Slide.NewsPublic]?: SimpleSlideConfig;
  [Slide.PublicTransport]?: SimpleSlideConfig;
  [Slide.Sociabble]?: SociabbleSlideConfig;
  [Slide.Spotlight]?: SpotlightSlideConfig;
  [Slide.Stock]?: StockSlideConfig;
  [Slide.Traffic]?: TrafficSlideConfig;
  [Slide.TwentyMin]?: SimpleSlideConfig;
  [Slide.Twitter]?: SimpleSlideConfig;
  [Slide.University]?: SimpleSlideConfig;
  [Slide.UniversityOverview]?: UniversityOverviewSlideConfig;
  [Slide.WeatherDaily]?: SimpleSlideConfig;
  [Slide.WeatherWeekly]?: SimpleSlideConfig;
  [Slide.Youtube]?: YoutubeSlideConfig;
}

export interface SimpleSlideConfig {
  duration: number;
}

export interface IframeSlideConfig {
  pages: {
    url: string;
    duration: number;
    bannerTitle: string;
  }[];
}

export interface SociabbleSlideConfig {
  duration: number;
  fileNames: [string,string,string];
}

export interface LocalVideoSlideConfig {
  videos: {
    file: string;
    duration: number;
    title: string;
  }[];
}

export interface SpotlightSlideConfig {
  pages: {
    url: string;
    duration: number;
  }[];
}

export interface StockSlideConfig extends SimpleSlideConfig {
  stock: {
    url: string;
  };
  exchange: {
    url: string;
  };
}

export interface TrafficSlideConfig extends SimpleSlideConfig {
  gmap: {
    longitude: number;
    latitude: number;
    zoom: number;
    apiKey: string;
  };
}

export interface UniversityOverviewSlideConfig extends SimpleSlideConfig {
  page: {
    url: string;
  };
}

export interface YoutubeVideoConfig {
  url: string;
  embedUrl: string;
  duration: number;
  background: VideoBackground;
  title: { [key in SlideshowLanguage]?: string };
}

/** Keeping the YouTube links in the slides config as it also contains the slide duration */
export interface YoutubeSlideConfig {
  videos: YoutubeVideoConfig[];
}
