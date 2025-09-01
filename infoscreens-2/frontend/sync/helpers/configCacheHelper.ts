import { CacheConfig } from 'models/cacheConfig';
import { LocalCache, LocalCacheFileName } from '../../common';
import { NodeService } from '../services/nodeService';

export class ConfigCacheHelper {
    public static readonly CONFIG_REFRESH_INTERVAL: number = 0.5;
    public static readonly ROOT_URL: string = NodeService.getAzureFunctionEndpoint() + '/api/v1/';
    public static readonly CACHE: CacheConfig = {
        name: LocalCache.Config,
        slide: undefined,
        apiEndpoint: 'infoscreen',
        localCacheFileName: LocalCacheFileName.Config,
        options: { isNodeConfig: true },
        defaultRefreshInterval: ConfigCacheHelper.CONFIG_REFRESH_INTERVAL
    };
}
