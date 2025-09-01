import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TranslationPipe } from '../../pipes/translation.pipe';
import { IndexableCache } from '../../enums/indexableCache';
import { SlideContentType } from '../../enums/slideContentType';
import { CacheService } from '../../services/cache.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, NodeConfig_Sync, Slide } from '../../../../../common';

@Component({
    templateUrl: './localvideo.html',
    styleUrls: ['./localvideo.scss']
})
export class LocalVideoComponent {
    slide: Slide = Slide.LocalVideo;
    localVideoTitle: string;
    localVideoLink: string;

    constructor(private http: HttpClient, private cacheService: CacheService, private translationPipe: TranslationPipe) {}

    ngOnInit() {
        try {
            this.http.get('cache/' + LocalCacheFileName.Config).subscribe((config: NodeConfig_Sync) => {
                var numberOfVideos = config.slides.config['localvideo'].videos.length;
                var video =
                    config.slides.config['localvideo'].videos[this.cacheService.getIndexForCachedElements(IndexableCache.localVideo, numberOfVideos)];

                if (video.title) this.localVideoTitle = video.title;
                else
                    this.translationPipe.getTranslationFromFileAsync('title.localVideo').then(text => {
                        if (!this.localVideoTitle) this.localVideoTitle = text;
                    });

                this.localVideoLink = 'assets/videos/' + video.file;
                LoggingService.debug(LocalVideoComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(LocalVideoComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
