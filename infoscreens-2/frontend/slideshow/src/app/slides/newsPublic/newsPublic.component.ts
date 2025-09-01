import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IndexableCache } from '../../enums/indexableCache';
import { ImageHelper } from '../../helpers/Images.helper';
import { CacheService } from '../../services/cache.service';
import { DateService } from '../../services/date.service';
import { LoggingService } from '../../services/logging.service';
import { ActemiumNews, LocalCacheFileName, Slide } from '../../../../../common';

@Component({
    templateUrl: './newsPublic.html',
    styleUrls: ['./newsPublic.scss']
})
export class NewsPublicComponent {
    readonly DEFAULT_IMAGE_NEWS = 'assets/images/News_Default_Image.jpg';
    slide: Slide = Slide.NewsPublic;
    newsImageLink: string = this.DEFAULT_IMAGE_NEWS;
    news: ActemiumNews;

    constructor(private http: HttpClient, private cacheService: CacheService, private dateService: DateService, private imageHelper: ImageHelper) {
        try {
            this.http.get('cache/' + LocalCacheFileName.NewsPublic).subscribe(async data => {
                var numberOfNews = Object.keys(data).length;
                var tmpNews: ActemiumNews = data[this.cacheService.getIndexForCachedElements(IndexableCache.newsPublic, numberOfNews)];
                tmpNews.publicationDate = tmpNews.publicationDate ? this.dateService.formatDatetoLocal(tmpNews.publicationDate, 'L') : null;
                this.news = tmpNews;

                if (this.news.imgSrc) {
                    if (await this.imageHelper.isImageUrlValidAsync(this.news.imgSrc)) this.newsImageLink = this.news.imgSrc;
                }

                LoggingService.debug(NewsPublicComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(NewsPublicComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
