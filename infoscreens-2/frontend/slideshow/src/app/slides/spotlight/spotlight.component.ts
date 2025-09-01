import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IndexableCache } from '../../enums/indexableCache';
import { CacheService } from '../../services/cache.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, NodeConfig_Sync, Slide } from '../../../../../common';

@Component({
    templateUrl: '../iframe/iframe.html',
    styleUrls: ['../iframe/iframe.scss']
})
export class SpotlightComponent {
    slide: Slide = Slide.Spotlight;
    bannerTitle: string = undefined;
    iframeSource: string;

    constructor(private http: HttpClient, private cacheService: CacheService) {
        try {
            this.http.get('cache/' + LocalCacheFileName.Config).subscribe((config: NodeConfig_Sync) => {
                var pages = config.slides.config['spotlight'].pages;
                this.iframeSource = pages[this.cacheService.getIndexForCachedElements(IndexableCache.spotlight, pages.length)].url;

                LoggingService.debug(SpotlightComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(SpotlightComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
