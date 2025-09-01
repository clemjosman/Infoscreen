import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, NodeConfig_Sync, Slide } from '../../../../../common';

@Component({
    templateUrl: '../iframe/iframe.html',
    styleUrls: ['../iframe/iframe.scss']
})
export class UniversityOverviewComponent {
    slide: Slide = Slide.University;
    bannerTitle: string = undefined;
    iframeSource: string;

    constructor(private http: HttpClient) {
        try {
            this.http.get('cache/' + LocalCacheFileName.Config).subscribe((config: NodeConfig_Sync) => {
                this.iframeSource = config.slides.config.universityoverview.page.url;
                LoggingService.debug(UniversityOverviewComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(UniversityOverviewComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
