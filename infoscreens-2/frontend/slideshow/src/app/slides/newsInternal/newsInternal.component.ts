import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IndexableCache } from '../../enums/indexableCache';
import { CacheService } from '../../services/cache.service';
import { DateService } from '../../services/date.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, News, NewsLayoutBox, Slide } from '../../../../../common';
import { BrandingService } from '../../services';
import { Branding } from '../../../../../common/enums/brandings';
import { ImageHelper } from '../../helpers/Images.helper';

interface News_Display extends News {
    text: NewsLayoutBox;
    file: NewsLayoutBox;
    fileFirst: boolean;
}

@Component({
    templateUrl: './newsInternal.html',
    styleUrls: ['./newsInternal.scss']
})
export class NewsInternalComponent {
    readonly DEFAULT_IMAGE_NEWS = 'assets/images/News_Default_Image.jpg';
    readonly slide: Slide = Slide.NewsInternal;
    newsFileLink: string = this.DEFAULT_IMAGE_NEWS;
    showFooter: boolean = false;
    news: News_Display | undefined;

    constructor(private http: HttpClient, private cacheService: CacheService, private dateService: DateService, private imageHelper: ImageHelper) {
        try {
            this.showFooter = BrandingService.branding === Branding.ActemiumCH;
            this.http.get('cache/' + LocalCacheFileName.NewsInternal).subscribe(async data => {
                var numberOfNews = Object.keys(data).length;
                var news: News_Display = data[this.cacheService.getIndexForCachedElements(IndexableCache.newsInternal, numberOfNews)];
                if (!news) {
                    this.news = undefined;
                    return;
                }
                news.publicationDate = news.publicationDate ? this.dateService.formatDatetoLocal(news.publicationDate, 'L') : null;

                switch (news.box1.content) {
                    case 'text':
                        news.text = news.box1;
                        news.file = news.box2;
                        news.fileFirst = false;
                        break;
                    case 'text':
                    default:
                        news.text = news.box2;
                        news.file = news.box1;
                        news.fileFirst = true;
                        break;
                }

                this.news = news;
                if (this.news.fileExtension != 'pdf' && this.news.fileSrc) {
                    if (await this.imageHelper.isImageUrlValidAsync(this.news.fileSrc)) this.newsFileLink = this.news.fileSrc;
                } else if (this.news.fileExtension == 'pdf' && this.news.fileSrc) {
                    this.newsFileLink = this.news.fileSrc;
                }

                LoggingService.debug(NewsInternalComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(NewsInternalComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }
}
