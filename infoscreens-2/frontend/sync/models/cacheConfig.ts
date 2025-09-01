import { LocalCache, LocalCacheFileName, Slide } from '../../common';

export class CacheConfig {
    /** Refresh process name referenced in the node config under localCache.refreshRates */
    name: LocalCache;

    /** Slide name referenced in the node config under slides.order and slides.config */
    slide: Slide | undefined;

    /** Suffix for the API backend call */
    apiEndpoint: string;

    /** Cached data file name */
    localCacheFileName: LocalCacheFileName;

    /** Options for specific slides (like randomize array for news) */
    options: CacheOptions;

    /** Refresh rate used if not defined in config */
    defaultRefreshInterval: number;
}

export interface CacheOptions {
    /** To identify the cached data that contains the config used to start other cache refresh processes */
    isNodeConfig?: boolean;

    /** Randomize and merge Sociabble data */
    randomizeSociabble?: boolean;

    /** Randomize data for the 20 min format */
    randomizeTwentyMinNews?: boolean;

    /** Randomize the array of data received */
    randomizeArray?: boolean;
}
