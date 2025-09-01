import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IndexableCache } from '../../enums/indexableCache';
import { CacheService } from '../../services/cache.service';
import { DateService } from '../../services/date.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../common';

@Component({
    templateUrl: './twentyMin.html',
    styleUrls: ['./twentyMin.scss']
})
export class TwentyMinComponent {
    slide: Slide = Slide.TwentyMin;

    news: {
        channelTitle: string;
        title: string;
        description: string;
        publicationDate: string;
        imageSrc: string;
        link: string;
    }[];
    numberOfChannels: number = 0;

    constructor(private http: HttpClient, private cacheService: CacheService, private dateService: DateService) {
        try {
            this.news = Array(3).fill({
                channelTitle: '',
                title: '',
                description: '',
                publicationDate: '',
                imageSrc: null,
                link: null
            });

            this.http.get('cache/' + LocalCacheFileName.TwentyMin).subscribe(data => {
                var cachedData: any = data;
                this.numberOfChannels = Math.min(cachedData.length, 3);

                for (var i = 0; i < 3; i++) {
                    if (i > this.numberOfChannels - 1) this.handleCachedChannel(cachedData, this.numberOfChannels - 1, i);
                    else this.handleCachedChannel(cachedData, i, i);
                }

                LoggingService.debug(TwentyMinComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(TwentyMinComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }

    handleCachedChannel(cachedData, channelIndex, newsIndex) {
        var indexableCache: IndexableCache;
        switch (channelIndex) {
            case 0:
                indexableCache = IndexableCache.twentyMinChannel1;
                break;

            case 1:
                indexableCache = IndexableCache.twentyMinChannel2;
                break;

            case 2:
                indexableCache = IndexableCache.twentyMinChannel3;
                break;

            default:
                throw Error('Channel index ' + channelIndex + ' is not valid or too big for twenty min indexable cache.');
        }
        var channel = cachedData[channelIndex];
        var numberOfNewsForChannel = Object.keys(channel.news).length;
        var tmpNews = channel.news[this.cacheService.getIndexForCachedElements(indexableCache, numberOfNewsForChannel)];
        if (tmpNews) {
            tmpNews.channelTitle = channel.title;
            tmpNews.publicationDate = this.dateService.formatDatetoLocal(tmpNews.publicationDate, 'L');
        }
        this.news[newsIndex] = tmpNews;
    }
}
