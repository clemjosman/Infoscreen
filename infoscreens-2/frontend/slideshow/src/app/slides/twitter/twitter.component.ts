import { Component, ViewEncapsulation } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IndexableCache } from '../../enums/indexableCache';
import { SlideContentType } from '../../enums/slideContentType';
import { ImageHelper } from '../../helpers/Images.helper';
import { CacheService } from '../../services/cache.service';
import { DateService } from '../../services/date.service';
import { LoggingService } from '../../services/logging.service';
import { LocalCacheFileName, Slide } from '../../../../../common';

@Component({
    templateUrl: './twitter.html',
    styleUrls: ['./twitter.scss']
})
export class TwitterComponent {
    slide: Slide = Slide.Twitter;

    tweets: {
        id: number;
        content: string;
        hashtags: string[];
        imgSrc: string;
        imgSrcs: string[];
        publishDate: string;
        userName: string;
        userScreenName: string;
        userImgSrc: string;
        retweetCount: number;
        favoriteCount: number;
        url: string;
        urlEncoded: string;
    }[] = [];

    constructor(private http: HttpClient, private cacheService: CacheService, private dateService: DateService, private imageHelper: ImageHelper) {
        try {
            this.http.get('cache/' + LocalCacheFileName.Twitter).subscribe(async data => {
                var numberOfTweets = Object.keys(data).length;

                for (var i = 0; i < Math.min(3, numberOfTweets); i++) {
                    this.tweets.push(data[this.cacheService.getIndexForCachedElements(IndexableCache.twitter, numberOfTweets)]);
                }

                this.tweets.map(async t => {
                    t.publishDate = this.dateService.formatDatetoLocal(t.publishDate, 'lll');
                    t.content = t.content
                        .split(' ')
                        .map(s => {
                            return this.formatStringforDisplay(s);
                        })
                        .join(' ');
                    if (t.imgSrcs && t.imgSrcs.length > 0) {
                        if (await this.imageHelper.isImageUrlValidAsync(t.imgSrcs[0])) t.imgSrc = t.imgSrcs[0];
                        else t.imgSrc = null;
                    }
                    return t;
                });

                LoggingService.debug(TwitterComponent.name, 'Preparing slide', 'Done');
            });
        } catch (error) {
            LoggingService.error(TwitterComponent.name, 'Preparing slide', 'An error occured while getting the slide data', error);
        }
    }

    formatStringforDisplay(string: string): string {
        if (string.startsWith('#')) {
            string = '<span class="hashtag">' + string + '</span>';
        } else if (string.startsWith('@')) {
            string = '<span class="mention">' + string + '</span>';
        } else if (string.startsWith('http')) {
            string = '<span class="link">' + string + '</span>';
        }
        return string;
    }
}
