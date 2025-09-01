import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TranslationPipe } from '../../pipes/translation.pipe';
import { IndexableCache } from '../../enums/indexableCache';
import { CacheService } from '../../services/cache.service';
import { LoggingService } from '../../services/logging.service';
import { TranslationService } from '../../services/translation.service';
import { LocalCacheFileName, NodeConfig_Sync, Slide, VideoBackground } from '../../../../../common';

@Component({
    templateUrl: './youtube.html',
    styleUrls: ['./youtube.scss']
})
export class YoutubeComponent {
    slide: Slide = Slide.Youtube;
    youtubeVideoTitle: string;
    youtubeVideoLink: string;
    background: VideoBackground | null = null;
    param: string;

    constructor(
        private http: HttpClient,
        private cacheService: CacheService,
        private translationService: TranslationService,
        private translationPipe: TranslationPipe
    ) {}

    ngOnInit() {
        try {
            this.http.get('cache/' + LocalCacheFileName.Config).subscribe((config: NodeConfig_Sync) => {
                this.translationPipe.getTranslationFromFileAsync('title.youtube').then(text => {
                    if (!this.youtubeVideoTitle) this.youtubeVideoTitle = text;
                });

                var numberOfVideos = config.slides.config['youtube'].videos.length;
                var video =
                    config.slides.config['youtube'].videos[this.cacheService.getIndexForCachedElements(IndexableCache.youtube, numberOfVideos)];

                this.translationService.chooseTranslationAsync(video.title).then(value => (this.youtubeVideoTitle = value));

                const videoId = video.embedUrl.split('/').pop();

                this.param = '?rel=0&amp;controls=0&amp;showinfo=0&amp;autoplay=1&amp;loop=1&amp;mute=1&amp;playlist=' + encodeURIComponent(videoId);

                this.youtubeVideoLink = video.embedUrl + this.param;
                LoggingService.debug(YoutubeComponent.name, 'Preparing slide', 'Done');

                this.background = video.background;
            });
        } catch (error) {
            LoggingService.error(YoutubeComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
