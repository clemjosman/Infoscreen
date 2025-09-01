import { LocalCache, Slide } from '../../common';

export class CacheRefreshProcessesConfig {
    name: LocalCache;
    slide: Slide;
    nodeInterval: NodeJS.Timeout;
    interval_minutes: number;
}
