import { CacheConfig } from 'models/cacheConfig';
import { LocalCache, LocalCacheFileName, Slide } from '../../common';
import { NodeService } from '../services/nodeService';

export class AzureFunctionsCachesHelper {
    public static readonly ROOT_URL: string = NodeService.getAzureFunctionEndpoint() + '/api/v1/';

    public static readonly CACHES: CacheConfig[] = [
        {
            name: LocalCache.CustomJobOffer,
            slide: Slide.CustomJobOffer,
            apiEndpoint: 'customJobOffer',
            localCacheFileName: LocalCacheFileName.CustomJobOffer,
            options: null,
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.Ideabox,
            slide: Slide.Ideabox,
            apiEndpoint: 'ideabox',
            localCacheFileName: LocalCacheFileName.Ideabox,
            options: null,
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.InfoscreenMonitoring,
            slide: Slide.InfoscreenMonitoring,
            apiEndpoint: 'infoscreensState',
            localCacheFileName: LocalCacheFileName.InfoscreenMonitoring,
            options: { randomizeArray: true },
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.JobOffers,
            slide: Slide.JobOffers,
            apiEndpoint: 'jobOffers',
            localCacheFileName: LocalCacheFileName.JobOffers,
            options: null,
            defaultRefreshInterval: 20
        },
        {
            name: LocalCache.NewsInternal,
            slide: Slide.NewsInternal,
            apiEndpoint: 'newsinternal',
            localCacheFileName: LocalCacheFileName.NewsInternal,
            options: null,
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.NewsPublic,
            slide: Slide.NewsPublic,
            apiEndpoint: 'newspublic',
            localCacheFileName: LocalCacheFileName.NewsPublic,
            options: null,
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.OpenWeather,
            slide: Slide.WeatherDaily,
            apiEndpoint: 'openweather',
            localCacheFileName: LocalCacheFileName.OpenWeather,
            options: null,
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.OpenWeather,
            slide: Slide.WeatherWeekly,
            apiEndpoint: 'openweather',
            localCacheFileName: LocalCacheFileName.OpenWeather,
            options: null,
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.PublicTransport,
            slide: Slide.PublicTransport,
            apiEndpoint: 'publictransport',
            localCacheFileName: LocalCacheFileName.PublicTransport,
            options: null,
            defaultRefreshInterval: 20
        },
        {
            name: LocalCache.Sociabble,
            slide: Slide.Sociabble,
            apiEndpoint: 'sociabble',
            localCacheFileName: LocalCacheFileName.Sociabble,
            options: { randomizeSociabble: true },
            defaultRefreshInterval: 20
        },
        {
            name: LocalCache.TwentyMin,
            slide: Slide.TwentyMin,
            apiEndpoint: 'twentyMin',
            localCacheFileName: LocalCacheFileName.TwentyMin,
            options: { randomizeTwentyMinNews: true },
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.Twitter,
            slide: Slide.Twitter,
            apiEndpoint: 'twitter',
            localCacheFileName: LocalCacheFileName.Twitter,
            options: { randomizeArray: true },
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.University,
            slide: Slide.University,
            apiEndpoint: 'university',
            localCacheFileName: LocalCacheFileName.University,
            options: null,
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.UptownArticle,
            slide: Slide.UptownArticle,
            apiEndpoint: 'uptownarticle',
            localCacheFileName: LocalCacheFileName.UptownArticle,
            options: null,
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.UptownEvent,
            slide: Slide.UptownEvent,
            apiEndpoint: 'uptownevent',
            localCacheFileName: LocalCacheFileName.UptownEvent,
            options: null,
            defaultRefreshInterval: 45
        },
        {
            name: LocalCache.UptownMenu,
            slide: Slide.UptownMenu,
            apiEndpoint: 'uptownmenu',
            localCacheFileName: LocalCacheFileName.UptownMenu,
            options: null,
            defaultRefreshInterval: 45
        }
    ];
}
