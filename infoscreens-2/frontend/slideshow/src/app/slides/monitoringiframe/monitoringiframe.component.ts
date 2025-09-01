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
export class MonitoringIframeComponent {
    slide: Slide = Slide.MonitoringIframe;
    bannerTitle: string = undefined;
    iframeSource: string = '';

    constructor(private http: HttpClient, private cacheService: CacheService) {
        try {
            this.http.get('cache/' + LocalCacheFileName.Config).subscribe((config: NodeConfig_Sync) => {
                this.iframeSource =
                    config.slides.config.monitoringiframe.pages[
                        this.cacheService.getIndexForCachedElements(
                            IndexableCache.monitoringiframe,
                            config.slides.config.monitoringiframe.pages.length
                        )
                    ].url;

                LoggingService.debug(MonitoringIframeComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(MonitoringIframeComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
