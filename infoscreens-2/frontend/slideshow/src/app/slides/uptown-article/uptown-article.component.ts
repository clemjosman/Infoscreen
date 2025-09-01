import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment-timezone';
import { IndexableCache } from '../../enums/indexableCache';
import { SlideContentType } from '../../enums/slideContentType';
import { ImageHelper } from '../../helpers/Images.helper';
import { TranslationPipe } from '../../pipes/translation.pipe';
import { CacheService } from '../../services/cache.service';
import { DateService } from '../../services/date.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../common';

@Component({
    templateUrl: './uptown-article.html',
    styleUrls: ['./uptown-article.scss']
})
export class UptownArticleComponent {
    slide: Slide = Slide.UptownArticle;
    articleImageSource: string = 'assets/images/banner/uptownBanner.jpg';
    title: string = undefined;
    article: any = undefined;

    constructor(
        private http: HttpClient,
        private cacheService: CacheService,
        private dateService: DateService,
        private translationPipe: TranslationPipe,
        private imageHelper: ImageHelper
    ) {
        try {
            this.translationPipe.getTranslationFromFileAsync('title.uptownarticle').then(text => {
                if (!this.title) this.title = text;
            });

            this.http.get('cache/' + LocalCacheFileName.UptownArticle).subscribe(async data => {
                var articleData: any = data;
                articleData = articleData.map(e => {
                    e.date = this.createDate(e.createdAt);
                    return e;
                });

                var numberOfArticles = Object.keys(articleData).length;
                this.article = articleData[this.cacheService.getIndexForCachedElements(IndexableCache.uptownArticle, numberOfArticles)];
                console.log(this.article);

                if (!this.article) return;

                if (this.article.pictures && this.article.pictures[0] && this.article.pictures[0].url) {
                    if (await this.imageHelper.isImageUrlValidAsync(this.article.pictures[0].url))
                        this.articleImageSource = this.article.pictures[0].url;
                }
                LoggingService.debug(UptownArticleComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(UptownArticleComponent.name, 'Preparing slide', 'An error occured while getting slide data', error);
        }
    }

    createDate(createdAt: string): string {
        var created = moment(createdAt);
        return this.dateService.formatDatetoLocal(created, 'L');
    }
}
