import { Api } from "../enums/api";

export interface BackendConfig {
  dataEndpointConfig: DataEndpointConfig;
}

export interface DataEndpointConfig {
  [Api.customJobOffer]?: CachedFilesEndpointConfig;
  [Api.ideabox]?: CachedFilesEndpointConfig;
  // Api.iotHub isn't used directly by the device - ignored here
  [Api.joboffers]?: CachedFilesEndpointConfig;
  [Api.microservicebus]?: CachedFilesEndpointConfig;
  [Api.newsInternal]?: NewsInternalApiCacheConfig;
  [Api.newsPublic]?: NewsPublicApiCacheConfig;
  [Api.openWeather]?: CachedFilesEndpointConfig;
  [Api.publicTransport]?: CachedFilesEndpointConfig;
  [Api.sociabble]?: CachedFilesEndpointConfig;
  [Api.twentyMin]?: TwentyMinApiCacheConfig;
  [Api.twitter]?: CachedFilesEndpointConfig;
  [Api.university]?: CachedFilesEndpointConfig;
  [Api.uptownArticle]?: CachedFilesEndpointConfig;
  [Api.uptownEvent]?: CachedFilesEndpointConfig;
  // Api.uptownMenu doesn't provide specific configuration of selection between multiple files
}

export interface CachedFilesEndpointConfig {
  cachedFileName: string | null;
  cachedFileNames: string[] | null;
}

export interface LimitNewsDateAge {
  maxNewsDateAge: number;
}

export interface LimitNewsCount {
  maxNewsCount: number;
}

export interface TwentyMinApiCacheConfig
  extends CachedFilesEndpointConfig,
    LimitNewsDateAge {}

export interface NewsPublicApiCacheConfig
  extends CachedFilesEndpointConfig,
    LimitNewsCount {}

export interface NewsInternalApiCacheConfig extends LimitNewsCount {}
