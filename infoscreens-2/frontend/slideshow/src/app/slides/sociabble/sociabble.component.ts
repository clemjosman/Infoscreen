import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment-timezone';
import { IndexableCache } from '../../enums/indexableCache';
import { ImageHelper } from '../../helpers/Images.helper';
import { TranslationPipe } from '../../pipes/translation.pipe';
import { CacheService } from '../../services/cache.service';
import { DateService } from '../../services/date.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../common';

@Component({
    templateUrl: './sociabble.html',
    styleUrls: ['./sociabble.scss']
})
export class SociabbleComponent {
    slide: Slide = Slide.Sociabble;
    postImageSource: string = 'assets/images/banner/sociabbleBanner.png';
    title: string = undefined;
    post: any = undefined;

    constructor(
        private http: HttpClient,
        private cacheService: CacheService,
        private dateService: DateService,
        private translationPipe: TranslationPipe,
        private imageHelper: ImageHelper
    ) {
        try {
            this.translationPipe.getTranslationFromFileAsync('title.sociabblepost').then(text => {
                if (!this.title) this.title = text;
            });

            this.http.get('cache/' + LocalCacheFileName.Sociabble).subscribe(async data => {
                var postData: any = data;
                postData = postData.map(e => {
                    e.date = this.createDate(e.publicationDate);
                    return e;
                });

                var numberOfPosts = Object.keys(postData).length;
                this.post = postData[this.cacheService.getIndexForCachedElements(IndexableCache.sociabble, numberOfPosts)];

                if (!this.post) return;

                if (this.post.image) {
                    if (await this.imageHelper.isImageUrlValidAsync(this.post.image)) this.postImageSource = this.post.image;
                }
                LoggingService.debug(SociabbleComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(SociabbleComponent.name, 'Preparing slide', 'An error occured while getting slide data', error);
        }
    }

    createDate(createdAt: string): string {
        var created = moment(createdAt);
        return this.dateService.formatDatetoLocal(created, 'L');
    }
}
