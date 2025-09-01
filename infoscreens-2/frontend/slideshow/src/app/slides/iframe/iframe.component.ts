import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IndexableCache } from '../../enums/indexableCache';
import { CacheService } from '../../services/cache.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, NodeConfig, NodeConfig_Sync, Slide } from '../../../../../common';

@Component({
    templateUrl: './iframe.html',
    styleUrls: ['./iframe.scss']
})
export class IframeComponent {
    slide: Slide = Slide.Iframe;
    iframeSource: string;
    bannerTitle: string = undefined;

    constructor(private http: HttpClient, private cacheService: CacheService) {
        try {
            this.http.get('cache/' + LocalCacheFileName.Config).subscribe((config: NodeConfig_Sync) => {
                var pages = config.slides.config['iframe'].pages;
                var page = pages[this.cacheService.getIndexForCachedElements(IndexableCache.iframe, pages.length)];
                this.iframeSource = page.url;
                this.bannerTitle = page.bannerTitle;
                LoggingService.debug(IframeComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(IframeComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
